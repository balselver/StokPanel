import type { MovementType } from './types.ts'

export function MovementBadge({ type }: { type: MovementType }) {
  return type === 'In' ? <span className="badge badge-in">Giriş</span> : <span className="badge badge-out">Çıkış</span>
}