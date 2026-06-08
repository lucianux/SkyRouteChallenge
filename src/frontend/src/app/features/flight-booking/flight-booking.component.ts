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

  // Leemos las Signals globales del servicio core
  flight = this.flightService.selectedFlight;
  searchParams = this.flightService.currentSearchParams;

  // States locales usando Signals
  isSubmitting = signal<boolean>(false);
  bookingSuccess = signal<boolean>(false);
  bookingReference = signal<string>('');

  // Computed Signal: Determina dinámicamente si el vuelo es internacional cruzando los datos
  // [Nota de Senior]: Mapeamos los 6 aeropuertos del enunciado para deducir el país localmente
  isInternational = computed(() => {
    const currentFlight = this.flight();
    const params = this.searchParams();
    if (!currentFlight || !params) return false;

    const getCountry = (airportCode: string): string => {
      const code = airportCode.toUpperCase();
      if (code === 'EZE' || code === 'AEP') return 'Argentina';
      if (code === 'JFK' || code === 'MIA') return 'USA';
      if (code === 'MAD' || code === 'BCN') return 'Spain';
      return 'Unknown';
    };

    return getCountry(params.origin) !== getCountry(params.destination);
  });

  // Computed Signals para las reglas dinámicas de la UI
  documentLabel = computed(() => this.isInternational() ? 'Passport Number' : 'National ID');
  
  // Regex dinámico: Pasaporte exige formato alfanumérico estricto, DNI solo números
  documentRegex = computed(() => this.isInternational() ? '^[A-Z0-9]{6,12}$' : '^[0-9]{7,10}$');

  // Formulario Reactivo Raíz
  bookingForm = this.fb.group({
    passengers: this.fb.array([])
  });

  get passengersArray(): FormArray {
    return this.bookingForm.get('passengers') as FormArray;
  }

  ngOnInit(): void {
    // Guardrail: Si el usuario recarga la página de reserva y no hay vuelo seleccionado, lo mandamos al home
    if (!this.flight() || !this.searchParams()) {
      this.router.navigate(['/search']);
      return;
    }

    // Inicializamos dinámicamente el FormArray según la cantidad de pasajeros buscados originalmente
    const passengerCount = this.searchParams()?.passengers || 1;
    for (let i = 0; i < passengerCount; i++) {
      this.passengersArray.push(this.createPassengerGroup());
    }
  }

  private createPassengerGroup() {
    return this.fb.group({
      fullName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      // Inyectamos el regex dinámico computado en la validación inicial
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
        this.flightService.clearState(); // Limpiamos el estado global tras el éxito
      },
      error: (err) => {
        console.error('Error al procesar la reserva:', err);
        this.isSubmitting.set(false);
        alert('Hubo un error al procesar tu reserva. Por favor, reintenta.');
      }
    });
  }
}