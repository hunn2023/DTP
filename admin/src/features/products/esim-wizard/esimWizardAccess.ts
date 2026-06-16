import type { EsimWizardTab } from '@/features/products/esim-wizard/types'

export type WizardAccessContext = {
  variantId: string | null
  isSetupFlow: boolean
  hasPrice: boolean
  hasPackage: boolean
}

export function canAccessWizardTab(tab: EsimWizardTab, ctx: WizardAccessContext): boolean {
  if (tab === 'variants') return true
  if (!ctx.variantId) return false
  if (!ctx.isSetupFlow) return true

  if (tab === 'prices') return true
  if (tab === 'packages') return ctx.hasPrice
  if (tab === 'carriers' || tab === 'features' || tab === 'review') return ctx.hasPackage
  return false
}

export function getNextSetupTab(tab: EsimWizardTab): EsimWizardTab | null {
  if (tab === 'variants') return 'prices'
  if (tab === 'prices') return 'packages'
  if (tab === 'packages') return 'carriers'
  if (tab === 'carriers') return 'features'
  if (tab === 'features') return 'review'
  return null
}

export function getWizardFallbackTab(ctx: WizardAccessContext): EsimWizardTab {
  if (!ctx.variantId) return 'variants'
  if (!ctx.hasPrice) return 'prices'
  if (!ctx.hasPackage) return 'packages'
  return 'variants'
}

export function resolveIsSetupFlow(opts: {
  isNewRoute: boolean
  setupParam: boolean
  packageIdParam: string | null
  variantId: string | null
  hasPackage: boolean
  isLoading: boolean
}): boolean {
  if (opts.setupParam || opts.isNewRoute) return true
  if (opts.packageIdParam) return false
  return Boolean(opts.variantId) && !opts.hasPackage && !opts.isLoading
}
