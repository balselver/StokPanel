// Tüm istekler buradan geçer: cookie, JSON, hata mesajı, oturum düşmesi

export class ApiError extends Error {
    status: number

    constructor(status: number, message: string) {
        super(message)
        this.name = 'ApiError'
        this.status = status
    }
}

// Oturum düşünce (401) ne yapılacağını AuthProvider söyler
let onUnauthorized: (() => void) | null = null

export function setUnauthorizedHandler(handler: (() => void) | null) {
    onUnauthorized = handler
}

async function request<T>(method: string, url: string, body?: unknown): Promise<T> {
    const response = await fetch(url, {
        method,
        credentials: 'include',
        headers: body !== undefined ? { 'Content-Type': 'application/json' } : undefined,
        body: body !== undefined ? JSON.stringify(body) : undefined,
    })

    if (!response.ok) {
        // /api/auth/* hariç: yanlış şifre de 401 döner, o "oturum düştü" demek değil
        if (response.status === 401 && !url.startsWith('/api/auth/')) onUnauthorized?.()
        throw new ApiError(response.status, await readError(response))
    }

    // 204 ve boş gövdeli cevaplarda JSON okumaya çalışma
    const text = await response.text()
    return (text ? JSON.parse(text) : undefined) as T
}

async function readError(response: Response): Promise<string> {
    try {
        const data = await response.json()
        if (typeof data?.error === 'string') return data.error
    } catch {
        // Gövde JSON değil, aşağıdaki varsayılan mesajlara düş
    }

    if (response.status === 401) return 'Oturumunuz sona erdi, lütfen tekrar giriş yapın'
    if (response.status === 403) return 'Bu işlem için yetkiniz yok'
    if (response.status === 404) return 'Kayıt bulunamadı'
    return 'Beklenmeyen bir hata oluştu, lütfen tekrar deneyin'
}

export const api = {
    get: <T>(url: string) => request<T>('GET', url),
    post: <T>(url: string, body?: unknown) => request<T>('POST', url, body),
    put: <T>(url: string, body?: unknown) => request<T>('PUT', url, body),
    patch: <T>(url: string, body?: unknown) => request<T>('PATCH', url, body),
    delete: <T>(url: string) => request<T>('DELETE', url),
}