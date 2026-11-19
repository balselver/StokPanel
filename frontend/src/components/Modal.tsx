import { useEffect, useRef, type ReactNode } from 'react'

type Props = {
    title: string
    onClose: () => void
    wide?: boolean
    children: ReactNode
}

// Bütün diyalogların ortak kabuğu (HTML <dialog>)
export function Modal({ title, onClose, wide = false, children }: Props) {
    const ref = useRef<HTMLDialogElement>(null)

    useEffect(() => {
        const dialog = ref.current
        if (dialog && !dialog.open) dialog.showModal()
    }, [])

    return (
        <dialog ref={ref} className={wide ? 'dialog dialog-wide' : 'dialog'} onClose={onClose}>
            <div className="dialog-body">
                <h2 className="dialog-title">{title}</h2>
                {children}
            </div>
        </dialog>
    )
}