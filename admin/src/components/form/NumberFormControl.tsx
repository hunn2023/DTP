import { Form, type FormControlProps } from 'react-bootstrap'

import { fromNumberInputValue, toNumberInputValue } from '@/components/form/numberFieldUtils'

type NumberFormControlProps = Omit<FormControlProps, 'value' | 'onChange' | 'type'> & {
  value: number
  onChange: (value: number) => void
  emptyWhenZero?: boolean
}

const NumberFormControl = ({
  value,
  onChange,
  emptyWhenZero = true,
  ...rest
}: NumberFormControlProps) => (
  <Form.Control
    type="number"
    value={toNumberInputValue(value, emptyWhenZero)}
    onChange={(e) => onChange(fromNumberInputValue(e.target.value))}
    {...rest}
  />
)

export default NumberFormControl
