import { useEffect } from 'react'
import clsx from 'clsx'
import {
  TbChecklist,
  TbCurrencyDollar,
  TbDeviceMobile,
  TbListDetails,
  TbPackage,
  TbRadio,
} from 'react-icons/tb'
import { useWizard } from 'react-use-wizard'

import { ESIM_WIZARD_STEPS, getEsimStepIndex } from '@/features/products/esim-wizard/esimWizardSteps'
import type { EsimWizardTab } from '@/features/products/esim-wizard/types'

const STEP_ICONS = [TbPackage, TbCurrencyDollar, TbDeviceMobile, TbRadio, TbListDetails, TbChecklist]

type EsimWizardStepperProps = {
  activeTab: EsimWizardTab
  canAccessTab: (tab: EsimWizardTab) => boolean
  onStepChange: (tab: EsimWizardTab) => void
  className?: string
}

const EsimWizardStepper = ({
  activeTab,
  canAccessTab,
  onStepChange,
  className,
}: EsimWizardStepperProps) => {
  const { goToStep, activeStep } = useWizard()
  const activeIndex = getEsimStepIndex(activeTab)

  useEffect(() => {
    if (activeStep !== activeIndex) {
      goToStep(activeIndex)
    }
  }, [activeTab, activeIndex, activeStep, goToStep])

  const handleGoToStep = (key: EsimWizardTab) => {
    if (!canAccessTab(key)) return
    onStepChange(key)
  }

  return (
    <ul className={clsx('nav nav-tabs wizard-tabs mb-3', className)}>
      {ESIM_WIZARD_STEPS.map((step, index) => {
        const Icon = STEP_ICONS[index]
        const disabled = !canAccessTab(step.key)

        return (
          <li key={step.key} className="nav-item">
            <button
              type="button"
              disabled={disabled}
              className={clsx(
                'nav-link d-flex w-100 text-start border-0',
                activeStep === index && 'active',
                activeStep > index && 'wizard-item-done',
                disabled && 'disabled opacity-50',
              )}
              onClick={() => handleGoToStep(step.key)}>
              <span className="d-flex align-items-center">
                <Icon className="fs-32" />
                <span className="flex-grow-1 ms-2 text-truncate">
                  <span className="mb-0 lh-base d-block fw-semibold text-body fs-base">{step.title}</span>
                  <span className="fs-xxs mb-0">{step.subtitle}</span>
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
