import { useCallback, useState, type ReactNode } from 'react'
import { ConfirmContext, type ConfirmOptions } from './confirmContext.ts'
import { Modal } from './Modal.tsx'

type Pending = {
    options: ConfirmOptions
    resolve: (result: boolean) => void
}

export function ConfirmProvider({ children }: { children: ReactNode }) {
    const [pending, setPending] = useState<Pending | null>(null)

    // confirm() bitmemiş bir Promise döner; kullanıcı butona basınca resolve edilir
    const confirm = useCallback(
        (options: ConfirmOptions) => new Promise<boolean>(resolve => setPending({ options, resolve })),
        [],
    )

    function finish(result: boolean) {
        pending?.resolve(result)
        setPending(null)
    }

    return (
        <ConfirmContext value={confirm}>
            {children}

            {pending && (
                <Modal title={pending.options.title} onClose={() => finish(false)}>
                    <p className="confirm-message">{pending.options.message}</p>
                    <div className="dialog-actions">
                        {/* Odak "Vazgeç"te: yanlışlıkla Enter'a basılırsa tehlikeli işlem yapılmaz */}
                        <button type="button" className="btn btn-secondary" onClick={() => finish(false)} autoFocus>
                            Vazgeç
                        </button>
                        <button
                            type="button"
                            className={pending.options.danger ? 'btn btn-danger' : 'btn btn-primary'}
                            onClick={() => finish(true)}
                        >
                            {pending.options.confirmText ?? 'Onayla'}
                        </button>
                    </div>
                </Modal>
            )}
        </ConfirmContext>
    )
}