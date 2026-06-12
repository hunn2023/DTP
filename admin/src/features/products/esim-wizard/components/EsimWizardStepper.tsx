import clsx from 'clsx'
import {
  TbChecklist,
  TbCurrencyDollar,
  TbDeviceMobile,
  TbListDetails,
  TbPackage,
  TbRadio,
} from 'react-icons/tb'

import { ESIM_WIZARD_STEPS, getEsimStepIndex } from '@/features/products/esim-wizard/esimWizardSteps'
import type { EsimWizardTab } from '@/features/products/esim-wizard/types'

const STEP_ICONS = [TbPackage, TbCurrencyDollar, TbDeviceMobile, TbRadio, TbListDetails, TbChecklist]

type EsimWizardStepperProps = {
  activeTab: EsimWizardTab
  canAccessSubTabs: boolean
  onStepClick: (tab: EsimWizardTab) => void
}

const EsimWizardStepper = ({ activeTab, canAccessSubTabs, onStepClick }: EsimWizardStepperProps) => {
  const activeIndex = getEsimStepIndex(activeTab)

  return (
    <div className="d-flex gap-2 overflow-auto pb-2">
      {ESIM_WIZARD_STEPS.map((step, index) => {
        const Icon = STEP_ICONS[index]
        const isDisabled = step.key !== 'variants' && !canAccessSubTabs
        const isActive = activeIndex === index
        const isDone = activeIndex > index

        return (
          <button
            key={step.key}
            type="button"
            disabled={isDisabled}
            className={clsx(
              'btn text-start border rounded-3 p-3 flex-shrink-0',
              isActive && 'btn-primary text-white shadow-sm',
              !isActive && isDone && 'btn-light border-primary-subtle',
              !isActive && !isDone && 'btn-light',
            )}
            style={{ minWidth: '12.5rem' }}
            onClick={() => onStepClick(step.key)}>
            <span className="d-flex align-items-center gap-2">
              <span
                className={clsx(
                  'rounded-circle d-inline-flex align-items-center justify-content-center flex-shrink-0',
                  isActive ? 'bg-white text-primary' : 'bg-primary-subtle text-primary',
                )}
                style={{ width: 34, height: 34 }}>
                <Icon />
              </span>
              <span className="min-w-0">
                <span className={clsx('d-block fw-semibold', !isActive && 'text-body')}>
                  {step.step}. {step.title}
                </span>
                <span className={clsx('d-block fs-xs', isActive ? 'text-white-50' : 'text-muted')}>
                  {step.subtitle}
                </span>
              </span>
            </span>
          </button>
        )
      })}
    </div>
  )
}

export default EsimWizardStepper
