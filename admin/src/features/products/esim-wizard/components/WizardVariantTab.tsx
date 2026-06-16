import { type FormEvent, useCallback, useEffect, useState } from 'react'
import { Alert, Card, Col, Form, Row } from 'react-bootstrap'

import { createProductVariant, updateProductVariant } from '@/apis/productVariantsApi'
import type { ProductVariant } from '@/features/master-data/products/types'
import { useTabDirty } from '@/features/products/esim-wizard/hooks/useTabDirty'
import RequiredMark from '@/components/form/RequiredMark'
import NumberFormControl from '@/components/form/NumberFormControl'
import { getDefaultVariantValues } from '@/features/products/esim-wizard/wizardDefaults'
import { slugify } from '@/modules/crud/form/slugify'
import type { FormFieldOption } from '@/modules/crud/form/types'

const DESC_MAX = 500

type WizardVariantTabProps = {
  isNew: boolean
  productOptions: FormFieldOption[]
  initialValues: ProductVariant | null
  onSaved: (variantId: string, productId: string) => void
  onSavingChange: (saving: boolean) => void
  onDirtyChange?: (dirty: boolean) => void
  onRegisterSave?: (fn: () => Promise<boolean>) => void
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

const WizardVariantTab = ({
  isNew,
  productOptions,
  initialValues,
  onSaved,
  onSavingChange,
  onDirtyChange,
  onRegisterSave,
}: WizardVariantTabProps) => {
  const [values, setValues] = useState<ProductVariant>(initialValues ?? getDefaultVariantValues())
  const [error, setError] = useState('')

  useEffect(() => {
    if (initialValues) setValues(initialValues)
  }, [initialValues])

  useTabDirty(values, initialValues, onDirtyChange)

  const saveVariant = useCallback(async (): Promise<boolean> => {
    setError('')

    if (!values.productId.trim() || !values.name.trim()) {
      setError('Vui lòng chọn sản phẩm và nhập tên biến thể')
      return false
    }

    const payload = {
      productId: values.productId.trim(),
      sku: values.sku.trim() || undefined,
      name: values.name.trim(),
      shortName: values.shortName.trim() || undefined,
      description: values.description.trim() || undefined,
      sortOrder: values.sortOrder,
      isActive: values.isActive,
    }

    onSavingChange(true)
    try {
      if (isNew || !values.id) {
        const id = await createProductVariant(payload)
        onSaved(id, payload.productId)
      } else {
        await updateProductVariant(values.id, payload)
        onSaved(values.id, payload.productId)
      }
      return true
    } catch (err) {
      setError(getErrorMessage(err, 'Không lưu được biến thể'))
      return false
    } finally {
      onSavingChange(false)
    }
  }, [values, isNew, onSaved, onSavingChange])

  useEffect(() => {
    onRegisterSave?.(saveVariant)
  }, [onRegisterSave, saveVariant])

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault()
    void saveVariant()
  }

  return (
    <Form id="esim-wizard-variant-form" onSubmit={handleSubmit}>
      <Row className="g-3">
        <Col lg={8}>
          <Card className="border shadow-none h-100">
            <Card.Body>
              <div className="mb-3">
                <h5 className="mb-1 fw-semibold">Thông tin biến thể</h5>
                <p className="text-muted mb-0 fs-sm">Đây là lớp liên kết giữa sản phẩm và gói eSIM bán ra.</p>
              </div>
              <Row className="g-3">
                <Col xs={12}>
                  <Form.Label className="fw-semibold">Sản phẩm <RequiredMark /></Form.Label>
                  <Form.Select
                    value={values.productId}
                    disabled={!isNew && Boolean(values.id)}
                    onChange={(e) => setValues((p) => ({ ...p, productId: e.target.value }))}
                    required>
                    <option value="">-- Chọn sản phẩm --</option>
                    {productOptions.map((opt) => (
                      <option key={opt.value} value={opt.value}>
                        {opt.label}
                      </option>
                    ))}
                  </Form.Select>
                </Col>
                <Col md={6}>
                  <Form.Label className="fw-semibold">Tên biến thể <RequiredMark /></Form.Label>
                  <Form.Control
                    value={values.name}
                    placeholder="VD: 1GB / 1 ngày"
                    onChange={(e) => setValues((p) => ({ ...p, name: e.target.value }))}
                    required
                  />
                </Col>
                <Col md={6}>
                  <Form.Label className="fw-semibold">SKU</Form.Label>
                  <Form.Control
                    value={values.sku}
                    placeholder="ESIM_JP_1GB_1D"
                    onChange={(e) => setValues((p) => ({ ...p, sku: e.target.value }))}
                  />
                </Col>
                <Col xs={12}>
                  <Form.Label className="fw-semibold">Tên ngắn trên card</Form.Label>
                  <Form.Control
                    value={values.shortName}
                    placeholder="VD: 1GB"
                    onChange={(e) => setValues((p) => ({ ...p, shortName: e.target.value }))}
                  />
                </Col>
                <Col xs={12}>
                  <div className="d-flex justify-content-between">
                    <Form.Label className="fw-semibold mb-0">Mô tả biến thể</Form.Label>
                    <span className="text-muted fs-xs">
                      {values.description.length}/{DESC_MAX}
                    </span>
                  </div>
                  <Form.Control
                    as="textarea"
                    rows={3}
                    maxLength={DESC_MAX}
                    value={values.description}
                    onChange={(e) => setValues((p) => ({ ...p, description: e.target.value }))}
                  />
                </Col>
              </Row>
            </Card.Body>
          </Card>
        </Col>
        <Col lg={4}>
          <Card className="border shadow-none h-100">
            <Card.Body>
              <h5 className="mb-3 fw-semibold">Cài đặt hiển thị</h5>
              <div className="mb-3">
                <Form.Label className="fw-semibold">Thứ tự sắp xếp</Form.Label>
                <NumberFormControl
                  min={0}
                  value={values.sortOrder}
                  emptyWhenZero={false}
                  onChange={(sortOrder) => setValues((p) => ({ ...p, sortOrder }))}
                />
              </div>
              <div>
                <Form.Label className="fw-semibold">Trạng thái</Form.Label>
                <Form.Select
                  value={values.isActive ? 'true' : 'false'}
                  onChange={(e) => setValues((p) => ({ ...p, isActive: e.target.value === 'true' }))}>
                  <option value="true">Hoạt động</option>
                  <option value="false">Ngừng hoạt động</option>
                </Form.Select>
              </div>
              {values.name && (
                <div className="rounded bg-light p-3 mt-3">
                  <div className="text-muted fs-xs mb-1">Gợi ý slug</div>
                  <code>{slugify(values.name)}</code>
                </div>
              )}
            </Card.Body>
          </Card>
        </Col>
      </Row>
      {error && (
        <Alert variant="danger" className="mt-3 mb-0">
          {error}
        </Alert>
      )}
    </Form>
  )
}

export default WizardVariantTab
