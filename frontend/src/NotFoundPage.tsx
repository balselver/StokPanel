import { Link } from 'react-router-dom'

export function NotFoundPage() {
    return (
        <div className="auth-page">
            <div className="card auth-card">
                <h1>Sayfa bulunamadı</h1>
                <p className="muted">Aradığınız adres mevcut değil.</p>
                <Link className="btn btn-primary" to="/">Ana sayfaya dön</Link>
            </div>
        </div>
    )
}