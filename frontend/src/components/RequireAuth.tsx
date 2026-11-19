import { Navigate, Outlet, useLocation } from 'react-router-dom'
import { useAuth } from '../features/auth/useAuth'

export function RequireAuth({ adminOnly = false }: { adminOnly?: boolean }) {
    const { user, loading, isAdmin } = useAuth()
    const location = useLocation()

    if (loading) return <div className="page-loading">Yükleniyor…</div>

    // Giriş sonrası kaldığı sayfaya dönebilsin diye adresi yanında taşı
    if (!user) return <Navigate to="/login" replace state={{ from: location.pathname }} />

    if (adminOnly && !isAdmin) return <Navigate to="/" replace />

    return <Outlet />
}