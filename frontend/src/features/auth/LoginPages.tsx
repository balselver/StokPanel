import { useRef, useState, type FormEvent } from 'react'
import { Link, Navigate, useLocation, useNavigate } from 'react-router-dom'
import { useAuth } from './useAuth.ts'


const IS_DEMO = import.meta.env.VITE_DEMO_MODE === 'true'
const DEMO_PASSWORD = import.meta.env.VITE_DEMO_PASSWORD ?? ''

const DEMO_ACCOUNTS = [
    { label: 'Admin', email: 'admin@stokpanel.local' },
    { label: 'Personel', email: 'personel@stokpanel.local' },
]
export function LoginPage() {
    const { user, login } = useAuth()
    const navigate = useNavigate()
    const location = useLocation()
    const [error, setError] = useState<string | null>(null)
    const [submitting, setSubmitting] = useState(false)

    const formRef = useRef<HTMLFormElement>(null)

    function fillDemo(email: string) {
        const form = formRef.current
        if (!form) return
            ; (form.elements.namedItem('email') as HTMLInputElement).value = email
            ; (form.elements.namedItem('password') as HTMLInputElement).value = DEMO_PASSWORD
    }

    // Korumalı bir sayfadan geldiyse giriş sonrası oraya dön
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
            <div className="auth-card">
                <div className="auth-logo">📦 StokPanel</div>
                <h1 className="auth-title">Hoş geldiniz!</h1>

                {error && <div className="alert alert-error">{error}</div>}

                {IS_DEMO && (
                    <div className="demo-box">
                        <strong>Demo hesapları</strong>
                        <span className="muted">Bir hesap seçin, bilgiler otomatik dolsun:</span>
                        <div className="demo-buttons">
                            {DEMO_ACCOUNTS.map(a => (
                                <button key={a.email} type="button" className="btn btn-secondary btn-sm" onClick={() => fillDemo(a.email)}>
                                    {a.label} olarak dene
                                </button>
                            ))}
                        </div>
                    </div>
                )}

                <form ref={formRef} className="auth-form" onSubmit={handleSubmit}>
                    <input
                        className="auth-input"
                        name="email"
                        type="email"
                        placeholder="E-posta"
                        aria-label="E-posta"
                        autoComplete="email"
                        required
                    />
                    <input
                        className="auth-input"
                        name="password"
                        type="password"
                        placeholder="Şifre"
                        aria-label="Şifre"
                        autoComplete="current-password"
                        required
                    />

                    <label className="auth-check">
                        <input name="rememberMe" type="checkbox" /> Beni hatırla
                    </label>

                    <button className="auth-button" disabled={submitting}>
                        {submitting ? 'Giriş yapılıyor…' : 'Giriş yap'}
                    </button>
                </form>

                <p className="auth-footer">
                    Hesabınız yok mu?<Link to="/kayit">Kayıt olun</Link>
                </p>
            </div>
        </div>
    )
}