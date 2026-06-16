import type { ReactNode } from 'react'
import { Wizard } from 'react-use-wizard'

import ComponentCard from '@/components/cards/ComponentCard'
import EsimWizardStepper from '@/features/products/esim-wizard/components/EsimWizardStepper'
import { getEsimStepIndex } from '@/features/products/esim-wizard/esimWizardSteps'
import type { EsimWizardTab } from '@/features/products/esim-wizard/types'

type EsimWizardProps = {
  title: string
  activeTab: EsimWizardTab
  canAccessTab: (tab: EsimWizardTab) => boolean
  onStepChange: (tab: EsimWizardTab) => void
  variantStep: ReactNode
  priceStep: ReactNode
  packageStep: ReactNode
  carrierStep: ReactNode
  featureStep: ReactNode
  reviewStep: ReactNode
}

const EsimWizardStep = ({ children }: { children: ReactNode }) => {
  return <div className="col-md-8 border border-dashed rounded p-4">{children}</div>
}

const EsimWizard = ({
  title,
  activeTab,
  canAccessTab,
  onStepChange,
  variantStep,
  priceStep,
  packageStep,
  carrierStep,
  featureStep,
  reviewStep,
}: EsimWizardProps) => {
  return (
    <ComponentCard title={title}>
      <div className="row">
        <Wizard
          startIndex={getEsimStepIndex(activeTab)}
          header={
            <div className="col-md-4">
              <EsimWizardStepper
                activeTab={activeTab}
                canAccessTab={canAccessTab}
                onStepChange={onStepChange}
                className="flex-column wizard-bordered wizard-tabs nav-pills"
              />
            </div>
          }>
          <EsimWizardStep>{variantStep}</EsimWizardStep>
          <EsimWizardStep>{priceStep}</EsimWizardStep>
          <EsimWizardStep>{packageStep}</EsimWizardStep>
          <EsimWizardStep>{carrierStep}</EsimWizardStep>
          <EsimWizardStep>{featureStep}</EsimWizardStep>
          <EsimWizardStep>{reviewStep}</EsimWizardStep>
        </Wizard>
      </div>
    </ComponentCard>
  )
}

export default EsimWizard
