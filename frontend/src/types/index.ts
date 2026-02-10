export interface CarCategory {
  id: number
  category: string
  baseDayRental: number
  baseKmPrice: number
}

export interface Rental {
  bookingNumber: string
  registrationNumber: string
  customerSocialSecurityNumber: string
  carCategoryId: number
  carCategory?: CarCategory
  pickupDateTime: string
  pickupMeterReading: number
  returnDateTime?: string
  returnMeterReading?: number
  totalPrice?: number
}

export interface PickupRegistrationDto {
  bookingNumber: string
  registrationNumber: string
  customerSocialSecurityNumber: string
  carCategoryId: number
  pickupDateTime: string
  pickupMeterReading: number
}

export interface ReturnRegistrationDto {
  bookingNumber: string
  returnDateTime: string
  returnMeterReading: number
}

export interface RentalReturnResult {
  bookingNumber: string
  daysRented: number
  kmDriven: number
  totalPrice: number
}
