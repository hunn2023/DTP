import { useEffect, type ReactNode } from 'react'
import clsx from 'clsx'
import { ProgressBar } from 'react-bootstrap'
import { TbBox, TbListDetails, TbPhoto } from 'react-icons/tb'
import { useWizard, Wizard } from 'react-use-wizard'

import ComponentCard from '@/components/cards/ComponentCard'
import type { ProductFormTab } from '@/features/master-data/products/types'

type ProductWizardProps = {
  title: string
  activeTab: ProductFormTab
  canAccessSubTabs: boolean
  onStepChange: (tab: ProductFormTab) => void
  productStep: ReactNode
  imagesStep: ReactNode
  attributesStep: ReactNode
}

type ProductWizardHeaderProps = {
  activeTab: ProductFormTab
  canAccessSubTabs: boolean
  onStepChange: (tab: ProductFormTab) => void
  className?: string
  withProgress?: boolean
}

const PRODUCT_WIZARD_TABS: ProductFormTab[] = ['product', 'images', 'attributes']

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

      <ul className={clsx('nav nav-tabs wizard-tabs mb-3', className)}>
        <li className="nav-item">
          <button
            type="button"
            className={clsx(
              'nav-link d-flex w-100 text-start border-0',
              activeStep === 0 && 'active',
              activeStep > 0 && 'wizard-item-done',
            )}
            onClick={() => handleGoToStep('product')}>
            <span className="d-flex align-items-center">
              <TbBox className="fs-32" />

              <span className="flex-grow-1 ms-2 text-truncate">
                <span className="mb-0 lh-base d-block fw-semibold text-body fs-base">Thông tin Product</span>
                <span className="fs-xxs mb-0">Tên, mã, danh mục</span>
              </span>
            </span>
          </button>
        </li>

        <li className="nav-item">
          <button
            type="button"
            disabled={!canAccessSubTabs}
            className={clsx(
              'nav-link d-flex w-100 text-start border-0',
              activeStep === 1 && 'active',
              activeStep > 1 && 'wizard-item-done',
              !canAccessSubTabs && 'disabled opacity-50',
            )}
            onClick={() => handleGoToStep('images')}>
            <span className="d-flex align-items-center">
              <TbPhoto className="fs-32" />

              <span className="flex-grow-1 ms-2 text-truncate">
                <span className="mb-0 lh-base d-block fw-semibold text-body fs-base">Hình ảnh</span>
                <span className="fs-xxs mb-0">Ảnh sản phẩm, thumbnail</span>
              </span>
            </span>
          </button>
        </li>

        <li className="nav-item">
          <button
            type="button"
            disabled={!canAccessSubTabs}
            className={clsx(
              'nav-link d-flex w-100 text-start border-0',
              activeStep === 2 && 'active',
              activeStep > 2 && 'wizard-item-done',
              !canAccessSubTabs && 'disabled opacity-50',
            )}
            onClick={() => handleGoToStep('attributes')}>
            <span className="d-flex align-items-center">
              <TbListDetails className="fs-32" />

              <span className="flex-grow-1 ms-2 text-truncate">
                <span className="mb-0 lh-base d-block fw-semibold text-body fs-base">Thuộc tính</span>
                <span className="fs-xxs mb-0">Thông số hiển thị</span>
              </span>
            </span>
          </button>
        </li>
      </ul>
    </>
  )
}

const ProductWizardStep = ({ children }: { children: ReactNode }) => {
  return <div className="col-md-8 border border-dashed rounded p-4">{children}</div>
}

const ProductWizard = ({
  title,
  activeTab,
  canAccessSubTabs,
  onStepChange,
  productStep,
  imagesStep,
  attributesStep,
}: ProductWizardProps) => {
  return (
    <ComponentCard title={title}>
      <div className="row">
        <Wizard
          startIndex={getTabIndex(activeTab)}
          header={
            <div className="col-md-4">
              <ProductWizardHeader
                activeTab={activeTab}
                canAccessSubTabs={canAccessSubTabs}
                onStepChange={onStepChange}
                className="flex-column wizard-bordered wizard-tabs nav-pills"
              />
            </div>
          }>
          <ProductWizardStep>{productStep}</ProductWizardStep>
          <ProductWizardStep>{imagesStep}</ProductWizardStep>
          <ProductWizardStep>{attributesStep}</ProductWizardStep>
        </Wizard>
      </div>
    </ComponentCard>
  )
}

export default ProductWizard