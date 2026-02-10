import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { rentalApi } from '../api/client'

export default function AllRentals() {
  const { data: rentals, isLoading } = useQuery({
    queryKey: ['rentals'],
    queryFn: rentalApi.getAllRentals,
    refetchInterval: 5000,
  })

  const getCategoryIcon = (categoryId?: number) => {
    if (!categoryId) return '🚗'
    if (categoryId === 1) return '🚗' // SmallCar
    if (categoryId === 2) return '🚙' // Combi
    if (categoryId === 3) return '🚚' // Truck
    return '🚗'
  }

  const getCategoryName = (categoryId?: number) => {
    if (!categoryId) return 'N/A'
    if (categoryId === 1) return 'Small Car'
    if (categoryId === 2) return 'Combi'
    if (categoryId === 3) return 'Truck'
    return 'Unknown'
  }

  const formatDateTime = (dateStr?: string) => {
    if (!dateStr) return '-'
    return new Date(dateStr).toLocaleString('sv-SE', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit',
    })
  }

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="text-xl text-gray-600">Loading rentals...</div>
      </div>
    )
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-3xl font-bold text-gray-800">All Rentals</h2>
        <div className="text-sm text-gray-600">
          Total: {rentals?.length || 0} rentals
        </div>
      </div>

      <div className="bg-white rounded-lg shadow-lg overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-200">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Booking #
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Registration
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider hidden md:table-cell">
                  Category
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider hidden lg:table-cell">
                  Customer SSN
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Pickup
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider hidden md:table-cell">
                  Return
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider hidden lg:table-cell">
                  Km Driven
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Price
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider sticky right-0 bg-gray-50">
                  Action
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {rentals?.map((rental) => {
                const isActive = !rental.returnDateTime
                const kmDriven = rental.returnMeterReading && rental.pickupMeterReading
                  ? rental.returnMeterReading - rental.pickupMeterReading
                  : null

                return (
                  <tr key={rental.bookingNumber} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      {rental.bookingNumber}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {rental.registrationNumber}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 hidden md:table-cell">
                      <span className="flex items-center gap-2">
                        <span className="text-xl">{getCategoryIcon(rental.carCategory?.id)}</span>
                        <span>{getCategoryName(rental.carCategory?.id)}</span>
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 hidden lg:table-cell">
                      {rental.customerSocialSecurityNumber}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      {formatDateTime(rental.pickupDateTime)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 hidden md:table-cell">
                      {formatDateTime(rental.returnDateTime)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900 hidden lg:table-cell">
                      {kmDriven !== null ? `${kmDriven} km` : '-'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-semibold text-gray-900">
                      {rental.totalPrice ? `${rental.totalPrice.toFixed(2)} SEK` : '-'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      {isActive ? (
                        <span className="px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full bg-yellow-100 text-yellow-800">
                          Active
                        </span>
                      ) : (
                        <span className="px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800">
                          Returned
                        </span>
                      )}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium sticky right-0 bg-white">
                      {isActive && (
                        <Link
                          to={`/return?booking=${rental.bookingNumber}`}
                          className="text-blue-600 hover:text-blue-900 font-medium"
                        >
                          Return
                        </Link>
                      )}
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>

          {(!rentals || rentals.length === 0) && (
            <div className="text-center py-12 text-gray-500">
              No rentals found. Start by registering a pickup!
            </div>
          )}
        </div>
      </div>
    </div>
  )
}
