import type { MovementType, Product } from '../products/types.ts'

export type RecentMovement = {
    id: string
    productId: string
    productName: string
    unit: string
    type: MovementType
    quantity: number
    userName: string
    createdAtUtc: string
}

export type Dashboard = {
    activeProductCount: number
    criticalCount: number
    todayInCount: number
    todayOutCount: number
    criticalProducts: Product[]
    recentMovements: RecentMovement[]
}