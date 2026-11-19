import { useEffect, useState, type DependencyList } from 'react'

// Veri yükleme kalıbı: data / error / loading / reload
export function useApiData<T>(load: () => Promise<T>, deps: DependencyList) {
    const [data, setData] = useState<T | null>(null)
    const [error, setError] = useState<string | null>(null)
    const [loading, setLoading] = useState(true)
    const [version, setVersion] = useState(0)

    useEffect(() => {
        let cancelled = false

        load()
            .then(result => {
                if (cancelled) return
                setData(result)
                setError(null)
            })
            .catch(err => {
                if (!cancelled) setError(err instanceof Error ? err.message : 'Veri yüklenemedi')
            })
            .finally(() => {
                if (!cancelled) setLoading(false)
            })

        return () => { cancelled = true }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [...deps, version])

    return { data, error, loading, reload: () => setVersion(v => v + 1) }
}