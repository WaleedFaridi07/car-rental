import { useEffect } from 'react'
import { useForm } from 'react-hook-form'
import { useMutation, useQuery } from '@tanstack/react-query'
import toast from 'react-hot-toast'
import { rentalApi } from '../api/client'
import type { PickupRegistrationDto } from '../types'

export default function RegisterPickup() {
  const { register, handleSubmit, setValue, reset, formState: { errors } } = useForm<PickupRegistrationDto>()

  const { data: categories } = useQuery({
    queryKey: ['categories'],
    queryFn: rentalApi.getAllCategories,
  })

  const pickupMutation = useMutation({
    mutationFn: rentalApi.registerPickup,
    onSuccess: (bookingNumber) => {
      toast.success(`Pickup registered successfully! Booking: ${bookingNumber}`, { duration: 5000 })
      reset()
      generateNewBookingNumber()
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.error || 'Failed to register pickup')
    },
  })

  const generateNewBookingNumber = async () => {
    try {
      const bookingNumber = await rentalApi.generateBookingNumber()
      setValue('bookingNumber', bookingNumber)
    } catch (error) {
      toast.error('Failed to generate booking number')
    }
  }

  useEffect(() => {
    generateNewBookingNumber()
  }, [])

  const onSubmit = (data: PickupRegistrationDto) => {
    pickupMutation.mutate(data)
  }

  const getCategoryIcon = (categoryId: number) => {
    if (categoryId === 1) return '🚗' 
    if (categoryId === 2) return '🚙' 
    if (categoryId === 3) return '🚚'
    return '🚗'
  }

  const getCategoryName = (categoryId: number) => {
    if (categoryId === 1) return 'Small Car'
    if (categoryId === 2) return 'Combi'
    if (categoryId === 3) return 'Truck'
    return 'Unknown'
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div className="bg-white rounded-lg shadow-lg p-8">
        <h2 className="text-3xl font-bold text-gray-800 mb-6">Register Pickup</h2>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Booking Number */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Booking Number
            </label>
            <div className="flex gap-2">
              <input
                {...register('bookingNumber', { required: 'Booking number is required' })}
                readOnly
                className="flex-1 px-4 py-2 border border-gray-300 rounded-lg bg-gray-50"
              />
              <button
                type="button"
                onClick={generateNewBookingNumber}
                className="px-4 py-2 bg-gray-600 text-white rounded-lg hover:bg-gray-700"
              >
                Generate New
              </button>
            </div>
            {errors.bookingNumber && (
              <p className="text-red-500 text-sm mt-1">{errors.bookingNumber.message}</p>
            )}
          </div>

          {/* Registration Number */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Registration Number
            </label>
            <input
              {...register('registrationNumber', { required: 'Registration number is required' })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              placeholder="ABC123"
            />
            {errors.registrationNumber && (
              <p className="text-red-500 text-sm mt-1">{errors.registrationNumber.message}</p>
            )}
          </div>

          {/* Customer SSN */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Customer Social Security Number
            </label>
            <input
              {...register('customerSocialSecurityNumber', { required: 'SSN is required' })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              placeholder="19900101-1234"
            />
            {errors.customerSocialSecurityNumber && (
              <p className="text-red-500 text-sm mt-1">{errors.customerSocialSecurityNumber.message}</p>
            )}
          </div>

          {/* Car Category */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Car Category
            </label>
            <select
              {...register('carCategoryId', { required: 'Category is required', valueAsNumber: true })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            >
              <option value="">Select a category</option>
              {categories?.map((cat) => (
                <option key={cat.id} value={cat.id}>
                  {getCategoryIcon(cat.id)} {getCategoryName(cat.id)} - {cat.baseDayRental} SEK/day
                  {cat.baseKmPrice > 0 && ` + ${cat.baseKmPrice} SEK/km`}
                </option>
              ))}
            </select>
            {errors.carCategoryId && (
              <p className="text-red-500 text-sm mt-1">{errors.carCategoryId.message}</p>
            )}
          </div>

          {/* Pickup DateTime */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Pickup Date & Time
            </label>
            <input
              type="datetime-local"
              {...register('pickupDateTime', { 
                required: 'Pickup date/time is required',
                validate: (value) => {
                  const pickupDate = new Date(value)
                  const now = new Date()
                  if (pickupDate < now) {
                    return 'Pickup date cannot be in the past'
                  }
                  return true
                }
              })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
            />
            {errors.pickupDateTime && (
              <p className="text-red-500 text-sm mt-1">{errors.pickupDateTime.message}</p>
            )}
          </div>

          {/* Pickup Meter Reading */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Meter Reading (km)
            </label>
            <input
              type="number"
              {...register('pickupMeterReading', { 
                required: 'Meter reading is required',
                valueAsNumber: true,
                min: { value: 0, message: 'Must be positive' }
              })}
              className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
              placeholder="10000"
            />
            {errors.pickupMeterReading && (
              <p className="text-red-500 text-sm mt-1">{errors.pickupMeterReading.message}</p>
            )}
          </div>

          {/* Buttons */}
          <div className="flex gap-4">
            <button
              type="submit"
              disabled={pickupMutation.isPending}
              className="flex-1 bg-blue-500 text-white py-3 rounded-lg hover:bg-blue-600 disabled:bg-gray-400 font-medium"
            >
              {pickupMutation.isPending ? 'Registering...' : 'Register Pickup'}
            </button>
            <button
              type="button"
              onClick={() => {
                reset()
                generateNewBookingNumber()
              }}
              className="px-6 py-3 border border-gray-300 rounded-lg hover:bg-gray-50 font-medium"
            >
              Clear
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}
