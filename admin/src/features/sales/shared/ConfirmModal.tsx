import { Button, Modal, ModalBody, ModalFooter, ModalHeader, ModalTitle } from 'react-bootstrap'

type ConfirmModalProps = {
  show: boolean
  title: string
  message: string
  confirmLabel?: string
  isSaving?: boolean
  onHide: () => void
  onConfirm: () => void
}

const ConfirmModal = ({
  show,
  title,
  message,
  confirmLabel = 'Xác nhận',
  isSaving = false,
  onHide,
  onConfirm,
}: ConfirmModalProps) => (
  <Modal show={show} onHide={onHide} centered>
    <ModalHeader closeButton>
      <ModalTitle>{title}</ModalTitle>
    </ModalHeader>
    <ModalBody>{message}</ModalBody>
    <ModalFooter>
      <Button variant="light" onClick={onHide} disabled={isSaving}>
        Hủy
      </Button>
      <Button variant="primary" onClick={onConfirm} disabled={isSaving}>
        {isSaving ? 'Đang xử lý...' : confirmLabel}
      </Button>
    </ModalFooter>
  </Modal>
)

export default ConfirmModal
