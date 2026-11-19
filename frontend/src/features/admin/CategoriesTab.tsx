import { useState } from 'react'
import { useConfirm } from '../../components/useConfirm.ts'
import { useApiData } from '../../hooks/useApiData.ts'
import { categoriesApi } from '../categories/categoriesApi.ts'
import type { Category } from '../categories/types.ts'
import { CategoryFormDialog } from './CategoryDormDialog.tsx'

export function CategoriesTab() {
    const confirm = useConfirm()
    const categories = useApiData(() => categoriesApi.list(), [])
    const [editing, setEditing] = useState<Category | 'new' | null>(null)
    const [message, setMessage] = useState<string | null>(null)
    const [error, setError] = useState<string | null>(null)

    async function handleDelete(category: Category) {
        const ok = await confirm({
            title: 'Kategori silinsin mi?',
            message: `"${category.name}" kalıcı olarak silinecek. Bu işlem geri alınamaz.`,
            confirmText: 'Sil',
            danger: true,
        })
        if (!ok) return

        setMessage(null)
        setError(null)
        try {
            await categoriesApi.remove(category.id)
            setMessage(`"${category.name}" silindi`)
            categories.reload()
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Silinemedi')
        }
    }

    return (
        <>
            {message && <div className="alert alert-success">{message}</div>}
            {(error ?? categories.error) && <div className="alert alert-error">{error ?? categories.error}</div>}

            <div className="card">
                <div className="toolbar">
                    <span className="muted">{categories.data?.length ?? 0} kategori</span>
                    <button className="btn btn-primary toolbar-end" onClick={() => { setMessage(null); setEditing('new') }}>
                        + Yeni kategori
                    </button>
                </div>

                {categories.loading ? (
                    <div className="page-loading">Yükleniyor…</div>
                ) : categories.data?.length === 0 ? (
                    <p className="empty">Henüz kategori yok. İlk kategoriyi ekleyerek başlayın.</p>
                ) : (
                    <div className="table-wrap">
                        <table className="table">
                            <thead>
                                <tr><th>Ad</th><th>Açıklama</th><th className="num">Ürün</th><th className="actions"></th></tr>
                            </thead>
                            <tbody>
                                {categories.data?.map(c => (
                                    <tr key={c.id}>
                                        <td data-label="Ad"><strong>{c.name}</strong></td>
                                        <td data-label="Açıklama" className="muted">{c.description ?? '—'}</td>
                                        <td data-label="Ürün" className="num">{c.productCount}</td>
                                        <td className="actions">
                                            <button className="btn btn-sm btn-secondary" onClick={() => { setMessage(null); setEditing(c) }}>
                                                Düzenle
                                            </button>
                                            <button
                                                className="btn btn-sm btn-out"
                                                disabled={c.productCount > 0}
                                                title={c.productCount > 0 ? 'İçinde ürün olan kategori silinemez' : undefined}
                                                onClick={() => handleDelete(c)}
                                            >
                                                Sil
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>

            {editing && (
                <CategoryFormDialog
                    category={editing === 'new' ? null : editing}
                    onClose={() => setEditing(null)}
                    onSaved={msg => { setMessage(msg); categories.reload() }}
                />
            )}
        </>
    )
}