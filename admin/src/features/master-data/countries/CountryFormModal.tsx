import { type FormEvent, useEffect, useState } from 'react'
import { Button, Col, Form, Modal, ModalBody, ModalFooter, ModalHeader, ModalTitle, Nav, Row, Tab } from 'react-bootstrap'

import FileUploader from '@/components/FileUploader'
import { countryFormConfig } from '@/features/master-data/countries/formConfig'
import { getCountryRegionOptions } from '@/features/master-data/countries/regionOptions'
import type { Country } from '@/features/master-data/types'
import { getFieldLabel } from '@/modules/crud/entities/fieldLabels'
import type { FormFieldConfig, FormModalMode } from '@/modules/crud/form/types'

type CountryFormModalProps = {
  show: boolean
  mode: FormModalMode
  initialValues: Country
  activeTab: 'info' | 'flag'
  countryIdForFlag: string | null
  isSaving: boolean
  onHide: () => void
  onTabChange: (tab: 'info' | 'flag') => void
  onContinueCreate: (values: Country) => void
  onSaveInfo: (values: Country) => void
  onSaveFlag: (file: File) => void
}

function getFieldValue(values: Country, name: keyof Country): string {
  const raw = values[name]
  if (typeof raw === 'boolean') return raw ? 'true' : 'false'
  if (raw === null || raw === undefined) return ''
  return String(raw)
}

function CountryFieldInput({
  field,
  value,
  readOnly,
  onChange,
}: {
  field: FormFieldConfig<Country>
  value: string
  readOnly: boolean
  onChange: (name: keyof Country, value: string | boolean | number) => void
}) {
  const label = field.label ?? getFieldLabel(field.name)
  const common = { disabled: readOnly, required: field.required && !readOnly, placeholder: field.placeholder }

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
        id={`country-field-${field.name}`}
        label={label}
        checked={value === 'true'}
        disabled={readOnly}
        onChange={(e) => onChange(field.name, e.target.checked)}
      />
    )
  }

  if (field.type === 'select') {
    const options =
      field.name === 'region' ? getCountryRegionOptions(value) : (field.options ?? [])
    return (
      <Form.Select {...common} value={value} onChange={(e) => onChange(field.name, e.target.value)}>
        <option value="">-- Chọn khu vực --</option>
        {options.map((opt) => (
          <option key={opt.value} value={opt.value}>
            {opt.label}
          </option>
        ))}
      </Form.Select>
    )
  }

  return (
    <Form.Control
      type={field.type === 'number' ? 'number' : 'text'}
      {...common}
      value={value}
      onChange={(e) => onChange(field.name, field.type === 'number' ? Number(e.target.value) : e.target.value)}
    />
  )
}

