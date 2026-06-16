import { forwardRef } from 'react'
import { Form, type FormControlProps } from 'react-bootstrap'
import { NumericFormat } from 'react-number-format'

type NumberFormControlProps = Omit<FormControlProps, 'value' | 'onChange' | 'type' | 'defaultValue'> & {
  value: number
  onChange: (value: number) => void
  emptyWhenZero?: boolean
  decimalScale?: number
  thousandSeparator?: string | boolean
}

const BootstrapInput = forwardRef<HTMLInputElement, FormControlProps>((props, ref) => (
  <Form.Control ref={ref} {...props} />
))
BootstrapInput.displayName = 'BootstrapInput'

const NumberFormControl = ({
  value,
  onChange,
  emptyWhenZero = true,
  decimalScale = 0,
  thousandSeparator = '.',
  min,
  ...rest
}: NumberFormControlProps) => {
  const minValue = min !== undefined ? Number(min) : undefined
  const displayValue = emptyWhenZero && value === 0 ? '' : value

  return (
    <NumericFormat
      customInput={BootstrapInput}
      thousandSeparator={thousandSeparator}
      decimalSeparator=","
      decimalScale={decimalScale}
      fixedDecimalScale={decimalScale > 0}
      allowNegative={minValue === undefined || minValue < 0}
      value={displayValue}
      onValueChange={(vals) => onChange(vals.floatValue ?? 0)}
      {...rest}
    />
  )
}

export default NumberFormControl
