import { api } from '../../api/client.ts'
import type { Category, SaveCategoryRequest } from './types.ts'

export const categoriesApi = {
    list: () => api.get<Category[]>('/api/categories'),
    create: (request: SaveCategoryRequest) => api.post<{ id: string }>('/api/categories', request),
    update: (id: string, request: SaveCategoryRequest) => api.put<void>(`/api/categories/${id}`, request),
    remove: (id: string) => api.delete<void>(`/api/categories/${id}`),
}