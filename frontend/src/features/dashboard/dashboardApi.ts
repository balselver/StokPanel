import { api } from '../../api/client.ts'
import type { Dashboard } from './types.ts'

export const dashboardApi = {
    get: () => api.get<Dashboard>('/api/dashboard'),
}