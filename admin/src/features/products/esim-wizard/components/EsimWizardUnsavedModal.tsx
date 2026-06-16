import { Button, Modal, ModalBody, ModalFooter, ModalHeader, ModalTitle } from 'react-bootstrap'

type EsimWizardUnsavedModalProps = {
  show: boolean
  isSaving: boolean
  onHide: () => void
  onSave: () => void
  onDiscard: () => void
}

const EsimWizardUnsavedModal = ({
  show,
  isSaving,
  onHide,
  onSave,
  onDiscard,
}: EsimWizardUnsavedModalProps) => (
  <Modal show={show} onHide={onHide} centered>
    <ModalHeader closeButton>
      <ModalTitle>Thay đổi chưa lưu</ModalTitle>
    </ModalHeader>
    <ModalBody>
      Có thay đổi chưa được lưu. Bạn có muốn lưu trước khi chuyển tab không?
    </ModalBody>
    <ModalFooter>
      <Button variant="outline-secondary" onClick={onDiscard} disabled={isSaving}>
        Không lưu
      </Button>
      <Button variant="primary" onClick={onSave} disabled={isSaving}>
        {isSaving ? 'Đang lưu...' : 'Lưu'}
      </Button>
    </ModalFooter>
  </Modal>
)

export default EsimWizardUnsavedModal
