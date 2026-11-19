import { useState } from 'react'
import { useConfirm } from '../../components/useConfirm.ts'
import { useApiData } from '../../hooks/useApiData.ts'
import { ROLES, type User } from '../auth/types.ts'
import { useAuth } from '../auth/useAuth.ts'
import { usersApi } from './usersApi.ts'

export function UsersTab() {
    const { user: me } = useAuth()
    const confirm = useConfirm()
    const users = useApiData(() => usersApi.list(), [])
    const [message, setMessage] = useState<string | null>(null)
    const [error, setError] = useState<string | null>(null)

    async function changeRole(user: User, role: string) {
        const ok = await confirm({
            title: 'Rol değiştirilsin mi?',
            message: `${user.fullName} kullanıcısının rolü "${role}" olarak değiştirilecek. Yetkileri en geç 1 dakika içinde güncellenir.`,
            confirmText: 'Değiştir',
        })
        if (!ok) return   // select, data'ya bağlı olduğu için eski değerinde kalır

        setMessage(null)
        setError(null)
        try {
            await usersApi.setRole(user.id, role)
            setMessage(`${user.fullName} artık ${role}`)
            users.reload()
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Rol değiştirilemedi')
        }
    }

    return (
        <>
            {message && <div className="alert alert-success">{message}</div>}
            {(error ?? users.error) && <div className="alert alert-error">{error ?? users.error}</div>}

            <div className="card">
                {users.loading ? (
                    <div className="page-loading">Yükleniyor…</div>
                ) : (
                    <div className="table-wrap">
                        <table className="table">
                            <thead>
                                <tr><th>Ad soyad</th><th>E-posta</th><th>Rol</th></tr>
                            </thead>
                            <tbody>
                                {users.data?.map(u => {
                                    const isMe = u.id === me?.id
                                    const role = u.roles[0] ?? ROLES.personel

                                    return (
                                        <tr key={u.id}>
                                            <td data-label="Ad soyad">
                                                <strong>{u.fullName}</strong> {isMe && <span className="muted">(siz)</span>}
                                            </td>
                                            <td data-label="E-posta">{u.email}</td>
                                            <td data-label="Rol">
                                                <select
                                                    aria-label={`${u.fullName} rolü`}
                                                    value={role}
                                                    disabled={isMe}
                                                    title={isMe ? 'Kendi rolünüzü değiştiremezsiniz' : undefined}
                                                    onChange={e => changeRole(u, e.target.value)}
                                                >
                                                    {Object.values(ROLES).map(r => <option key={r} value={r}>{r}</option>)}
                                                </select>
                                            </td>
                                        </tr>
                                    )
                                })}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>
        </>
    )
}