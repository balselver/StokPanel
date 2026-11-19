// Biçimlendiriciler bir kez oluşturulur, her çağrıda yeniden değil
const quantityFormat = new Intl.NumberFormat('tr-TR', { maximumFractionDigits: 3 })

const dateTimeFormat = new Intl.DateTimeFormat('tr-TR', {
    dateStyle: 'short',
    timeStyle: 'short',
    timeZone: 'Europe/Istanbul',
})

export function formatQuantity(value: number, unit?: string) {
    const text = quantityFormat.format(value)
    return unit ? `${text} ${unit}` : text
}

export function formatDateTime(iso: string) {
    return dateTimeFormat.format(new Date(iso))
}