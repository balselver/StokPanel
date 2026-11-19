import { useState, type FormEvent } from 'react'
import { Link, Navigate, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from './useAuth'

export function LoginPage() {
    const { user, login } = useAuth()
    const navigate = useNavigate()
    const location = useLocation()
    const [error, setError] = useState<string | null>(null)
    const [submitting, setSubmitting] = useState(false)

    const from = (location.state as { from?: string } | null)?.from ?? '/'

    if (user) return <Navigate to={from} replace />

    async function handleSubmit(e: FormEvent<HTMLFormElement>) {
        e.preventDefault()
        const form = new FormData(e.currentTarget)

        setError(null)
        setSubmitting(true)
        try {
            await login({
                email: String(form.get('email')),
                password: String(form.get('password')),
                rememberMe: form.get('rememberMe') === 'on',
            })
            navigate(from, { replace: true })
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Giriş yapılamadı')
        } finally {
            setSubmitting(false)
        }
    }

    return (
        <div className="auth-page">
            <div className="card auth-card">
                <h1 className="auth-brand">📦 StokPanel</h1>
                <p className="muted">Hesabınıza giriş yapın</p>

                {error && <div className="alert alert-error">{error}</div>}

                <form onSubmit={handleSubmit}>
                    <div className="field">
                        <label htmlFor="email">E-posta</label>
                        <input id="email" name="email" type="email" autoComplete="email" required />
                    </div>
                    <div className="field">
                        <label htmlFor="password">Şifre</label>
                        <input id="password" name="password" type="password" autoComplete="current-password" required />
                    </div>
                    <label className="checkbox">
                        <input name="rememberMe" type="checkbox" /> Beni hatırla
                    </label>

                    <button className="btn btn-primary btn-block" disabled={submitting}>
                        {submitting ? 'Giriş yapılıyor…' : 'Giriş yap'}
                    </button>
                </form>

                <p className="auth-footer">
                    Hesabınız yok mu? <Link to="/kayit">Kayıt olun</Link>
                </p>
            </div>
        </div>
    )
}