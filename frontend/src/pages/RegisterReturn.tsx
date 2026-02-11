import { useState, useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { useMutation, useQuery } from '@tanstack/react-query'
import { useSearchParams } from 'react-router-dom'
import toast from 'react-hot-toast'
import { rentalApi } from '../api/client'
import type { ReturnRegistrationDto, RentalReturnResult } from '../types'

export default function RegisterReturn() {
  const [searchParams] = useSearchParams()
  const [result, setResult] = useState<RentalReturnResult | null>(null)
  const { register, handleSubmit, setValue, watch, formState: { errors } } = useForm<ReturnRegistrationDto>()
  
  const bookingNumber = watch('bookingNumber')
  
  const { data: rentals } = useQuery({
    queryKey: ['rentals'],
    queryFn: rentalApi.getAllRentals,
  })
  
  const currentRental = rentals?.find(r => r.bookingNumber === bookingNumber)

  useEffect(() => {
    const bookingNumber = searchParams.get('booking')
    if (bookingNumber) {
      setValue('bookingNumber', bookingNumber)
    }
  }, [searchParams, setValue])

  const returnMutation = useMutation({
    mutationFn: rentalApi.registerReturn,
    onSuccess: (data) => {
      setResult(data)
      toast.success('Return registered successfully!', { duration: 5000 })
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.error || 'Failed to register return')
    },
  })

  const onSubmit = (data: ReturnRegistrationDto) => {
    setResult(null)
    returnMutation.mutate(data)
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div className="bg-white rounded-lg shadow-lg p-8">
        <h2 className="text-3xl font-bold text-gray-800 mb-6">Register Return</h2>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Booking Number */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Booking Number
            </label>
            <input
              {...register('bookingNumber', { required: 'Booking number is required' })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              placeholder="BK202602101230123"
            />
            {errors.bookingNumber && (
              <p className="text-red-500 text-sm mt-1">{errors.bookingNumber.message}</p>
            )}
          </div>

          {/* Return DateTime */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Return Date & Time
            </label>
            <input
              type="datetime-local"
              {...register('returnDateTime', { 
                required: 'Return date/time is required',
                validate: (value) => {
                  if (!currentRental?.pickupDateTime) return true
                  const returnDate = new Date(value)
                  const pickupDate = new Date(currentRental.pickupDateTime)
                  if (returnDate < pickupDate) {
                    return 'Return date cannot be earlier than pickup date'
                  }
                  return true
                }
              })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            />
            {errors.returnDateTime && (
              <p className="text-red-500 text-sm mt-1">{errors.returnDateTime.message}</p>
            )}
          </div>

          {/* Return Meter Reading */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Meter Reading (km)
            </label>
            <input
              type="number"
              {...register('returnMeterReading', { 
                required: 'Meter reading is required',
                valueAsNumber: true,
                min: { value: 0, message: 'Must be positive' },
                validate: (value) => {
                  if (!currentRental?.pickupMeterReading) return true
                  if (value < currentRental.pickupMeterReading) {
                    return `Return meter reading must be greater than pickup reading (${currentRental.pickupMeterReading} km)`
                  }
                  return true
                }
              })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              placeholder="10200"
            />
            {errors.returnMeterReading && (
              <p className="text-red-500 text-sm mt-1">{errors.returnMeterReading.message}</p>
            )}
          </div>

          {/* Submit Button */}
          <button
            type="submit"
            disabled={returnMutation.isPending}
            className="w-full bg-green-500 text-white py-3 rounded-lg hover:bg-green-600 disabled:bg-gray-400 font-medium"
          >
            {returnMutation.isPending ? 'Processing...' : 'Register Return'}
          </button>
        </form>

        {/* Result Card */}
        {result && (
          <div className="mt-8 bg-green-50 border border-green-200 rounded-lg p-6">
            <h3 className="text-xl font-bold text-green-800 mb-4">Return Successful!</h3>
            <div className="space-y-2">
              <div className="flex justify-between">
                <span className="text-gray-600">Booking Number:</span>
                <span className="font-semibold">{result.bookingNumber}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Days Rented:</span>
                <span className="font-semibold">{result.daysRented} days</span>
              </div>
              <div className="flex justify-between">
                <span className="text-gray-600">Kilometers Driven:</span>
                <span className="font-semibold">{result.kmDriven} km</span>
              </div>
              <div className="flex justify-between pt-4 border-t border-green-300">
                <span className="text-lg font-bold text-gray-800">Total Price:</span>
                <span className="text-2xl font-bold text-green-600">{result.totalPrice.toFixed(2)} SEK</span>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  )
}
