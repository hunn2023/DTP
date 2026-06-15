import { type FormEvent, useEffect, useState } from 'react'
import { Button, Form, Modal, ModalBody, ModalFooter, ModalHeader, ModalTitle } from 'react-bootstrap'

type PromptModalProps = {
  show: boolean
  title: string
  label: string
  placeholder?: string
  required?: boolean
  initialValue?: string
  confirmLabel?: string
  isSaving?: boolean
  onHide: () => void
  onConfirm: (value: string) => void
}

const PromptModal = ({
  show,
  title,
  label,
  placeholder,
  required = true,
  initialValue = '',
  confirmLabel = 'Xác nhận',
  isSaving = false,
  onHide,
  onConfirm,
}: PromptModalProps) => {
  const [value, setValue] = useState(initialValue)
  const [error, setError] = useState('')

  useEffect(() => {
    if (show) {
      setValue(initialValue)
      setError('')
    }
  }, [show, initialValue])

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    if (required && !value.trim()) {
      setError('Trường này là bắt buộc')
      return
    }
    onConfirm(value.trim())
  }

  return (
    <Modal show={show} onHide={onHide} centered>
      <Form onSubmit={handleSubmit}>
        <ModalHeader closeButton>
          <ModalTitle>{title}</ModalTitle>
        </ModalHeader>
        <ModalBody>
          <Form.Label className="fw-semibold">{label}</Form.Label>
          <Form.Control
            as="textarea"
            rows={3}
            value={value}
            placeholder={placeholder}
            onChange={(e) => {
              setValue(e.target.value)
              setError('')
            }}
          />
          {error && <div className="text-danger fs-xs mt-1">{error}</div>}
        </ModalBody>
        <ModalFooter>
          <Button variant="light" onClick={onHide} disabled={isSaving}>
            Hủy
          </Button>
          <Button variant="primary" type="submit" disabled={isSaving}>
            {isSaving ? 'Đang xử lý...' : confirmLabel}
          </Button>
        </ModalFooter>
      </Form>
    </Modal>
  )
}

export default PromptModal
