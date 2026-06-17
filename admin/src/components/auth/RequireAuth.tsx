import { Navigate, Outlet, useLocation } from 'react-router'

import Loader from '@/components/Loader'
import { useAuth } from '@/context/useAuthContext'

const RequireAuth = () => {
  const { isAuthenticated, isBootstrapping } = useAuth()
  const location = useLocation()

  if (isBootstrapping) {
    return <Loader height="100vh" />
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />
  }

  return <Outlet />
}

export default RequireAuth
