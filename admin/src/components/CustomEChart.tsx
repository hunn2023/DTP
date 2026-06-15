import { type EChartsOption } from 'echarts'
import { type EChartsReactProps } from 'echarts-for-react'
import * as echarts from 'echarts/core'
import { lazy, Suspense, useMemo } from 'react'

import Loader from '@/components/Loader'
import { useLayoutContext } from '@/context/useLayoutContext'

const ReactECharts = lazy(() => import('echarts-for-react'))

type EChartClientProps = {
  getOptions: () => EChartsOption
  extensions: any[]
} & Omit<EChartsReactProps, 'option'>

let extensionsRegistered = false

const CustomEChart = ({ getOptions, extensions, ...props }: EChartClientProps) => {
  if (!extensionsRegistered) {
    echarts.use(extensions)
    extensionsRegistered = true
  }

  const { skin, theme } = useLayoutContext()

  const options = useMemo(() => getOptions(), [getOptions, skin, theme])

  const chartHeight = props.style?.height != null ? String(props.style.height) : '200px'

  return (
    <Suspense fallback={<Loader height={chartHeight} />}>
      <ReactECharts echarts={echarts} {...props} option={options} />
    </Suspense>
  )
}

export default CustomEChart
