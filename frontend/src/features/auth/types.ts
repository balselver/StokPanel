export type User = {
    id: string
    fullName: string
    email: string
    roles: string[]
}

export type LoginRequest = { email: string; password: string; rememberMe: boolean }
export type RegisterRequest = { fullName: string; email: string; password: string; confirmPassword: string }

// enum yerine "as const" nesne (erasableSyntaxOnly)
export const ROLES = {
    admin: 'Admin',
    personel: 'Personel',
} as const