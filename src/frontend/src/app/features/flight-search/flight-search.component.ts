import { Component, inject, signal, computed } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { FlightService } from '../../core/services/flight.service';
import { FlightResponse } from '../../core/models/flight.model';
import { FlightCardComponent } from './components/flight-card.component';

@Component({
  selector: 'app-flight-search',
  standalone: true,
  imports: [ReactiveFormsModule, FlightCardComponent],
  templateUrl: './flight-search.component.html',
  styleUrl: './flight-search.component.scss'
})
export class FlightSearchComponent {
  private fb = inject(FormBuilder);
  private flightService = inject(FlightService);
  private router = inject(Router);

  // Signals para manejar el estado local reactivo
  flights = signal<FlightResponse[]>([]);
  isLoading = signal<boolean>(false);
  sortBy = signal<string>('price'); // 'price' | 'duration' | 'departure'

  // Formulario Reactivo
  searchForm = this.fb.group({
    origin: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]],
    destination: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]],
    departureDate: ['', Validators.required],
    passengers: [1, [Validators.required, Validators.min(1), Validators.max(9)]],
    cabinClass: ['Economy', Validators.required]
  });

  // Computed Signal: Se ejecuta automáticamente en memoria cuando cambia 'flights' o 'sortBy'
  // Reemplaza de forma eficiente a los Pipes de ordenamiento tradicionales
  orderedFlights = computed(() => {
    const list = [...this.flights()];
    const criteria = this.sortBy();

    if (criteria === 'price') {
      return list.sort((a, b) => a.priceTotal - b.priceTotal);
    } else if (criteria === 'duration') {
      return list.sort((a, b) => a.durationMinutes - b.durationMinutes);
    } else if (criteria === 'departure') {
      return list.sort((a, b) => new Date(a.departureTime).getTime() - new Date(b.departureTime).getTime());
    }
    return list;
  });

  onSearch(): void {
    if (this.searchForm.invalid) return;

    this.isLoading.set(true);
    const formValue = this.searchForm.value as any;

    this.flightService.searchFlights(formValue).subscribe({
      next: (results) => {
        this.flights.set(results);
        this.isLoading.set(false);
      },
      error: (err) => {
        console.error('Error buscando vuelos:', err);
        this.isLoading.set(false);
      }
    });
  }

  changeSort(criteria: string): void {
    this.sortBy.set(criteria);
  }

  onFlightSelected(flight: FlightResponse): void {
    this.flightService.selectFlight(flight);
    this.router.navigate(['/booking']);
  }
}