import { api } from '../../api/client.ts'

export type AuditLog = {
    id: string
    entityType: string
    action: string
    description: string
    userName: string
    createdAtUtc: string
}

export type AuditLogPage = {
    items: AuditLog[]
    page: number
    totalPages: number
    totalCount: number
}

export const auditApi = {
    list: (entityType: string, page: number) => {
        const params = new URLSearchParams({ page: String(page) })
        if (entityType) params.set('entityType', entityType)
        return api.get<AuditLogPage>(`/api/audit-logs?${params}`)
    },
}