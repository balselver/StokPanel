import { useState, type FormEvent } from 'react'
import { Modal } from '../../components/Modal.tsx'
import { categoriesApi } from '../categories/categoriesApi.ts'
import type { Category } from '../categories/types.ts'

type Props = {
    category: Category | null   // null → yeni kategori
    onClose: () => void
    onSaved: (message: string) => void
}

export function CategoryFormDialog({ category, onClose, onSaved }: Props) {
    const [error, setError] = useState<string | null>(null)
    const [submitting, setSubmitting] = useState(false)

    async function handleSubmit(e: FormEvent<HTMLFormElement>) {
        e.preventDefault()
        const form = new FormData(e.currentTarget)
        const description = String(form.get('description') ?? '').trim()
        const request = { name: String(form.get('name')), description: description || null }

        setError(null)
        setSubmitting(true)
        try {
            if (category) await categoriesApi.update(category.id, request)
            else await categoriesApi.create(request)

            onSaved(category ? `"${request.name}" güncellendi` : `"${request.name}" eklendi`)
            onClose()
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Kaydedilemedi')
        } finally {
            setSubmitting(false)
        }
    }

    return (
        <Modal title={category ? 'Kategoriyi düzenle' : 'Yeni kategori'} onClose={onClose}>
            {error && <div className="alert alert-error">{error}</div>}

            <form onSubmit={handleSubmit}>
                <div className="field">
                    <label htmlFor="name">Ad</label>
                    <input id="name" name="name" defaultValue={category?.name} maxLength={100} required autoFocus />
                </div>
                <div className="field">
                    <label htmlFor="description">Açıklama (isteğe bağlı)</label>
                    <textarea id="description" name="description" defaultValue={category?.description ?? ''} maxLength={500} />
                </div>

                <div className="dialog-actions">
                    <button type="button" className="btn btn-secondary" onClick={onClose}>Vazgeç</button>
                    <button className="btn btn-primary" disabled={submitting}>{submitting ? 'Kaydediliyor…' : 'Kaydet'}</button>
                </div>
            </form>
        </Modal>
    )
}