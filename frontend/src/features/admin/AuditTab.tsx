import { useState } from 'react'
import { useApiData } from '../../hooks/useApiData.ts'
import { formatDateTime } from '../../utils/format.ts'
import { auditApi } from './auditApi.ts'

const ENTITY_TYPES = ['Ürün', 'Kategori', 'Kullanıcı']

function actionBadgeClass(action: string) {
    if (action === 'Eklendi' || action === 'Aktif edildi') return 'badge badge-in'
    if (action === 'Silindi' || action === 'Pasife alındı') return 'badge badge-out'
    return 'badge'
}

export function AuditTab() {
    const [entityType, setEntityType] = useState('')
    const [page, setPage] = useState(1)
    const logs = useApiData(() => auditApi.list(entityType, page), [entityType, page])

    return (
        <>
            {logs.error && <div className="alert alert-error">{logs.error}</div>}

            <div className="card">
                <div className="toolbar">
                    <select
                        aria-label="Kayıt türü"
                        value={entityType}
                        onChange={e => { setEntityType(e.target.value); setPage(1) }}
                    >
                        <option value="">Tüm kayıtlar</option>
                        {ENTITY_TYPES.map(t => <option key={t} value={t}>{t}</option>)}
                    </select>
                    <span className="muted toolbar-end">{logs.data?.totalCount ?? 0} işlem</span>
                </div>

                {logs.loading ? (
                    <div className="page-loading">Yükleniyor…</div>
                ) : logs.data?.items.length === 0 ? (
                    <p className="empty">Henüz kayıtlı işlem yok</p>
                ) : (
                    <>
                        <div className="table-wrap">
                            <table className="table">
                                <thead>
                                    <tr><th>Tarih</th><th>Tür</th><th>İşlem</th><th>Açıklama</th><th>Kullanıcı</th></tr>
                                </thead>
                                <tbody>
                                    {logs.data?.items.map(l => (
                                        <tr key={l.id}>
                                            <td data-label="Tarih" className="nowrap">{formatDateTime(l.createdAtUtc)}</td>
                                            <td data-label="Tür">{l.entityType}</td>
                                            <td data-label="İşlem"><span className={actionBadgeClass(l.action)}>{l.action}</span></td>
                                            <td data-label="Açıklama">{l.description}</td>
                                            <td data-label="Kullanıcı">{l.userName}</td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>

                        {logs.data && logs.data.totalPages > 1 && (
                            <div className="pager">
                                <button className="btn btn-secondary btn-sm" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>
                                    ← Önceki
                                </button>
                                <span className="muted">Sayfa {logs.data.page} / {logs.data.totalPages}</span>
                                <button className="btn btn-secondary btn-sm" disabled={page >= logs.data.totalPages} onClick={() => setPage(p => p + 1)}>
                                    Sonraki →
                                </button>
                            </div>
                        )}
                    </>
                )}
            </div>
        </>
    )
}