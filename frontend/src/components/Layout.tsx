import { NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../features/auth/useAuth.ts'
import { useConfirm } from './useConfirm.ts'

export function Layout() {
    const { user, isAdmin, logout } = useAuth()
    const confirm = useConfirm()

    async function handleLogout() {
        const ok = await confirm({
            title: 'Çıkış yapılsın mı?',
            message: 'Oturumunuz kapatılacak. Tekrar giriş yapmanız gerekecek.',
            confirmText: 'Çıkış yap',
        })
        if (ok) await logout()
    }

    return (
        <>
            <header className="app-header">
                <div className="app-header-inner">
                    <span className="brand">📦 StokPanel</span>

                    <nav className="main-nav">
                        <NavLink to="/" end>Gösterge</NavLink>
                        <NavLink to="/urunler">Ürünler</NavLink>
                        {isAdmin && <NavLink to="/yonetim">Yönetim</NavLink>}
                    </nav>

                    <div className="user-box">
                        <span className="user-name">{user?.fullName}</span>
                        <span className={isAdmin ? 'badge badge-admin' : 'badge'}>{isAdmin ? 'Admin' : 'Personel'}</span>
                        <button className="btn btn-secondary btn-sm" onClick={handleLogout}>Çıkış</button>
                    </div>
                </div>
            </header>

            <main className="container">
                <Outlet />
            </main>
        </>
    )
}