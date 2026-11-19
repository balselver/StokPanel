import { createContext } from 'react'

export type ConfirmOptions = {
    title: string
    message: string
    confirmText?: string
    danger?: boolean
}

export type ConfirmFn = (options: ConfirmOptions) => Promise<boolean>

export const ConfirmContext = createContext<ConfirmFn | null>(null)