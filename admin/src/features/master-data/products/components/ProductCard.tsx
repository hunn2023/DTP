import { Card, Form } from 'react-bootstrap'
import { LuFolder, LuShoppingCart } from 'react-icons/lu'
import { TbDeviceSim } from 'react-icons/tb'

import defaultProductImage from '@/assets/images/cards/none-images.png'
import type { CatalogProduct } from '@/features/master-data/products/types'

type ProductCardProps = {
  product: CatalogProduct
  countryFlagUrl?: string
  onClick: () => void
  onToggleActive: (product: CatalogProduct) => void
}

const ProductCard = ({ product, countryFlagUrl, onClick, onToggleActive }: ProductCardProps) => {
  const thumbnailUrl = product.thumbnailUrl || defaultProductImage
  const isDefaultThumbnail = !product.thumbnailUrl

  return (
    <Card
      className="h-100 product-card shadow-none border"
      role="button"
      onClick={onClick}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault()
          onClick()
        }
      }}
      tabIndex={0}>
      <div className="product-card__media position-relative">
        <img
          src={thumbnailUrl}
          alt={product.name}
          className={`product-card__image${isDefaultThumbnail ? ' product-card__image--default' : ''}`}
        />

        <div className="product-card__badges position-absolute top-0 end-0 m-2 d-flex flex-column gap-1">
          {product.isHot && <span className="badge bg-danger">HOT</span>}
          {product.isFeatured && <span className="badge bg-primary">FEATURED</span>}
        </div>

        <div className="product-card__sim position-absolute bottom-0 start-0 m-2">
          <TbDeviceSim className="fs-lg" />
        </div>
      </div>

      <Card.Body className="p-3 product-card__body">
        <div className="product-card__body-main">
          <h6 className="product-card__title mb-0 text-truncate">{product.name}</h6>

          <div className="d-flex flex-wrap align-items-center gap-3 product-card__meta">
            {product.categoryName && (
              <span className="d-inline-flex align-items-center gap-1 text-truncate">
                <LuFolder className="product-card__meta-icon flex-shrink-0" />
                <span className="product-card__meta-value text-truncate">{product.categoryName}</span>
              </span>
            )}
            {product.countryName && (
              <span className="d-inline-flex align-items-center gap-1 text-truncate">
                {countryFlagUrl ? (
                  <img src={countryFlagUrl} alt="" className="product-card__flag flex-shrink-0" />
                ) : null}
                <span className="product-card__meta-value text-truncate">{product.countryName}</span>
              </span>
            )}
          </div>

          {product.shortDescription ? (
            <p className="product-card__desc-main mb-0">{product.shortDescription}</p>
          ) : null}
        </div>

        {product.locationText ? (
          <p className="product-card__desc-value mb-0">{product.locationText}</p>
        ) : null}
      </Card.Body>

      <Card.Footer
        className="bg-transparent border-top d-flex align-items-center justify-content-between gap-2 py-2 px-3"
        onClick={(e) => e.stopPropagation()}>
        <span className="product-card__sold d-inline-flex align-items-center gap-1">
          <LuShoppingCart className="product-card__sold-icon" />
          <span className="product-card__sold-label">Đã bán:</span>
          <span className="product-card__sold-value">{product.soldCount}</span>
        </span>

        <Form.Check
          type="switch"
          id={`product-active-${product.id}`}
          label={<span className="product-card__switch-label">Hoạt động</span>}
          className="mb-0 product-card__switch"
          checked={product.isActive}
          onChange={() => onToggleActive(product)}
        />
      </Card.Footer>
    </Card>
  )
}

export default ProductCard
