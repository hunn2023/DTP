import { type FormEvent, useEffect, useState } from 'react'
import { Card, Col, Form, Row } from 'react-bootstrap'

import {
  createProductVariant,
  updateProductVariant,
} from '@/features/master-data/products/product-variants.api'
import type { ProductVariant } from '@/features/master-data/products/types'
import { getDefaultVariantValues } from '@/features/products/esim-wizard/wizardDefaults'
import type { FormFieldOption } from '@/modules/crud/form/types'
import { slugify } from '@/modules/crud/form/slugify'

const DESC_MAX = 500

type WizardVariantTabProps = {
  isNew: boolean
  productOptions: FormFieldOption[]
  initialValues: ProductVariant | null
  onSaved: (variantId: string, productId: string) => void
  onSavingChange: (saving: boolean) => void
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
}: WizardVariantTabProps) => {
  const [values, setValues] = useState<ProductVariant>(initialValues ?? getDefaultVariantValues())
  const [error, setError] = useState('')

  useEffect(() => {
    if (initialValues) setValues(initialValues)
  }, [initialValues])

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')

    if (!values.productId.trim() || !values.name.trim()) {
      setError('Vui lòng chọn sản phẩm và nhập tên variant')
      return
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
    } catch (err) {
      setError(getErrorMessage(err, 'Không lưu được variant'))
    } finally {
      onSavingChange(false)
    }
  }

  return (
    <Form id="esim-wizard-variant-form" onSubmit={(e) => void handleSubmit(e)}>
      <Row className="g-4">
        <Col lg={8}>
          <Card className="border shadow-none">
            <Card.Body>
              <h5 className="mb-3 fw-semibold">Thông tin variant</h5>
              <Row className="g-3">
                <Col xs={12}>
                  <Form.Label className="fw-semibold">Sản phẩm *</Form.Label>
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
                  <Form.Label className="fw-semibold">Tên variant *</Form.Label>
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
                  <Form.Label className="fw-semibold">Tên ngắn (hiển thị trên card)</Form.Label>
                  <Form.Control
                    value={values.shortName}
                    placeholder="VD: 1GB"
                    onChange={(e) => setValues((p) => ({ ...p, shortName: e.target.value }))}
                  />
                </Col>
                <Col xs={12}>
                  <div className="d-flex justify-content-between">
                    <Form.Label className="fw-semibold mb-0">Mô tả variant</Form.Label>
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
          <Card className="border shadow-none">
            <Card.Body>
              <h5 className="mb-3 fw-semibold">Cài đặt</h5>
              <div className="mb-3">
                <Form.Label className="fw-semibold">Thứ tự sắp xếp</Form.Label>
                <Form.Control
                  type="number"
                  min={0}
                  value={values.sortOrder}
                  onChange={(e) => setValues((p) => ({ ...p, sortOrder: Number(e.target.value) }))}
                />
              </div>
              <div>
                <Form.Label className="fw-semibold">Trạng thái</Form.Label>
                <Form.Select
                  value={values.isActive ? 'true' : 'false'}
                  onChange={(e) => setValues((p) => ({ ...p, isActive: e.target.value === 'true' }))}>
                  <option value="true">Hoạt động</option>
                  <option value="false">Ngưng hoạt động</option>
                </Form.Select>
              </div>
              {values.name && (
                <p className="text-muted fs-xs mt-3 mb-0">
                  Gợi ý slug: <code>{slugify(values.name)}</code>
                </p>
              )}
            </Card.Body>
          </Card>
        </Col>
      </Row>
      {error && <div className="text-danger mt-3">{error}</div>}
    </Form>
  )
}

export default WizardVariantTab
