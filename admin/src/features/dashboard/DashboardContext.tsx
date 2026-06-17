import { createContext, useContext, type ReactNode } from 'react'

import type { useDashboardPage } from '@/features/dashboard/useDashboardPage'

type DashboardContextValue = ReturnType<typeof useDashboardPage>

const DashboardContext = createContext<DashboardContextValue | null>(null)

type DashboardProviderProps = {
  value: DashboardContextValue
  children: ReactNode
}

export function DashboardProvider({ value, children }: DashboardProviderProps) {
  return <DashboardContext.Provider value={value}>{children}</DashboardContext.Provider>
}

export function useDashboard(): DashboardContextValue {
  const context = useContext(DashboardContext)
  if (!context) {
    throw new Error('useDashboard must be used within DashboardProvider')
  }
  return context
}
