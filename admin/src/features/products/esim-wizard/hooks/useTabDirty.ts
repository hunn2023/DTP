import { useEffect, useRef } from 'react'

export function useTabDirty<T>(
  values: T,
  baseline: T | null | undefined,
  onDirtyChange?: (dirty: boolean) => void,
): void {
  const baselineRef = useRef(baseline)
  baselineRef.current = baseline

  const onDirtyChangeRef = useRef(onDirtyChange)
  onDirtyChangeRef.current = onDirtyChange

  useEffect(() => {
    if (!onDirtyChangeRef.current || baselineRef.current == null) return
    onDirtyChangeRef.current(JSON.stringify(values) !== JSON.stringify(baselineRef.current))
  }, [values])
}
