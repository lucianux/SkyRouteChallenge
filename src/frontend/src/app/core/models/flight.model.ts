export interface FlightSearchParams {
  origin: string;
  destination: string;
  departureDate: string;
  passengers: number;
  cabinClass: string;
}

export interface AirportDetails {
  code: string;
  airportName: string;
  country: string;
}

export interface FlightResponse {
  flightId: string;
  origin: AirportDetails;
  destination: AirportDetails;
  provider: string;
  flightNumber: string;
  departureTime: string;
  arrivalTime: string;
  durationMinutes: number;
  cabinClass: string;
  pricePerPassenger: number;
  priceTotal: number;
  isInternational: boolean;
}

export interface PassengerDetails {
  fullName: string;
  email: string;
  documentNumber: string;
}

export interface BookingRequest {
  flightId: string;
  passengers: PassengerDetails[];
}

export interface BookingResponse {
  bookingReference: string;
  status: string;
  createdAt: string;
}