import { Component, input, output } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { FlightResponse } from '../../../core/models/flight.model';

@Component({
  selector: 'app-flight-card',
  standalone: true,
  imports: [CurrencyPipe, DatePipe],
  templateUrl: './flight-card.component.html',
  styleUrl: './flight-card.component.scss'
})
export class FlightCardComponent {
  // Signal Inputs (Sintaxis moderna de Angular 21)
  flight = input.required<FlightResponse>();
  
  // Output moderno (reemplaza a @Output() con EventEmitter)
  selectFlight = output<FlightResponse>();

  onSelect(): void {
    this.selectFlight.emit(this.flight());
  }

  formatDuration(minutes: number): string {
    const hours = Math.floor(minutes / 60);
    const mins = minutes % 60;
    return `${hours}h ${mins}m`;
  }
}