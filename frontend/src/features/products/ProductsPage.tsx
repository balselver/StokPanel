import { useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { useApiData } from '../../hooks/useApiData.ts'
import { useDebouncedValue } from '../../hooks/useDebouncedValue.ts'
import { formatQuantity } from '../../utils/format.ts'
import { useAuth } from '../auth/useAuth.ts'
import { categoriesApi } from '../categories/categoriesApi.ts'
import { productsApi } from './productsApi.ts'
import { StockMovementDialog } from './StockMovementDialog.tsx'
import type { Movement, MovementType, Product } from './types.ts'

export function ProductsPage() {
    const { isAdmin } = useAuth()

    // Filtreler adres çubuğunda: sayfa yenilense de, link paylaşılsa da kaybolmaz
    const [params, setParams] = useSearchParams()
    const search = params.get('q') ?? ''
    const categoryId = params.get('kategori') ?? ''
    const onlyCritical = params.get('kritik') === '1'
    const includeInactive = params.get('pasif') === '1'

    const debouncedSearch = useDebouncedValue(search.trim(), 400)

    const products = useApiData(
        () => productsApi.list({ search: debouncedSearch, categoryId, onlyCritical, includeInactive }),
        [debouncedSearch, categoryId, onlyCritical, includeInactive],
    )
    const categories = useApiData(() => categoriesApi.list(), [])

    const [movement, setMovement] = useState<{ product: Product; type: MovementType } | null>(null)
    const [message, setMessage] = useState<string | null>(null)

    function setParam(key: string, value: string) {
        setParams(prev => {
            const next = new URLSearchParams(prev)
            if (value) next.set(key, value)
            else next.delete(key)
            return next
        }, { replace: true })   // Her harf tarayıcı geçmişine eklenmesin
    }

    function handleSaved(product: Product, saved: Movement) {
        const action = saved.type === 'In' ? 'giriş' : 'çıkış'
        setMessage(
            `${product.name}: ${formatQuantity(saved.quantity, product.unit)} ${action} yapıldı. ` +
            `Yeni stok: ${formatQuantity(saved.quantityAfter, product.unit)}`,
        )
        products.reload()
    }

    return (
        <>
            <div className="page-header">
                <h1>Ürünler</h1>
            </div>

            {message && <div className="alert alert-success">{message}</div>}

            <div className="card">
                <div className="toolbar">
                    <input
                        className="search"
                        type="search"
                        placeholder="Ürün adı veya stok kodu ara…"
                        aria-label="Ara"
                        value={search}
                        onChange={e => setParam('q', e.target.value)}
                    />
                    <select aria-label="Kategori" value={categoryId} onChange={e => setParam('kategori', e.target.value)}>
                        <option value="">Tüm kategoriler</option>
                        {categories.data?.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
                    </select>
                    <label className="checkbox-inline">
                        <input type="checkbox" checked={onlyCritical} onChange={e => setParam('kritik', e.target.checked ? '1' : '')} />
                        Sadece kritik
                    </label>
                    {isAdmin && (
                        <label className="checkbox-inline">
                            <input type="checkbox" checked={includeInactive} onChange={e => setParam('pasif', e.target.checked ? '1' : '')} />
                            Pasifleri göster
                        </label>
                    )}
                </div>

                {products.error && <div className="alert alert-error">{products.error}</div>}

                {products.loading ? (
                    <div className="page-loading">Yükleniyor…</div>
                ) : products.data?.length === 0 ? (
                    <p className="empty">Aramanıza uygun ürün bulunamadı</p>
                ) : (
                    <div className="table-wrap">
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Ürün</th>
                                    <th>Kategori</th>
                                    <th className="num">Stok</th>
                                    <th className="actions"></th>
                                </tr>
                            </thead>
                            <tbody>
                                {products.data?.map(p => (
                                    <tr key={p.id}>
                                        <td data-label="Ürün">
                                            <Link className="product-link" to={`/urunler/${p.id}`}>{p.name}</Link>
                                            <span className="sku">{p.sku}</span>
                                        </td>
                                        <td data-label="Kategori">{p.categoryName}</td>
                                        <td data-label="Stok" className="num">
                                            {formatQuantity(p.quantity, p.unit)}{' '}
                                            {!p.isActive
                                                ? <span className="badge badge-inactive">Pasif</span>
                                                : p.isCritical && <span className="badge badge-critical">Kritik</span>}
                                        </td>
                                        <td className="actions">
                                            <button className="btn btn-sm btn-in" disabled={!p.isActive}
                                                onClick={() => { setMessage(null); setMovement({ product: p, type: 'In' }) }}>
                                                + Giriş
                                            </button>
                                            <button className="btn btn-sm btn-out" disabled={p.quantity <= 0}
                                                onClick={() => { setMessage(null); setMovement({ product: p, type: 'Out' }) }}>
                                                − Çıkış
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </div>

            {movement && (
                <StockMovementDialog
                    product={movement.product}
                    type={movement.type}
                    onClose={() => setMovement(null)}
                    onSaved={saved => handleSaved(movement.product, saved)}
                />
            )}
        </>
    )
}