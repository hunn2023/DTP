import { type FormEvent, useCallback, useEffect, useState } from 'react'
import { Alert, Card, Col, Form, Row } from 'react-bootstrap'

import {
  createProductPrice,
  updateProductPrice,
} from '@/apis/productPricesApi'
import type { ProductPriceRow } from '@/features/master-data/products/types'
import { useTabDirty } from '@/features/products/esim-wizard/hooks/useTabDirty'
import { useWizardTabForm } from '@/features/products/esim-wizard/hooks/useWizardTabForm'
import RequiredMark from '@/components/form/RequiredMark'
import NumberFormControl from '@/components/form/NumberFormControl'
import { getDefaultPriceValues } from '@/features/products/esim-wizard/wizardDefaults'

const NOTE_MAX = 500

type WizardPriceTabProps = {
  productId: string
  variantId: string
  initialValues: ProductPriceRow | null
  onSaved: (priceId: string) => void
  onSavingChange: (saving: boolean) => void
  onDirtyChange?: (dirty: boolean) => void
  onRegisterSave?: (fn: () => Promise<boolean>) => void
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

const WizardPriceTab = ({
  productId,
  variantId,
  initialValues,
  onSaved,
  onSavingChange,
  onDirtyChange,
  onRegisterSave,
}: WizardPriceTabProps) => {
  const [values, setValues] = useWizardTabForm(
    initialValues,
    () => getDefaultPriceValues(productId, variantId),
  )
  const [error, setError] = useState('')

  useTabDirty(values, initialValues, onDirtyChange)

  const savePrice = useCallback(async (): Promise<boolean> => {
    setError('')

    if (values.salePrice <= 0) {
      setError('Vui lòng nhập giá bán')
      return false
    }

    onSavingChange(true)
    try {
      if (!values.id) {
        const id = await createProductPrice({
          productId,
          productVariantId: variantId,
          currency: values.currency,
          originalPrice: values.originalPrice,
          salePrice: values.salePrice,
          costPrice: values.costPrice,
          startDate: values.startDate || null,
          endDate: values.endDate || null,
          note: values.note,
        })
        onSaved(id)
      } else {
        await updateProductPrice(values.id, {
          currency: values.currency,
          originalPrice: values.originalPrice,
          salePrice: values.salePrice,
          costPrice: values.costPrice,
          startDate: values.startDate || null,
          endDate: values.endDate || null,
          isActive: values.isActive,
          note: values.note,
        })
        onSaved(values.id)
      }
      return true
    } catch (err) {
      setError(getErrorMessage(err, 'Không lưu được giá'))
      return false
    } finally {
      onSavingChange(false)
    }
  }, [values, productId, variantId, onSaved, onSavingChange])

  useEffect(() => {
    onRegisterSave?.(savePrice)
  }, [onRegisterSave, savePrice])

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault()
    void savePrice()
  }

  return (
    <Form id="esim-wizard-price-form" onSubmit={handleSubmit}>
      <Card className="border shadow-none">
        <Card.Body>
          <div className="mb-3">
            <h5 className="mb-1 fw-semibold">Giá bán</h5>
            <p className="text-muted mb-0 fs-sm">Thiết lập giá hiển thị và khoảng thời gian áp dụng.</p>
          </div>
          <Row className="g-3">
            <Col md={4}>
              <Form.Label className="fw-semibold">Đơn vị tiền tệ <RequiredMark /></Form.Label>
              <Form.Select
                value={values.currency}
                onChange={(e) => setValues((p) => ({ ...p, currency: e.target.value }))}>
                <option value="VND">VND</option>
                <option value="USD">USD</option>
              </Form.Select>
            </Col>
            <Col md={4}>
              <Form.Label className="fw-semibold">Giá gốc</Form.Label>
              <NumberFormControl
                min={0}
                value={values.originalPrice}
                onChange={(originalPrice) => setValues((p) => ({ ...p, originalPrice }))}
              />
            </Col>
            <Col md={4}>
              <Form.Label className="fw-semibold">Giá bán <RequiredMark /></Form.Label>
              <NumberFormControl
                min={0}
                value={values.salePrice}
                onChange={(salePrice) => setValues((p) => ({ ...p, salePrice }))}
                required
              />
            </Col>
            <Col md={4}>
              <Form.Label className="fw-semibold">Giá vốn</Form.Label>
              <NumberFormControl
                min={0}
                value={values.costPrice}
                onChange={(costPrice) => setValues((p) => ({ ...p, costPrice }))}
              />
            </Col>
            <Col md={4}>
              <Form.Label className="fw-semibold">Từ ngày</Form.Label>
              <Form.Control
                type="date"
                value={values.startDate}
                onChange={(e) => setValues((p) => ({ ...p, startDate: e.target.value }))}
              />
            </Col>
            <Col md={4}>
              <Form.Label className="fw-semibold">Đến ngày</Form.Label>
              <Form.Control
                type="date"
                value={values.endDate}
                onChange={(e) => setValues((p) => ({ ...p, endDate: e.target.value }))}
              />
            </Col>
            <Col xs={12}>
              <div className="d-flex justify-content-between">
                <Form.Label className="fw-semibold mb-0">Ghi chú</Form.Label>
                <span className="text-muted fs-xs">
                  {values.note.length}/{NOTE_MAX}
                </span>
              </div>
              <Form.Control
                as="textarea"
                rows={3}
                maxLength={NOTE_MAX}
                value={values.note}
                onChange={(e) => setValues((p) => ({ ...p, note: e.target.value }))}
              />
            </Col>
          </Row>
        </Card.Body>
      </Card>
      {error && (
        <Alert variant="danger" className="mt-3 mb-0">
          {error}
        </Alert>
      )}
    </Form>
  )
}

export default WizardPriceTab
