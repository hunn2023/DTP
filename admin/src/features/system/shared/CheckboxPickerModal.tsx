import { useEffect, useMemo, useState } from 'react'
import { Button, Form, Modal, ModalBody, ModalFooter, ModalHeader, ModalTitle, Spinner } from 'react-bootstrap'

export type PickerOption = {
  value: string
  label: string
  hint?: string
}

type CheckboxPickerModalProps = {
  show: boolean
  title: string
  options?: PickerOption[]
  groupedOptions?: Record<string, PickerOption[]>
  selectedIds: string[]
  isLoading?: boolean
  isSaving?: boolean
  onHide: () => void
  onSave: (selectedIds: string[]) => void
}

function toggleId(ids: string[], id: string): string[] {
  return ids.includes(id) ? ids.filter((item) => item !== id) : [...ids, id]
}

function OptionList({
  options,
  selectedIds,
  onToggle,
}: {
  options: PickerOption[]
  selectedIds: string[]
  onToggle: (id: string) => void
}) {
  return (
    <div className="d-flex flex-column gap-2">
      {options.map((option) => (
        <Form.Check
          key={option.value}
          type="checkbox"
          id={`picker-${option.value}`}
          label={
            <span>
              <span className="fw-medium">{option.label}</span>
              {option.hint ? <span className="text-muted ms-2 fs-xs">{option.hint}</span> : null}
            </span>
          }
          checked={selectedIds.includes(option.value)}
          onChange={() => onToggle(option.value)}
        />
      ))}
    </div>
  )
}

const CheckboxPickerModal = ({
  show,
  title,
  options = [],
  groupedOptions,
  selectedIds,
  isLoading = false,
  isSaving = false,
  onHide,
  onSave,
}: CheckboxPickerModalProps) => {
  const [draftIds, setDraftIds] = useState<string[]>(selectedIds)

  useEffect(() => {
    if (show) setDraftIds(selectedIds)
  }, [show, selectedIds])

  const flatOptions = useMemo(() => {
    if (options.length > 0) return options
    if (!groupedOptions) return []
    return Object.values(groupedOptions).flat()
  }, [options, groupedOptions])

  const handleToggle = (id: string) => {
    setDraftIds((prev) => toggleId(prev, id))
  }

  const handleSelectAll = () => {
    setDraftIds(flatOptions.map((item) => item.value))
  }

  const handleClear = () => {
    setDraftIds([])
  }

  return (
    <Modal show={show} onHide={onHide} centered size="lg" scrollable>
      <ModalHeader closeButton>
        <ModalTitle>{title}</ModalTitle>
      </ModalHeader>
      <ModalBody>
        {isLoading ? (
          <div className="text-center py-4">
            <Spinner animation="border" size="sm" className="me-2" />
            Đang tải...
          </div>
        ) : groupedOptions ? (
          Object.entries(groupedOptions).map(([module, moduleOptions]) => (
            <div key={module} className="mb-4">
              <div className="fw-semibold mb-2">{module}</div>
              <OptionList options={moduleOptions} selectedIds={draftIds} onToggle={handleToggle} />
            </div>
          ))
        ) : (
          <OptionList options={options} selectedIds={draftIds} onToggle={handleToggle} />
        )}
      </ModalBody>
      <ModalFooter className="justify-content-between">
        <div className="d-flex gap-2">
          <Button variant="light" size="sm" onClick={handleSelectAll} disabled={isLoading || isSaving}>
            Chọn tất cả
          </Button>
          <Button variant="light" size="sm" onClick={handleClear} disabled={isLoading || isSaving}>
            Bỏ chọn
          </Button>
        </div>
        <div className="d-flex gap-2">
          <Button variant="light" onClick={onHide} disabled={isSaving}>
            Hủy
          </Button>
          <Button variant="primary" onClick={() => onSave(draftIds)} disabled={isLoading || isSaving}>
            {isSaving ? 'Đang lưu...' : 'Lưu'}
          </Button>
        </div>
      </ModalFooter>
    </Modal>
  )
}

export default CheckboxPickerModal
