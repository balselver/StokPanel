import { useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { useApiData } from '../../hooks/useApiData.ts'
import { formatDateTime, formatQuantity } from '../../utils/format.ts'
import { MovementBadge } from './MovementBadge.tsx'
import { productsApi } from './productsApi.ts'
import { StockMovementDialog } from './StockMovementDialog.tsx'
import type { MovementType } from './types.ts'

export function ProductDetailPage() {
    const { id = '' } = useParams()
    const product = useApiData(() => productsApi.get(id), [id])
    const movements = useApiData(() => productsApi.movements(id), [id])
    const [dialogType, setDialogType] = useState<MovementType | null>(null)

    if (product.loading) return <div className="page-loading">Yükleniyor…</div>

    if (!product.data) {
        return (
            <>
                <Link className="back-link" to="/urunler">← Ürünlere dön</Link>
                <div className="alert alert-error">{product.error}</div>
            </>
        )
    }

    const p = product.data

    return (
        <>
            <Link className="back-link" to="/urunler">← Ürünlere dön</Link>

            <div className="card">
                <div className="page-header">
                    <div>
                        <h1>{p.name}</h1>
                        <span className="sku">{p.sku} · {p.categoryName}</span>
                    </div>
                    <div className="actions">
                        <button className="btn btn-in" disabled={!p.isActive} onClick={() => setDialogType('In')}>+ Giriş</button>
                        <button className="btn btn-out" disabled={p.quantity <= 0} onClick={() => setDialogType('Out')}>− Çıkış</button>
                    </div>
                </div>

                <div className="detail-grid">
                    <div>
                        <div className="detail-label">Mevcut stok</div>
                        <div className={p.isCritical ? 'stock-big qty-out' : 'stock-big'}>{formatQuantity(p.quantity, p.unit)}</div>
                    </div>
                    <div>
                        <div className="detail-label">Kritik seviye</div>
                        <div className="detail-value">{formatQuantity(p.criticalLevel, p.unit)}</div>
                    </div>
                    <div>
                        <div className="detail-label">Durum</div>
                        <div className="detail-value">
                            {!p.isActive
                                ? <span className="badge badge-inactive">Pasif</span>
                                : p.isCritical
                                    ? <span className="badge badge-critical">Kritik seviyede</span>
                                    : <span className="badge badge-in">Yeterli</span>}
                        </div>
                    </div>
                </div>
            </div>

            <section className="card section-gap">
                <h2 className="card-title">Stok hareketleri</h2>

                {movements.error && <div className="alert alert-error">{movements.error}</div>}

                {movements.loading ? (
                    <div className="page-loading">Yükleniyor…</div>
                ) : movements.data?.length === 0 ? (
                    <p className="empty">Bu ürün için henüz hareket yok</p>
                ) : (
                    <div className="table-wrap">
                        <table className="table">
                            <thead>
                                <tr>
                                    <th>Tarih</th>
                                    <th>Tür</th>
                                    <th className="num">Miktar</th>
                                    <th className="num">Sonraki stok</th>
                                    <th>Kullanıcı</th>
                                    <th>Not</th>
                                </tr>
                            </thead>
                            <tbody>
                                {movements.data?.map(m => (
                                    <tr key={m.id}>
                                        <td data-label="Tarih">{formatDateTime(m.createdAtUtc)}</td>
                                        <td data-label="Tür"><MovementBadge type={m.type} /></td>
                                        <td data-label="Miktar" className={`num ${m.type === 'In' ? 'qty-in' : 'qty-out'}`}>
                                            {m.type === 'In' ? '+' : '−'}{formatQuantity(m.quantity, p.unit)}
                                        </td>
                                        <td data-label="Sonraki stok" className="num">{formatQuantity(m.quantityAfter, p.unit)}</td>
                                        <td data-label="Kullanıcı">{m.userName}</td>
                                        <td data-label="Not" className="muted">{m.note ?? '—'}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </section>

            {dialogType && (
                <StockMovementDialog
                    product={p}
                    type={dialogType}
                    onClose={() => setDialogType(null)}
                    onSaved={() => { product.reload(); movements.reload() }}
                />
            )}
        </>
    )
}