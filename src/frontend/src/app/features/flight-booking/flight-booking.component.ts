import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators, FormArray } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { FlightService } from '../../core/services/flight.service';
import { BookingRequest, PassengerDetails } from '../../core/models/flight.model';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-flight-booking',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, CurrencyPipe],
  templateUrl: './flight-booking.component.html',
  styleUrl: './flight-booking.component.scss'
})
export class FlightBookingComponent implements OnInit {
  private fb = inject(FormBuilder);
  private flightService = inject(FlightService);
  private router = inject(Router);

  // We read the global signals from the core service
  flight = this.flightService.selectedFlight;
  searchParams = this.flightService.currentSearchParams;

  // Local states using Signals
  isSubmitting = signal<boolean>(false);
  bookingSuccess = signal<boolean>(false);
  bookingReference = signal<string>('');

  isInternational = computed(() => {
    const currentFlight = this.flight();
    return currentFlight?.isInternational ?? false;
  });

  // Computed Signals for dynamic UI rules
  documentLabel = computed(() => this.isInternational() ? 'Passport Number' : 'National ID');
  
  // Dynamic regex: Passport requires strict alphanumeric format, DNI only numbers
  documentRegex = computed(() => this.isInternational() ? '^[A-Z0-9]{6,12}$' : '^[0-9]{7,10}$');

  // Root Reactive Form
  bookingForm = this.fb.group({
    passengers: this.fb.array([])
  });

  get passengersArray(): FormArray {
    return this.bookingForm.get('passengers') as FormArray;
  }

  ngOnInit(): void {
    // If the user reloads the booking page and there is no flight selected, we send them to the home page.
    if (!this.flight() || !this.searchParams()) {
      this.router.navigate(['/search']);
      return;
    }

    // We dynamically initialize the FormArray according to the number of passengers originally searched for
    const passengerCount = this.searchParams()?.passengers || 1;
    for (let i = 0; i < passengerCount; i++) {
      this.passengersArray.push(this.createPassengerGroup());
    }
  }

  private createPassengerGroup() {
    return this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      // We inject the computed dynamic regex into the initial validation
      documentNumber: ['', [Validators.required, Validators.pattern(this.documentRegex())]]
    });
  }

  onSubmit(): void {
    if (this.bookingForm.invalid || !this.flight()) return;

    this.isSubmitting.set(true);

    const bookingRequest: BookingRequest = {
      flightId: this.flight()!.flightId,
      passengers: this.bookingForm.value.passengers as PassengerDetails[]
    };

    this.flightService.createBooking(bookingRequest).subscribe({
      next: (response) => {
        this.bookingReference.set(response.bookingReference);
        this.bookingSuccess.set(true);
        this.isSubmitting.set(false);
        this.flightService.clearState(); // We clean up the overall state after success
      },
      error: (err) => {
        console.error('Error al procesar la reserva:', err);
        this.isSubmitting.set(false);
        alert('Hubo un error al procesar tu reserva. Por favor, reintenta.');
      }
    });
  }
}