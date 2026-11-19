import { api } from '../../api/client.ts'
import type { Category } from './type.ts'

export const categoriesApi = {
    list: () => api.get<Category[]>('/api/categories'),
}