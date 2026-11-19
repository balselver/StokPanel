import { BrowserRouter, Route, Routes } from 'react-router-dom'
import { Layout } from './components/Layout.tsx'
import { RequireAuth } from './components/RequireAuth.tsx'
import { AdminPage } from './features/admin/AdminPage.tsx'
import { AuthProvider } from './features/auth/AuthProvider.tsx'
import { LoginPage } from './features/auth/LoginPages.tsx'
import { RegisterPage } from './features/auth/RegisterPage.tsx'
import { DashboardPage } from './features/dashboard/DashboardPage.tsx'
import { ProductsPage } from './features/products/ProductsPage.tsx'
import { NotFoundPage } from './NotFoundPage.tsx'

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/kayit" element={<RegisterPage />} />

          {/* Giriş gerekli */}
          <Route element={<RequireAuth />}>
            <Route element={<Layout />}>
              <Route index element={<DashboardPage />} />
              <Route path="urunler" element={<ProductsPage />} />

              {/* Sadece admin */}
              <Route element={<RequireAuth adminOnly />}>
                <Route path="yonetim" element={<AdminPage />} />
              </Route>
            </Route>
          </Route>

          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  )
}