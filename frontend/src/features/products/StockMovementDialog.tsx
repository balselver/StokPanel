import { useEffect, useRef, useState, type FormEvent } from 'react'
import { formatQuantity } from '../../utils/format.ts'
import { productsApi } from './productsApi.ts'
import type { Movement, MovementType, Product } from './types.ts'

type Props = {
    product: Product
    type: MovementType
    onClose: () => void
    onSaved: (movement: Movement) => void
}

export function StockMovementDialog({ product, type, onClose, onSaved }: Props) {
    const dialogRef = useRef<HTMLDialogElement>(null)
    const [error, setError] = useState<string | null>(null)
    const [submitting, setSubmitting] = useState(false)
    const isIn = type === 'In'

    // StrictMode effect'i iki kez çalıştırır; açık diyaloğu tekrar açmak hata verir
    useEffect(() => {
        const dialog = dialogRef.current
        if (dialog && !dialog.open) dialog.showModal()
    }, [])

    async function handleSubmit(e: FormEvent<HTMLFormElement>) {
        e.preventDefault()
        const form = new FormData(e.currentTarget)
        const note = String(form.get('note') ?? '').trim()

        setError(null)
        setSubmitting(true)
        try {
            const saved = await productsApi.createMovement(product.id, {
                type,
                quantity: Number(form.get('quantity')),
                note: note || null,
            })
            onSaved(saved)
            dialogRef.current?.close()   // close → onClose → üst bileşen diyaloğu kaldırır
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Kaydedilemedi')
        } finally {
            setSubmitting(false)
        }
    }

    return (
        <dialog ref={dialogRef} className="dialog" onClose={onClose}>
            <form className="dialog-body" onSubmit={handleSubmit}>
                <h2 className="dialog-title">{isIn ? 'Stok girişi' : 'Stok çıkışı'}</h2>
                <p className="muted">
                    {product.name} · Mevcut stok: <strong>{formatQuantity(product.quantity, product.unit)}</strong>
                </p>

                {error && <div className="alert alert-error">{error}</div>}

                <div className="field">
                    <label htmlFor="quantity">Miktar ({product.unit})</label>
                    <input id="quantity" name="quantity" type="number" min="0.001" step="0.001" required autoFocus />
                </div>

                <div className="field">
                    <label htmlFor="note">Not (isteğe bağlı)</label>
                    <textarea
                        id="note"
                        name="note"
                        maxLength={500}
                        placeholder={isIn ? 'Örn. Tedarikçiden geldi' : 'Örn. Satış, fire, iade'}
                    />
                </div>

                <div className="dialog-actions">
                    <button type="button" className="btn btn-secondary" onClick={() => dialogRef.current?.close()}>
                        Vazgeç
                    </button>
                    <button className={isIn ? 'btn btn-primary' : 'btn btn-danger'} disabled={submitting}>
                        {submitting ? 'Kaydediliyor…' : isIn ? 'Girişi kaydet' : 'Çıkışı kaydet'}
                    </button>
                </div>
            </form>
        </dialog>
    )
}