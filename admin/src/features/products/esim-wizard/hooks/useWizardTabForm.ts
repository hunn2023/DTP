import { useEffect, useRef, useState } from 'react'

type Identifiable = { id?: string }

export function useWizardTabForm<T extends Identifiable>(
  initialValues: T | null,
  createDefaults: () => T,
): [T, React.Dispatch<React.SetStateAction<T>>] {
  const [values, setValues] = useState<T>(() => initialValues ?? createDefaults())
  const syncedIdRef = useRef(initialValues?.id ?? '')

  useEffect(() => {
    if (!initialValues?.id) return
    if (syncedIdRef.current === initialValues.id) return
    syncedIdRef.current = initialValues.id
    setValues(initialValues)
  }, [initialValues])

  return [values, setValues]
}
