import axios from 'axios'
import type { CarCategory, Rental, PickupRegistrationDto, ReturnRegistrationDto, RentalReturnResult } from '../types'

const api = axios.create({
  baseURL: 'http://localhost:5225/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

export const rentalApi = {
  generateBookingNumber: async (): Promise<string> => {
    const { data } = await api.get<{ bookingNumber: string }>('/rentals/generate-booking-number')
    return data.bookingNumber
  },

  registerPickup: async (dto: PickupRegistrationDto): Promise<string> => {
    const { data } = await api.post<string>('/rentals/pickup', dto)
    return data;
  },

  registerReturn: async (dto: ReturnRegistrationDto): Promise<RentalReturnResult> => {
    const { data } = await api.post<RentalReturnResult>('/rentals/return', dto)
    return data
  },

  getAllRentals: async (): Promise<Rental[]> => {
    const { data } = await api.get<Rental[]>('/rentals')
    return data
  },

  getRentalByBookingNumber: async (bookingNumber: string): Promise<Rental> => {
    const { data } = await api.get<Rental>(`/rentals/${bookingNumber}`)
    return data
  },

  getAllCategories: async (): Promise<CarCategory[]> => {
    const { data } = await api.get<CarCategory[]>('/categories')
    return data
  },
}
