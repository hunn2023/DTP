import clsx from 'clsx'
import { TbBox, TbListDetails, TbPhoto } from 'react-icons/tb'

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
    <ul className="nav nav-tabs wizard-tabs product-form-stepper">
      {PRODUCT_FORM_STEPS.map((step, index) => {
        const Icon = STEP_ICONS[index]
        const isDisabled = step.key !== 'product' && !canAccessSubTabs
        const isActive = activeIndex === index

        return (
          <li key={step.key} className="nav-item">
            <button
              type="button"
              disabled={isDisabled}
              className={clsx('nav-link d-flex w-100 text-start border-0', isActive && 'active')}
              onClick={() => onStepClick(step.key)}>
              <span className="d-flex align-items-center w-100 gap-2">
                <span
                  className={clsx(
                    'step-icon rounded-circle d-flex align-items-center justify-content-center flex-shrink-0',
                    'bg-primary-subtle text-primary',
                  )}>
                  <Icon />
                </span>
                <span className="flex-grow-1 text-truncate">
                  <span className="step-label mb-0 d-block fw-semibold text-body">
                    {step.step}. {step.title}
                  </span>
                  <span className="step-subtitle mb-0 d-block text-muted">{step.subtitle}</span>
                </span>
              </span>
            </button>
          </li>
        )
      })}
    </ul>
  )
}

export default ProductFormStepper
