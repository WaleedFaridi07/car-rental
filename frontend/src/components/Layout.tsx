import { useState } from 'react'
import { Link, useLocation } from 'react-router-dom'

interface LayoutProps {
  children: React.ReactNode
}

export default function Layout({ children }: LayoutProps) {
  const [sidebarOpen, setSidebarOpen] = useState(true)
  const location = useLocation()

  const isActive = (path: string) => location.pathname === path

  return (
    <div className="flex h-screen bg-gray-100">
      {/* Sidebar */}
      <aside
        className={`${
          sidebarOpen ? 'w-64' : 'w-20'
        } bg-white shadow-lg transition-all duration-300 ease-in-out`}
      >
        <div className="flex items-center justify-between p-4 border-b">
          {sidebarOpen && <h1 className="text-xl font-bold text-gray-800">Car Rental</h1>}
          <button
            onClick={() => setSidebarOpen(!sidebarOpen)}
            className="p-2 rounded-lg hover:bg-gray-100"
          >
            {sidebarOpen ? '←' : '→'}
          </button>
        </div>

        <nav className="p-4 space-y-2">
          <Link
            to="/rentals"
            className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
              isActive('/rentals') || isActive('/')
                ? 'bg-blue-500 text-white'
                : 'text-gray-700 hover:bg-gray-100'
            }`}
          >
            <span className="text-xl">📋</span>
            {sidebarOpen && <span>All Rentals</span>}
          </Link>

          <Link
            to="/pickup"
            className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
              isActive('/pickup')
                ? 'bg-blue-500 text-white'
                : 'text-gray-700 hover:bg-gray-100'
            }`}
          >
            <span className="text-xl">🚗</span>
            {sidebarOpen && <span>Register Pickup</span>}
          </Link>

          <Link
            to="/return"
            className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
              isActive('/return')
                ? 'bg-blue-500 text-white'
                : 'text-gray-700 hover:bg-gray-100'
            }`}
          >
            <span className="text-xl">✅</span>
            {sidebarOpen && <span>Register Return</span>}
          </Link>
        </nav>
      </aside>

      {/* Main Content */}
      <main className="flex-1 overflow-auto">
        <div className="p-8">{children}</div>
      </main>
    </div>
  )
}
