import { Image } from 'react-bootstrap'
import { LuUser } from 'react-icons/lu'

import { generateInitials } from '@/helpers/casing'

type CustomerAvatarProps = {
  fullName: string
  avatarUrl?: string | null
  size?: 'sm' | 'md' | 'lg'
  className?: string
}

const SIZE_MAP = {
  sm: { px: 32, className: 'avatar-sm', icon: 14, fs: 'fs-xs' },
  md: { px: 40, className: 'avatar-md', icon: 18, fs: 'fs-sm' },
  lg: { px: 72, className: 'avatar-xl', icon: 28, fs: 'fs-22' },
} as const

const CustomerAvatar = ({
  fullName,
  avatarUrl,
  size = 'md',
  className = '',
}: CustomerAvatarProps) => {
  const config = SIZE_MAP[size]
  const src = avatarUrl?.trim()
  const displayName = fullName.trim() || 'Khách hàng'
  const initials = generateInitials(displayName).slice(0, 2) || '?'

  if (src) {
    return (
      <Image
        src={src}
        alt={displayName}
        width={config.px}
        height={config.px}
        className={`rounded-circle flex-shrink-0 ${config.className} ${className}`}
      />
    )
  }

  if (initials !== '?') {
    return (
      <span
        className={`avatar ${config.className} flex-shrink-0 ${className}`}
        style={{ width: config.px, height: config.px }}>
        <span
          className={`avatar-title text-bg-primary fw-semibold rounded-circle ${config.fs} d-flex align-items-center justify-content-center`}>
          {initials}
        </span>
      </span>
    )
  }

  return (
    <span
      className={`avatar ${config.className} flex-shrink-0 ${className}`}
      style={{ width: config.px, height: config.px }}>
      <span
        className={`avatar-title bg-light text-muted rounded-circle d-flex align-items-center justify-content-center`}>
        <LuUser size={config.icon} />
      </span>
    </span>
  )
}

export default CustomerAvatar
