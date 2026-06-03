import { type FormEvent, useEffect, useState } from 'react'
import { Button, Col, Form, Modal, ModalBody, ModalFooter, ModalHeader, ModalTitle, Row } from 'react-bootstrap'

import { getFieldLabel } from '@/modules/crud/entities/fieldLabels'
import type { FormFieldConfig, FormModalMode } from '@/modules/crud/form/types'
import type { CrudEntityBase } from '@/modules/crud/types'

function resolveLabel<T>(field: FormFieldConfig<T>): string {
  return field.label ?? getFieldLabel(field.name)
}

type EntityFormModalProps<T extends CrudEntityBase> = {
  show: boolean
  mode: FormModalMode
  entityName: string
  fields: FormFieldConfig<T>[]
  viewFields?: FormFieldConfig<T>[]
  initialValues: T
  onHide: () => void
  onSubmit: (values: T) => void
}

function getFieldValue<T>(values: T, name: keyof T & string): string {
  const raw = values[name as keyof T]
  if (Array.isArray(raw)) return ''
  if (typeof raw === 'boolean') return raw ? 'true' : 'false'
  if (raw === null || raw === undefined) return ''
  return String(raw)
}

function getMultiSelectIds<T>(values: T, name: keyof T & string): number[] {
  const raw = values[name as keyof T]
  if (!Array.isArray(raw)) return []
  return raw.map((id) => Number(id)).filter((id) => !Number.isNaN(id))
}

function FormFieldInput<T extends CrudEntityBase>({
  field,
  value,
  selectedIds,
  readOnly,
  onChange,
}: {
  field: FormFieldConfig<T>
  value: string
  selectedIds: number[]
  readOnly: boolean
  onChange: (name: keyof T & string, value: string | boolean | number | number[]) => void
}) {
  const common = {
    disabled: readOnly,
    required: field.required && !readOnly,
    placeholder: field.placeholder,
  }

  if (field.type === 'textarea') {
    return (
      <Form.Control
        as="textarea"
        rows={3}
        {...common}
        value={value}
        onChange={(e) => onChange(field.name, e.target.value)}
      />
    )
  }

  if (field.type === 'checkbox') {
    return (
      <Form.Check
        type="switch"
        id={`field-${field.name}`}
        label={resolveLabel(field)}
        checked={value === 'true'}
        disabled={readOnly}
        onChange={(e) => onChange(field.name, e.target.checked)}
      />
    )
  }

  if (field.type === 'multiselect') {
    return (
      <>
        <Form.Select
          {...common}
          multiple
          value={selectedIds.map(String)}
          onChange={(e) => {
            const ids = Array.from(e.target.selectedOptions, (opt) => Number(opt.value))
            onChange(field.name, ids)
          }}>
          {field.options?.map((opt) => (
            <option key={opt.value} value={opt.value}>
              {opt.label}
            </option>
          ))}
        </Form.Select>
        <Form.Text className="text-muted">Giữ Ctrl (Windows) hoặc Cmd (Mac) để chọn nhiều mục.</Form.Text>
      </>
    )
  }

  if (field.type === 'select') {
    return (
      <Form.Select
        {...common}
        value={value}
        onChange={(e) => {
          const v = e.target.value
          onChange(field.name, field.parseAsNumber ? Number(v) : v)
        }}>
        <option value="">-- Chọn --</option>
        {field.options?.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </Form.Select>
    )
  }

  const inputType =
    field.type === 'number'
      ? 'number'
      : field.type === 'color'
        ? 'color'
        : field.type === 'url'
          ? 'url'
          : field.type === 'password'
            ? 'password'
            : 'text'

  return (
    <Form.Control
      type={inputType}
      {...common}
      value={value}
      onChange={(e) => onChange(field.name, field.type === 'number' ? Number(e.target.value) : e.target.value)}
    />
  )
}

const EntityFormModal = <T extends CrudEntityBase>({
  show,
  mode,
  entityName,
  fields,
  viewFields,
  initialValues,
  onHide,
  onSubmit,
}: EntityFormModalProps<T>) => {
  const [values, setValues] = useState<T>(initialValues)
  const [errors, setErrors] = useState<Partial<Record<keyof T & string, string>>>({})
  const readOnly = mode === 'view'
  const activeFields = mode === 'view' ? (viewFields ?? fields) : fields

  useEffect(() => {
    if (show) {
      setValues(initialValues)
      setErrors({})
    }
  }, [show, initialValues])

  const titleMap: Record<FormModalMode, string> = {
    create: `Thêm ${entityName}`,
    edit: `Sửa ${entityName}`,
    view: `Chi tiết ${entityName}`,
  }

  const handleChange = (name: keyof T & string, raw: string | boolean | number | number[]) => {
    setValues((prev) => ({ ...prev, [name]: raw }))
    setErrors((prev) => {
      const next = { ...prev }
      delete next[name]
      return next
    })
  }

  const validate = (): boolean => {
    const next: Partial<Record<keyof T & string, string>> = {}
    activeFields.forEach((field) => {
      if (!field.required || field.type === 'checkbox') return
      if (field.type === 'multiselect') {
        if (getMultiSelectIds(values, field.name).length === 0) {
          next[field.name] = 'Trường này là bắt buộc'
        }
        return
      }
      const val = getFieldValue(values, field.name)
      if (!val.trim()) next[field.name] = 'Trường này là bắt buộc'
    })
    setErrors(next)
    return Object.keys(next).length === 0
  }

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault()
    if (readOnly) return
    if (!validate()) return
    onSubmit(values)
  }

  return (
    <Modal show={show} onHide={onHide} centered size="lg" scrollable>
      <Form onSubmit={handleSubmit}>
        <ModalHeader closeButton>
          <ModalTitle>{titleMap[mode]}</ModalTitle>
        </ModalHeader>
        <ModalBody>
          <Row className="g-3">
            {activeFields.map((field) => (
              <Col
                key={field.name}
                xs={12}
                md={field.col ?? (field.type === 'textarea' || field.type === 'multiselect' ? 12 : 6)}>
                {field.type !== 'checkbox' && (
                  <Form.Label className="fw-semibold">
                    {resolveLabel(field)}
                    {field.required && <span className="text-danger ms-1">*</span>}
                  </Form.Label>
                )}
                <FormFieldInput
                  field={field}
                  value={getFieldValue(values, field.name)}
                  selectedIds={getMultiSelectIds(values, field.name)}
                  readOnly={readOnly}
                  onChange={handleChange}
                />
                {field.hint && <Form.Text className="text-muted">{field.hint}</Form.Text>}
                {errors[field.name] && <div className="text-danger fs-xs mt-1">{errors[field.name]}</div>}
              </Col>
            ))}
          </Row>
        </ModalBody>
        <ModalFooter>
          <Button variant="light" onClick={onHide}>
            {readOnly ? 'Đóng' : 'Hủy'}
          </Button>
          {!readOnly && (
            <Button variant="primary" type="submit">
              {mode === 'create' ? 'Tạo mới' : 'Lưu thay đổi'}
            </Button>
          )}
        </ModalFooter>
      </Form>
    </Modal>
  )
}

export default EntityFormModal
