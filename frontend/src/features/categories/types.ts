export type Category = {
    id: string
    name: string
    description: string | null
    productCount: number
}

export type SaveCategoryRequest = {
    name: string
    description: string | null
}