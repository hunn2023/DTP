import { Card, Form } from 'react-bootstrap'
import { LuBuilding2, LuMapPin } from 'react-icons/lu'

import type { ProductPriceRow } from '@/features/master-data/products/types'
import { getEsimPackageCardImage } from '@/features/products/esim-packages/esimPackageCardImages'
import type { EsimPackage } from '@/features/products/esim-packages/types'
import './esim-packages-grid.scss'

type EsimPackageCardProps = {
  pkg: EsimPackage
  price?: ProductPriceRow | null
  onClick: () => void
  onToggleActive: (pkg: EsimPackage) => void
}

function formatMoney(value: number, currency: string): string {
  return `${value.toLocaleString('vi-VN')} ${currency}`
}

function formatData(pkg: EsimPackage): string {
  if (pkg.isUnlimited) return 'Unlimited'
  if (pkg.dataAmount == null) return '—'
  return `${pkg.dataAmount}${pkg.dataUnit}`
}

function formatValidity(days: number): string {
  return `${days} Ngày`
}

function getCarrierLabel(pkg: EsimPackage): string {
  if (pkg.carriers.length === 0) return pkg.providerName || '—'
  return pkg.carriers.map((c) => c.carrierName).join(', ')
}

const EsimPackageCard = ({ pkg, price, onClick, onToggleActive }: EsimPackageCardProps) => {
  const imageUrl = getEsimPackageCardImage(pkg.id)
  const salePrice = price?.salePrice ?? 0
  const originalPrice = price?.originalPrice ?? 0
  const currency = price?.currency ?? 'VND'
  const showOriginal = originalPrice > salePrice && salePrice > 0

  return (
    <Card
      className="h-100 esim-package-card shadow-none border"
      role="button"
      onClick={onClick}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault()
          onClick()
        }
      }}
      tabIndex={0}>
      <div className="esim-package-card__hero">
        <img src={imageUrl} alt={pkg.name} className="esim-package-card__image" />
        <div className="esim-package-card__hero-overlay" />

        {salePrice > 0 && (
          <div className="esim-package-card__price-badge">
            {showOriginal && (
              <div className="esim-package-card__price-original">{formatMoney(originalPrice, currency)}</div>
            )}
            <div className="esim-package-card__price-sale">{formatMoney(salePrice, currency)}</div>
          </div>
        )}

        <div className="esim-package-card__hero-content">
          <h6 className="esim-package-card__title mb-0">{pkg.name}</h6>
        </div>
      </div>

      <Card.Body className="p-3 esim-package-card__body">
        <div className="esim-package-card__specs">
          <div className="esim-package-card__spec">
            <span className="esim-package-card__spec-label">Tốc độ</span>
            <span className="esim-package-card__spec-value">{pkg.speedPolicy || '—'}</span>
          </div>
          <div className="esim-package-card__spec">
            <span className="esim-package-card__spec-label">Dung lượng</span>
            <span className="esim-package-card__spec-value">{formatData(pkg)}</span>
          </div>
          <div className="esim-package-card__spec">
            <span className="esim-package-card__spec-label">Thời hạn</span>
            <span className="esim-package-card__spec-value">{formatValidity(pkg.validityDays)}</span>
          </div>
        </div>

        {pkg.countryName ? (
          <div className="esim-package-card__country">
            <LuMapPin className="esim-package-card__country-icon" />
            <span className="esim-package-card__country-name">{pkg.countryName}</span>
          </div>
        ) : null}
      </Card.Body>

      <Card.Footer
        className="bg-transparent border-top d-flex align-items-center justify-content-between gap-2 py-2 px-3"
        onClick={(e) => e.stopPropagation()}>
        <span className="esim-package-card__carrier d-inline-flex align-items-center gap-1 text-truncate">
          <LuBuilding2 className="esim-package-card__carrier-icon" />
          <span className="text-truncate">{getCarrierLabel(pkg)}</span>
        </span>

        <Form.Check
          type="switch"
          id={`esim-package-active-${pkg.id}`}
          label={<span className="esim-package-card__switch-label">Hoạt động</span>}
          className="mb-0 flex-shrink-0"
          checked={pkg.isActive}
          onChange={() => onToggleActive(pkg)}
        />
      </Card.Footer>
    </Card>
  )
}

export default EsimPackageCard
