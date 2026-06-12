import clsx from 'clsx'
import { TbBox, TbCheck, TbListDetails, TbLock, TbPhoto } from 'react-icons/tb'

import { getStepIndex, PRODUCT_FORM_STEPS } from '@/features/master-data/products/productFormSteps'
import type { ProductFormTab } from '@/features/master-data/products/types'

const STEP_ICONS = [TbBox, TbPhoto, TbListDetails]

type ProductFormStepperProps = {
  activeTab: ProductFormTab
  canAccessSubTabs: boolean
  onStepClick: (tab: ProductFormTab) => void
}

const ProductFormStepper = ({ activeTab, canAccessSubTabs, onStepClick }: ProductFormStepperProps) => {
  const activeIndex = getStepIndex(activeTab)

  return (
    <>
      <style>{`
        .product-stepper {
          display: grid;
          grid-template-columns: repeat(3, minmax(0, 1fr));
          gap: 14px;
          position: relative;
        }

        .product-stepper-item {
          position: relative;
          display: flex;
          align-items: stretch;
        }

        .product-stepper-line {
          position: absolute;
          top: 30px;
          left: -50%;
          width: calc(100% + 14px);
          height: 2px;
          background: var(--bs-border-color);
          z-index: 0;
        }

        .product-stepper-button {
          position: relative;
          z-index: 1;
          width: 100%;
          min-height: 88px;
          border: 1px solid var(--bs-border-color);
          background: var(--bs-body-bg);
          border-radius: 18px;
          padding: 16px;
          display: flex;
          align-items: center;
          gap: 12px;
          text-align: left;
          transition: all 0.18s ease;
        }

        .product-stepper-button:hover:not(:disabled) {
          border-color: var(--bs-primary);
          box-shadow: 0 10px 28px rgba(13, 110, 253, 0.12);
          transform: translateY(-1px);
        }

        .product-stepper-button:disabled {
          cursor: not-allowed;
          opacity: 0.68;
        }

        .product-stepper-icon {
          width: 48px;
          height: 48px;
          border-radius: 999px;
          background: var(--bs-secondary-bg);
          color: var(--bs-secondary-color);
          display: flex;
          align-items: center;
          justify-content: center;
          flex-shrink: 0;
          font-size: 23px;
          transition: all 0.18s ease;
        }

        .product-stepper-content {
          min-width: 0;
          display: block;
        }

        .product-stepper-title {
          display: block;
          color: var(--bs-body-color);
          font-weight: 700;
          line-height: 1.25;
        }

        .product-stepper-number {
          display: block;
          color: var(--bs-secondary-color);
          font-size: 12px;
          font-weight: 600;
          margin-bottom: 3px;
        }

        .product-stepper-subtitle {
          display: block;
          color: var(--bs-secondary-color);
          font-size: 13px;
          margin-top: 4px;
          line-height: 1.35;
        }

        .product-stepper-item.is-active .product-stepper-button {
          border-color: var(--bs-primary);
          background: rgba(var(--bs-primary-rgb), 0.07);
          box-shadow: 0 12px 30px rgba(13, 110, 253, 0.14);
        }

        .product-stepper-item.is-active .product-stepper-icon {
          background: var(--bs-primary);
          color: #fff;
        }

        .product-stepper-item.is-active .product-stepper-number,
        .product-stepper-item.is-active .product-stepper-title {
          color: var(--bs-primary);
        }

        .product-stepper-item.is-done .product-stepper-button {
          border-color: rgba(var(--bs-success-rgb), 0.35);
          background: rgba(var(--bs-success-rgb), 0.04);
        }

        .product-stepper-item.is-done .product-stepper-icon {
          background: var(--bs-success);
          color: #fff;
        }

        .product-stepper-item.is-done .product-stepper-line {
          background: var(--bs-success);
        }

        .product-stepper-item.is-disabled .product-stepper-button {
          background: var(--bs-light);
        }

        .product-stepper-item.is-disabled .product-stepper-icon {
          background: var(--bs-tertiary-bg);
          color: var(--bs-secondary-color);
        }

        @media (max-width: 991.98px) {
          .product-stepper {
            grid-template-columns: 1fr;
            gap: 12px;
          }

          .product-stepper-line {
            display: none;
          }

          .product-stepper-button {
            min-height: auto;
            border-radius: 14px;
          }
        }
      `}</style>

      <div className="product-stepper">
        {PRODUCT_FORM_STEPS.map((step, index) => {
          const Icon = STEP_ICONS[index]
          const isDisabled = step.key !== 'product' && !canAccessSubTabs
          const isActive = activeIndex === index
          const isDone = index < activeIndex && !isDisabled

          return (
            <div
              key={step.key}
              className={clsx(
                'product-stepper-item',
                isActive && 'is-active',
                isDone && 'is-done',
                isDisabled && 'is-disabled',
              )}>
              {index > 0 && <div className="product-stepper-line" />}

              <button
                type="button"
                disabled={isDisabled}
                className="product-stepper-button"
                onClick={() => onStepClick(step.key)}>
                <span className="product-stepper-icon">
                  {isDisabled ? <TbLock /> : isDone ? <TbCheck /> : <Icon />}
                </span>

                <span className="product-stepper-content">
                  <span className="product-stepper-title">
                    <span className="product-stepper-number">Bước {step.step}</span>
                    {step.title}
                  </span>

                  <span className="product-stepper-subtitle">{step.subtitle}</span>
                </span>
              </button>
            </div>
          )
        })}
      </div>
    </>
  )
}

export default ProductFormStepper