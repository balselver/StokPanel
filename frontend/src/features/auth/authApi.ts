import { api } from '../../api/client.ts'
import type { LoginRequest, RegisterRequest, User } from './types.ts'

export const authApi = {
    me: () => api.get<User>('/api/auth/me'),
    login: (request: LoginRequest) => api.post<User>('/api/auth/login', request),
    register: (request: RegisterRequest) => api.post<User>('/api/auth/register', request),
    logout: () => api.post<void>('/api/auth/logout'),
}