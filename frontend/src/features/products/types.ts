export type MovementType = 'In' | 'Out'

export type Product = {
    id: string
    name: string
    sku: string
    unit: string
    categoryId: string
    categoryName: string
    quantity: number
    criticalLevel: number
    isCritical: boolean
    isActive: boolean
}

export type Movement = {
    id: string
    type: MovementType
    quantity: number
    quantityAfter: number
    note: string | null
    userName: string
    createdAtUtc: string
}

export type ProductFilter = {
    search?: string
    categoryId?: string
    onlyCritical?: boolean
    includeInactive?: boolean
}

export type CreateMovementRequest = {
    type: MovementType
    quantity: number
    note: string | null
}

export type SaveProductRequest = {
    name: string
    sku: string
    unit: string
    categoryId: string
    criticalLevel: number
}