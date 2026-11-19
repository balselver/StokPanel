import { useEffect, useState, type ReactNode } from 'react'
import { setUnauthorizedHandler } from '../../api/client.ts'
import { authApi } from './authApi.ts'
import { AuthContext } from './authContext.ts'
import { ROLES, type LoginRequest, type RegisterRequest, type User } from './types.ts'

export function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(null)
    const [loading, setLoading] = useState(true)

    // Açılışta: cookie hâlâ geçerli mi?
    useEffect(() => {
        let cancelled = false

        authApi.me()
            .then(u => { if (!cancelled) setUser(u) })
            .catch(() => { if (!cancelled) setUser(null) })
            .finally(() => { if (!cancelled) setLoading(false) })

        return () => { cancelled = true }
    }, [])

    // Herhangi bir istekte 401 gelirse oturumu kapat
    useEffect(() => {
        setUnauthorizedHandler(() => setUser(null))
        return () => setUnauthorizedHandler(null)
    }, [])

    const value = {
        user,
        loading,
        isAdmin: user?.roles.includes(ROLES.admin) ?? false,
        login: async (request: LoginRequest) => setUser(await authApi.login(request)),
        register: async (request: RegisterRequest) => setUser(await authApi.register(request)),
        logout: async () => {
            await authApi.logout()
            setUser(null)
        },
    }

    // React 19: Context doğrudan provider olarak kullanılabilir (.Provider gerekmez)
    return <AuthContext value={value}>{children}</AuthContext>
}