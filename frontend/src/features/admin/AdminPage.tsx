import { useSearchParams } from 'react-router-dom'
import { CategoriesTab } from './CategoriesTab.tsx'
import { ProductsTab } from './ProductsTab.tsx'
import { UsersTab } from './UsersTab.tsx'

const TABS = [
    { key: 'urunler', label: 'Ürünler' },
    { key: 'kategoriler', label: 'Kategoriler' },
    { key: 'kullanicilar', label: 'Kullanıcılar' },
] as const

export function AdminPage() {
    const [params, setParams] = useSearchParams()
    const active = params.get('sekme') ?? 'urunler'

    return (
        <>
            <div className="page-header">
                <h1>Yönetim</h1>
            </div>

            <div className="tabs" role="tablist">
                {TABS.map(tab => (
                    <button
                        key={tab.key}
                        role="tab"
                        aria-selected={active === tab.key}
                        className={active === tab.key ? 'tab active' : 'tab'}
                        onClick={() => setParams({ sekme: tab.key }, { replace: true })}
                    >
                        {tab.label}
                    </button>
                ))}
            </div>

            {active === 'urunler' && <ProductsTab />}
            {active === 'kategoriler' && <CategoriesTab />}
            {active === 'kullanicilar' && <UsersTab />}
        </>
    )
}