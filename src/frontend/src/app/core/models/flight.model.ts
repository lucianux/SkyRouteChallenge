export interface FlightSearchParams {
  origin: string;
  destination: string;
  departureDate: string;
  passengers: number;
  cabinClass: string;
}

export interface FlightResponse {
  flightId: string;
  provider: string;
  flightNumber: string;
  departureTime: string;
  arrivalTime: string;
  durationMinutes: number;
  cabinClass: string;
  pricePerPassenger: number;
  priceTotal: number;
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