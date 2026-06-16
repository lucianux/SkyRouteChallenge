import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { FlightSearchParams, FlightResponse, BookingRequest, BookingResponse } from '../models/flight.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FlightService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}`;

  selectedFlight = signal<FlightResponse | null>(null);
  currentSearchParams = signal<FlightSearchParams | null>(null);

  searchFlights(params: FlightSearchParams): Observable<FlightResponse[]> {
    // We set the current parameters in the Signal
    this.currentSearchParams.set(params);

    const httpParams = new HttpParams()
      .set('origin', params.origin)
      .set('destination', params.destination)
      .set('departureDate', params.departureDate)
      .set('passengers', params.passengers.toString())
      .set('cabinClass', params.cabinClass);

    return this.http.get<FlightResponse[]>(`${this.apiUrl}/flights`, { params: httpParams });
  }

  createBooking(request: BookingRequest): Observable<BookingResponse> {
    return this.http.post<BookingResponse>(`${this.apiUrl}/bookings`, request);
  }

  selectFlight(flight: FlightResponse): void {
    this.selectedFlight.set(flight);
  }

  clearState(): void {
    this.selectedFlight.set(null);
    this.currentSearchParams.set(null);
  }
}