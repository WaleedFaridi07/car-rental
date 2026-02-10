import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout'
import RegisterPickup from './pages/RegisterPickup'
import RegisterReturn from './pages/RegisterReturn'
import AllRentals from './pages/AllRentals'

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<AllRentals />} />
        <Route path="/pickup" element={<RegisterPickup />} />
        <Route path="/return" element={<RegisterReturn />} />
        <Route path="/rentals" element={<AllRentals />} />
      </Routes>
    </Layout>
  )
}

export default App
