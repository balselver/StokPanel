import { api } from '../../api/client.ts'
import type { User } from '../auth/types.ts'

export const usersApi = {
    list: () => api.get<User[]>('/api/users'),
    setRole: (id: string, role: string) => api.put<void>(`/api/users/${id}/role`, { role }),
}