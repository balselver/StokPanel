import { Link } from 'react-router-dom'
import { useApiData } from '../../hooks/useApiData.ts'
import { formatDateTime, formatQuantity } from '../../utils/format.ts'
import { MovementBadge } from '../products/MovementBadge.tsx'
import { dashboardApi } from './dashboardApi.ts'

export function DashboardPage() {
  const { data, error, loading } = useApiData(() => dashboardApi.get(), [])

  if (loading) return <div className="page-loading">Yükleniyor…</div>
  if (!data) return <div className="alert alert-error">{error}</div>

  return (
    <>
      <div className="page-header">
        <h1>Gösterge</h1>
        <Link className="btn btn-primary" to="/urunler">Ürünlere git</Link>
      </div>

      <div className="stats">
        <Stat icon="📦" label="Aktif ürün" value={data.activeProductCount} />
        <Stat icon="⚠️" label="Kritik stok" value={data.criticalCount} tone="warning" />
        <Stat icon="⬇️" label="Bugünkü giriş" value={data.todayInCount} tone="success" />
        <Stat icon="⬆️" label="Bugünkü çıkış" value={data.todayOutCount} tone="danger" />
      </div>

      <div className="grid-2">
        <section className="card">
          <h2 className="card-title">Kritik seviyedeki ürünler</h2>
          {data.criticalProducts.length === 0 ? (
            <p className="empty">Kritik seviyede ürün yok 🎉</p>
          ) : (
            <table className="table">
              <thead>
                <tr><th>Ürün</th><th className="num">Stok</th><th className="num">Kritik</th></tr>
              </thead>
              <tbody>
                {data.criticalProducts.map(p => (
                  <tr key={p.id}>
                    <td data-label="Ürün">
                      <Link className="product-link" to={`/urunler/${p.id}`}>{p.name}</Link>
                      <span className="sku">{p.sku}</span>
                    </td>
                    <td data-label="Stok" className="num">
                      <strong className="qty-out">{formatQuantity(p.quantity, p.unit)}</strong>
                    </td>
                    <td data-label="Kritik" className="num">{formatQuantity(p.criticalLevel, p.unit)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </section>

        <section className="card">
          <h2 className="card-title">Son hareketler</h2>
          {data.recentMovements.length === 0 ? (
            <p className="empty">Henüz stok hareketi yok</p>
          ) : (
            <ul className="activity">
              {data.recentMovements.map(m => (
                <li key={m.id}>
                  <MovementBadge type={m.type} />
                  <div>
                    <Link className="product-link" to={`/urunler/${m.productId}`}>{m.productName}</Link>
                    <div className="meta">{m.userName} · {formatDateTime(m.createdAtUtc)}</div>
                  </div>
                  <span className={`qty ${m.type === 'In' ? 'qty-in' : 'qty-out'}`}>
                    {m.type === 'In' ? '+' : '−'}{formatQuantity(m.quantity, m.unit)}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </section>
      </div>
    </>
  )
}

type StatProps = { icon: string; label: string; value: number; tone?: 'success' | 'warning' | 'danger' }

function Stat({ icon, label, value, tone }: StatProps) {
  return (
    <div className={tone ? `stat stat-${tone}` : 'stat'}>
      <div className="stat-icon">{icon}</div>
      <div>
        <div className="stat-value">{value}</div>
        <div className="stat-label">{label}</div>
      </div>
    </div>
  )
}