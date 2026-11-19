import { useEffect, useState } from 'react'

// Değer değiştikten "delay" ms sonra güncellenir (her harfte istek gitmesin)
export function useDebouncedValue<T>(value: T, delay = 400) {
    const [debounced, setDebounced] = useState(value)

    useEffect(() => {
        const timer = setTimeout(() => setDebounced(value), delay)
        return () => clearTimeout(timer)
    }, [value, delay])

    return debounced
}