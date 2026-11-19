import { useState } from 'react'
import { useConfirm } from '../../components/useConfirm.ts'
import { useApiData } from '../../hooks/useApiData.ts'
import { useDebouncedValue } from '../../hooks/useDebouncedValue.ts'
import { formatQuantity } from '../../utils/format.ts'
import { categoriesApi } from '../categories/categoriesApi.ts'
import { productsApi } from '../products/productsApi.ts'
import type { Product } from '../products/types.ts'
import { ProductFormDialog } from './ProductFormDialog.tsx'

export function ProductsTab() {
    const confirm = useConfirm()
    const [search, setSearch] = useState('')
    const debouncedSearch = useDebouncedValue(search.trim(), 400)

    const products = useApiData(
        () => productsApi.list({ search: debouncedSearch, includeInactive: true }),
        [debouncedSearch],
    )
    const categories = useApiData(() => categoriesApi.list(), [])

    const [editing, setEditing] = useState<Product | 'new' | null>(null)
    const [message, setMessage] = useState<string | null>(null)
    const [error, setError] = useState<string | null>(null)

    async function toggleActive(product: Product) {
        if (product.isActive) {
            const ok = await confirm({
                title: 'Ürün pasife alınsın mı?',
                message: `"${product.name}" ürün listesinde görünmeyecek ve stok girişi yapılamayacak. Geçmiş hareketleri korunur, istediğiniz zaman tekrar aktif edebilirsiniz.`,
                confirmText: 'Pasife al',
                danger: true,
            })
            if (!ok) return
        }

        setMessage(null)
        setError(null)
        try {
            await productsApi.setActive(product.id, !product.isActive)
            setMessage(`"${product.name}" ${product.isActive ? 'pasife alındı' : 'aktif edildi'}`)
            products.reload()
        } catch (err) {
            setError(err instanceof Error ? err.message : 'İşlem yapılamadı')
        }
    }

    return (
        <>
            {message && <div className="alert alert-success">{message}</div>}
            {(error ?? products.error) && <div className="alert alert-error">{error ?? products.error}</div>}

            <div className="card">
                <div className="toolbar">
                    <input
                        className="search"
                        type="search"
                        placeholder="Ürün adı veya stok kodu ara…"
                        aria-label="Ara"
                        value={search}
                        onChange={e => setSearch(e.target.value)}
                    />
                    <button className="btn btn-primary" onClick={() => { setMessage(null); setEditing('new') }}>
                        + Yeni ürün
                    </button>
                </div>

                {products.loading ? (
                    <div className="page-loading">Yükleniyor…</div>
                ) : products.data?.length === 0 ? (
                    <p className="empty">Ürün bulunamadı</p>
                ) : (
                    <div className="table-wrap">
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Ürün</th>
                                    <th>Kategori</th>
                                    <th className="num">Stok</th>
                                    <th className="num">Kritik</th>
                                    <th>Durum</th>
                                    <th className="actions"></th>
                                </tr>
                            </thead>
                            <tbody>
                                {products.data?.map(p => (
                                    <tr key={p.id}>
                                        <td data-label="Ürün">
                                            <strong>{p.name}</strong>
                                            <span className="sku">{p.sku}</span>
                                        </td>
                                        <td data-label="Kategori">{p.categoryName}</td>
                                        <td data-label="Stok" className="num">{formatQuantity(p.quantity, p.unit)}</td>
                                        <td data-label="Kritik" className="num">{formatQuantity(p.criticalLevel, p.unit)}</td>
                                        <td data-label="Durum">
                                            {p.isActive
                                                ? <span className="badge badge-in">Aktif</span>
                                                : <span className="badge badge-inactive">Pasif</span>}
                                        </td>
                                        <td className="actions">
                                            <button className="btn btn-sm btn-secondary" onClick={() => { setMessage(null); setEditing(p) }}>
                                                Düzenle
                                            </button>
                                            <button className={p.isActive ? 'btn btn-sm btn-out' : 'btn btn-sm btn-in'} onClick={() => toggleActive(p)}>
                                                {p.isActive ? 'Pasife al' : 'Aktif et'}
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
                <ProductFormDialog
                    product={editing === 'new' ? null : editing}
                    categories={categories.data ?? []}
                    onClose={() => setEditing(null)}
                    onSaved={msg => { setMessage(msg); products.reload() }}
                />
            )}
        </>
    )
}