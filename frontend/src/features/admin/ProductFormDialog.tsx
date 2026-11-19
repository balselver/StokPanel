import { useState, type FormEvent } from 'react'
import { Modal } from '../../components/Modal.tsx'
import type { Category } from '../categories/types.ts'
import { productsApi } from '../products/productsApi.ts'
import type { Product } from '../products/types.ts'

type Props = {
    product: Product | null   // null → yeni ürün
    categories: Category[]
    onClose: () => void
    onSaved: (message: string) => void
}

const UNIT_SUGGESTIONS = ['adet', 'kg', 'gr', 'lt', 'ml', 'metre', 'paket', 'kutu', 'koli']

export function ProductFormDialog({ product, categories, onClose, onSaved }: Props) {
    const [error, setError] = useState<string | null>(null)
    const [submitting, setSubmitting] = useState(false)

    async function handleSubmit(e: FormEvent<HTMLFormElement>) {
        e.preventDefault()
        const form = new FormData(e.currentTarget)
        const request = {
            name: String(form.get('name')),
            sku: String(form.get('sku')),
            unit: String(form.get('unit')),
            categoryId: String(form.get('categoryId')),
            criticalLevel: Number(form.get('criticalLevel')),
        }

        setError(null)
        setSubmitting(true)
        try {
            if (product) await productsApi.update(product.id, request)
            else await productsApi.create(request)

            onSaved(product ? `"${request.name}" güncellendi` : `"${request.name}" eklendi`)
            onClose()
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Kaydedilemedi')
        } finally {
            setSubmitting(false)
        }
    }

    return (
        <Modal title={product ? 'Ürünü düzenle' : 'Yeni ürün'} onClose={onClose} wide>
            {error && <div className="alert alert-error">{error}</div>}

            {categories.length === 0 ? (
                <>
                    <p className="confirm-message">Ürün eklemeden önce en az bir kategori oluşturmalısınız.</p>
                    <div className="dialog-actions">
                        <button type="button" className="btn btn-secondary" onClick={onClose}>Kapat</button>
                    </div>
                </>
            ) : (
                <form onSubmit={handleSubmit}>
                    <div className="form-grid">
                        <div className="field full">
                            <label htmlFor="name">Ürün adı</label>
                            <input id="name" name="name" defaultValue={product?.name} maxLength={150} required autoFocus />
                        </div>
                        <div className="field">
                            <label htmlFor="sku">Stok kodu (SKU)</label>
                            <input id="sku" name="sku" defaultValue={product?.sku} maxLength={50} placeholder="Örn. KLM-001" required />
                        </div>
                        <div className="field">
                            <label htmlFor="unit">Birim</label>
                            <input id="unit" name="unit" list="unit-options" defaultValue={product?.unit ?? 'adet'} maxLength={20} required />
                            <datalist id="unit-options">
                                {UNIT_SUGGESTIONS.map(u => <option key={u} value={u} />)}
                            </datalist>
                        </div>
                        <div className="field">
                            <label htmlFor="categoryId">Kategori</label>
                            <select id="categoryId" name="categoryId" defaultValue={product?.categoryId ?? ''} required>
                                <option value="" disabled>Seçin…</option>
                                {categories.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                            </select>
                        </div>
                        <div className="field">
                            <label htmlFor="criticalLevel">Kritik seviye</label>
                            <input id="criticalLevel" name="criticalLevel" type="number" min="0" step="0.001"
                                defaultValue={product?.criticalLevel ?? 0} required />
                        </div>
                    </div>

                    {!product && (
                        <p className="hint">Yeni ürün 0 stokla oluşturulur. İlk girişi Ürünler sayfasından "+ Giriş" ile yapın.</p>
                    )}

                    <div className="dialog-actions">
                        <button type="button" className="btn btn-secondary" onClick={onClose}>Vazgeç</button>
                        <button className="btn btn-primary" disabled={submitting}>{submitting ? 'Kaydediliyor…' : 'Kaydet'}</button>
                    </div>
                </form>
            )}
        </Modal>
    )
}