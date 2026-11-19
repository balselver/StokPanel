import { api } from '../../api/client.ts'
import type { CreateMovementRequest, Movement, Product, ProductFilter } from './types.ts'

export const productsApi = {
    list: (filter: ProductFilter) => {
        const params = new URLSearchParams()
        if (filter.search) params.set('search', filter.search)
        if (filter.categoryId) params.set('categoryId', filter.categoryId)
        if (filter.onlyCritical) params.set('onlyCritical', 'true')
        if (filter.includeInactive) params.set('includeInactive', 'true')

        const query = params.toString()
        return api.get<Product[]>(`/api/products${query ? `?${query}` : ''}`)
    },

    get: (id: string) => api.get<Product>(`/api/products/${id}`),

    movements: (id: string, take = 50) => api.get<Movement[]>(`/api/products/${id}/movements?take=${take}`),

    createMovement: (id: string, request: CreateMovementRequest) =>
        api.post<Movement>(`/api/products/${id}/movements`, request),
}