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
    <ul className="nav nav-tabs wizard-tabs product-form-stepper">
      {ESIM_WIZARD_STEPS.map((step, index) => {
        const Icon = STEP_ICONS[index]
        const isDisabled = step.key !== 'variants' && !canAccessSubTabs
        const isActive = activeIndex === index

        return (
          <li key={step.key} className="nav-item">
            <button
              type="button"
              disabled={isDisabled}
              className={clsx('nav-link d-flex w-100 text-start border-0', isActive && 'active')}
              onClick={() => onStepClick(step.key)}>
              <span className="d-flex align-items-center w-100 gap-2">
                <span className="step-icon rounded-circle d-flex align-items-center justify-content-center flex-shrink-0 bg-primary-subtle text-primary">
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

export default EsimWizardStepper
