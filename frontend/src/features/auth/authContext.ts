import { createContext } from 'react'
import type { LoginRequest, RegisterRequest, User } from './types.ts'

export type AuthContextValue = {
  user: User | null
  loading: boolean
  isAdmin: boolean
  login: (request: LoginRequest) => Promise<void>
  register: (request: RegisterRequest) => Promise<void>
  logout: () => Promise<void>
}

export const AuthContext = createContext<AuthContextValue | null>(null)