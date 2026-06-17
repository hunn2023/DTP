import { LayoutProvider } from '@/context/useLayoutContext'
import { NotificationProvider } from '@/context/useNotificationContext'
import { AuthProvider } from '@/context/useAuthContext'
import { type ChildrenType } from '@/types'

const AppWrapper = ({ children }: ChildrenType) => {
  return (
    <AuthProvider>
      <LayoutProvider>
        <NotificationProvider>{children}</NotificationProvider>
      </LayoutProvider>
    </AuthProvider>
  )
}

export default AppWrapper