const CountryFormModal = ({
  show,
  mode,
  initialValues,
  activeTab,
  countryIdForFlag,
  isSaving,
  onHide,
  onTabChange,
  onContinueCreate,
  onSaveInfo,
  onSaveFlag,
}: CountryFormModalProps) => {
  const [values, setValues] = useState<Country>(initialValues)
  const [errors, setErrors] = useState<Partial<Record<keyof Country, string>>>({})
  const [flagFiles, setFlagFiles] = useState<File[] | undefined>()
  const readOnly = mode === 'view'
  const fields = countryFormConfig.fields

  useEffect(() => {
    if (show) {
      setValues(initialValues)
      setErrors({})
      setFlagFiles(undefined)
    }
  }, [show, initialValues])

  const titleMap: Record<FormModalMode, string> = {
    create: 'Thêm quốc gia',
    edit: 'Sửa quốc gia',
    view: 'Chi tiết quốc gia',
  }

  const handleChange = (name: keyof Country, raw: string | boolean | number) => {
    setValues((prev) => ({ ...prev, [name]: raw }))
    setErrors((prev) => {
      const next = { ...prev }
      delete next[name]
      return next
    })
  }

  const validateInfo = (): boolean => {
    const next: Partial<Record<keyof Country, string>> = {}
    fields.forEach((field) => {
      if (!field.required || field.type === 'checkbox') return
      const val = getFieldValue(values, field.name)
      if (!val.trim()) next[field.name] = 'Trường này là bắt buộc'
    })
    setErrors(next)
    return Object.keys(next).length === 0
  }

  const handleInfoSubmit = (e: FormEvent) => {
    e.preventDefault()
    if (readOnly) return
    if (!validateInfo()) return
    if (mode === 'create') {
      onContinueCreate(values)
      return
    }
    onSaveInfo(values)
  }

  const handleFlagSubmit = (e: FormEvent) => {
    e.preventDefault()
    if (readOnly) return
    const file = flagFiles?.[0]
    if (!file) {
      setErrors((prev) => ({ ...prev, flagUrl: 'Vui lòng chọn ảnh cờ' }))
      return
    }
    onSaveFlag(file)
  }

  const canOpenFlagTab = mode !== 'create' || Boolean(countryIdForFlag)

  return (
    <Modal show={show} onHide={onHide} centered size="lg" scrollable>
      <ModalHeader closeButton>
        <ModalTitle>{titleMap[mode]}</ModalTitle>
      </ModalHeader>
      <ModalBody>
        <Tab.Container activeKey={activeTab} onSelect={(key) => key && onTabChange(key as 'info' | 'flag')}>
          <Nav variant="tabs" className="mb-3">
            <Nav.Item>
              <Nav.Link eventKey="info">Thông tin</Nav.Link>
            </Nav.Item>
            <Nav.Item>
              <Nav.Link eventKey="flag" disabled={!canOpenFlagTab}>
                Cờ quốc gia
              </Nav.Link>
            </Nav.Item>
          </Nav>

          <Tab.Content>
            <Tab.Pane eventKey="info">
              <Form id="country-info-form" onSubmit={handleInfoSubmit}>
                <Row className="g-3">
                  {fields.map((field) => (
                    <Col
                      key={field.name}
                      xs={12}
                      md={field.col ?? (field.type === 'textarea' ? 12 : 6)}>
                      {field.type !== 'checkbox' && (
                        <Form.Label className="fw-semibold">
                          {field.label ?? getFieldLabel(field.name)}
                          {field.required && <span className="text-danger ms-1">*</span>}
                        </Form.Label>
                      )}
                      <CountryFieldInput
                        field={field}
                        value={getFieldValue(values, field.name)}
                        readOnly={readOnly}
                        onChange={handleChange}
                      />
                      {field.hint && <Form.Text className="text-muted">{field.hint}</Form.Text>}
                      {errors[field.name] && <div className="text-danger fs-xs mt-1">{errors[field.name]}</div>}
                    </Col>
                  ))}
                </Row>
              </Form>
            </Tab.Pane>

            <Tab.Pane eventKey="flag">
              {readOnly ? (
                values.flagUrl ? (
                  <img src={values.flagUrl} alt={values.name} className="rounded border" width={120} height={80} style={{ objectFit: 'cover' }} />
                ) : (
                  <p className="text-muted mb-0">Chưa có ảnh cờ</p>
                )
              ) : (
                <Form id="country-flag-form" onSubmit={handleFlagSubmit}>
                  {values.flagUrl && (
                    <div className="mb-3">
                      <Form.Label className="fw-semibold">Cờ hiện tại</Form.Label>
                      <div>
                        <img src={values.flagUrl} alt={values.name} className="rounded border" width={80} height={56} style={{ objectFit: 'cover' }} />
                      </div>
                    </div>
                  )}
                  <Form.Label className="fw-semibold">Tải lên ảnh cờ</Form.Label>
                  <FileUploader
                    files={flagFiles}
                    setFiles={setFlagFiles}
                    accept={{ 'image/*': ['.png', '.jpg', '.jpeg', '.webp'] }}
                    maxSize={10 * 1024 * 1024}
                    maxFileCount={1}
                  />
                  {errors.flagUrl && <div className="text-danger fs-xs mt-1">{errors.flagUrl}</div>}
                </Form>
              )}
            </Tab.Pane>
          </Tab.Content>
        </Tab.Container>
      </ModalBody>

      <ModalFooter>
        <Button variant="light" onClick={onHide} disabled={isSaving}>
          {readOnly ? 'Đóng' : 'Hủy'}
        </Button>
        {!readOnly && activeTab === 'info' && (
          <Button variant="primary" type="submit" form="country-info-form" disabled={isSaving}>
            {mode === 'create' ? (isSaving ? 'Đang tạo...' : 'Tiếp tục') : isSaving ? 'Đang lưu...' : 'Lưu thay đổi'}
          </Button>
        )}
        {!readOnly && activeTab === 'flag' && (
          <Button variant="primary" type="submit" form="country-flag-form" disabled={isSaving || !canOpenFlagTab}>
            {isSaving ? 'Đang tải lên...' : 'Lưu'}
          </Button>
        )}
      </ModalFooter>
    </Modal>
  )
}

export default CountryFormModal
