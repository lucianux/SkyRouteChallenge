import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'search',
    pathMatch: 'full'
  },
  {
    path: 'search',
    loadComponent: () => 
      import('./features/flight-search/flight-search.component')
        .then(m => m.FlightSearchComponent),
    title: 'SkyRoute - Buscar Vuelos' // Configura el título de la pestaña nativamente
  },
  {
    path: 'booking',
    loadComponent: () => 
      import('./features/flight-booking/flight-booking.component')
        .then(m => m.FlightBookingComponent),
    title: 'SkyRoute - Confirmar Reserva'
  },
  {
    path: '**',
    redirectTo: 'search' // Guardriel simple para redirigir cualquier ruta inválida
  }
];