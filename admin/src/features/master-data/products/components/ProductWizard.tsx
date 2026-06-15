import { useEffect, type ReactNode } from 'react'
import clsx from 'clsx'
import { ProgressBar } from 'react-bootstrap'
import { TbBox, TbFileText, TbHelp, TbListDetails, TbPhoto } from 'react-icons/tb'
import { useWizard, Wizard } from 'react-use-wizard'

import ComponentCard from '@/components/cards/ComponentCard'
import { getProductTabTitle, PRODUCT_TAB_LABELS } from '@/features/master-data/products/productFormSteps'
import type { ProductFormTab } from '@/features/master-data/products/types'

type ProductWizardProps = {
  activeTab: ProductFormTab
  canAccessSubTabs: boolean
  onStepChange: (tab: ProductFormTab) => void
  productStep: ReactNode
  imagesStep: ReactNode
  attributesStep: ReactNode
  faqsStep: ReactNode
  contentsStep: ReactNode
}

type ProductWizardHeaderProps = {
  activeTab: ProductFormTab
  canAccessSubTabs: boolean
  onStepChange: (tab: ProductFormTab) => void
  className?: string
  withProgress?: boolean
}

const PRODUCT_WIZARD_TABS: ProductFormTab[] = ['product', 'images', 'attributes', 'faqs', 'contents']

const WIZARD_NAV_ITEMS: {
  key: ProductFormTab
  icon: typeof TbBox
  title: string
}[] = [
  { key: 'product', icon: TbBox, title: PRODUCT_TAB_LABELS.product },
  { key: 'images', icon: TbPhoto, title: PRODUCT_TAB_LABELS.images },
  { key: 'attributes', icon: TbListDetails, title: PRODUCT_TAB_LABELS.attributes },
  { key: 'faqs', icon: TbHelp, title: PRODUCT_TAB_LABELS.faqs },
  { key: 'contents', icon: TbFileText, title: PRODUCT_TAB_LABELS.contents },
]

const getTabIndex = (tab: ProductFormTab) => {
  const index = PRODUCT_WIZARD_TABS.indexOf(tab)
  return index >= 0 ? index : 0
}

const ProductWizardHeader = ({
  activeTab,
  canAccessSubTabs,
  onStepChange,
  className,
  withProgress,
}: ProductWizardHeaderProps) => {
  const { goToStep, activeStep, stepCount } = useWizard()

  useEffect(() => {
    const targetStep = getTabIndex(activeTab)

    if (activeStep !== targetStep) {
      goToStep(targetStep)
    }
  }, [activeTab, activeStep, goToStep])

  const handleGoToStep = (tab: ProductFormTab) => {
    if (tab !== 'product' && !canAccessSubTabs) return

    onStepChange(tab)
  }

  return (
    <>
      {withProgress && <ProgressBar now={(activeStep + 1) * (100 / stepCount)} className="mb-3" style={{ height: '6px' }} />}

      <ul className={clsx('nav nav-tabs wizard-tabs product-wizard-nav mb-3', className)}>
        {WIZARD_NAV_ITEMS.map((item, index) => {
          const Icon = item.icon
          const disabled = item.key !== 'product' && !canAccessSubTabs

          return (
            <li key={item.key} className="nav-item">
              <button
                type="button"
                disabled={disabled}
                className={clsx(
                  'nav-link d-flex w-100 text-start border-0',
                  activeStep === index && 'active',
                  activeStep > index && 'wizard-item-done',
                  disabled && 'disabled opacity-50',
                )}
                onClick={() => handleGoToStep(item.key)}>
                <span className="d-flex align-items-center">
                  <Icon className="fs-32" />

                  <span className="flex-grow-1 ms-2 text-truncate">
                    <span className="product-wizard-nav__title mb-0 d-block fw-semibold text-body fs-base">
                      {item.title}
                    </span>
                  </span>
                </span>
              </button>
            </li>
          )
        })}
      </ul>
    </>
  )
}

const ProductWizardStep = ({ children }: { children: ReactNode }) => {
  return <div className="col-12 product-wizard-content-col border border-dashed rounded p-4">{children}</div>
}

const ProductWizard = ({
  activeTab,
  canAccessSubTabs,
  onStepChange,
  productStep,
  imagesStep,
  attributesStep,
  faqsStep,
  contentsStep,
}: ProductWizardProps) => {
  return (
    <ComponentCard title={getProductTabTitle(activeTab)}>
      <div className="row product-wizard-layout">
        <Wizard
          startIndex={getTabIndex(activeTab)}
          header={
            <div className="col-12 product-wizard-nav-col">
              <ProductWizardHeader
                activeTab={activeTab}
                canAccessSubTabs={canAccessSubTabs}
                onStepChange={onStepChange}
                className="flex-column wizard-bordered nav-pills"
              />
            </div>
          }>
          <ProductWizardStep>{productStep}</ProductWizardStep>
          <ProductWizardStep>{imagesStep}</ProductWizardStep>
          <ProductWizardStep>{attributesStep}</ProductWizardStep>
          <ProductWizardStep>{faqsStep}</ProductWizardStep>
          <ProductWizardStep>{contentsStep}</ProductWizardStep>
        </Wizard>
      </div>
    </ComponentCard>
  )
}

export default ProductWizard
