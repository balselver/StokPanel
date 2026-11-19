import { useState, type FormEvent } from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from './useAuth'

export function RegisterPage() {
    const { user, register } = useAuth()
    const navigate = useNavigate()
    const [error, setError] = useState<string | null>(null)
    const [submitting, setSubmitting] = useState(false)

    if (user) return <Navigate to="/" replace />

    async function handleSubmit(e: FormEvent<HTMLFormElement>) {
        e.preventDefault()
        const form = new FormData(e.currentTarget)

        setError(null)
        setSubmitting(true)
        try {
            await register({
                fullName: String(form.get('fullName')),
                email: String(form.get('email')),
                password: String(form.get('password')),
                confirmPassword: String(form.get('confirmPassword')),
            })
            navigate('/', { replace: true })   // Backend kayıttan sonra otomatik giriş yapıyor
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Kayıt yapılamadı')
        } finally {
            setSubmitting(false)
        }
    }

    return (
        <div className="auth-page">
            <div className="card auth-card">
                <h1 className="auth-brand">📦 StokPanel</h1>
                <p className="muted">Yeni hesap oluşturun</p>

                {error && <div className="alert alert-error">{error}</div>}

                <form onSubmit={handleSubmit}>
                    <div className="field">
                        <label htmlFor="fullName">Ad soyad</label>
                        <input id="fullName" name="fullName" autoComplete="name" required />
                    </div>
                    <div className="field">
                        <label htmlFor="email">E-posta</label>
                        <input id="email" name="email" type="email" autoComplete="email" required />
                    </div>
                    <div className="field">
                        <label htmlFor="password">Şifre</label>
                        <input id="password" name="password" type="password" autoComplete="new-password" minLength={8} required />
                        <span className="hint">En az 8 karakter, bir büyük harf, bir küçük harf ve bir rakam</span>
                    </div>
                    <div className="field">
                        <label htmlFor="confirmPassword">Şifre (tekrar)</label>
                        <input id="confirmPassword" name="confirmPassword" type="password" autoComplete="new-password" required />
                    </div>

                    <button className="btn btn-primary btn-block" disabled={submitting}>
                        {submitting ? 'Kaydediliyor…' : 'Kayıt ol'}
                    </button>
                </form>

                <p className="auth-footer">
                    Zaten hesabınız var mı? <Link to="/login">Giriş yapın</Link>
                </p>
            </div>
        </div>
    )
}