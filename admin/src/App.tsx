import { Suspense } from 'react'
import { useRoutes } from 'react-router'

import Loader from '@/components/Loader'
import { routes } from '@/routes'

const App = () => {
  return (
    <Suspense fallback={<Loader height="100vh" />}>
      {useRoutes(routes)}
    </Suspense>
  )
}

export default App
