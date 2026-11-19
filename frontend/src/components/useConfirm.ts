import { useContext } from 'react'
import { ConfirmContext } from './confirmContext.ts'

export function useConfirm() {
    const confirm = useContext(ConfirmContext)
    if (!confirm) throw new Error('useConfirm, ConfirmProvider içinde kullanılmalı')
    return confirm
}