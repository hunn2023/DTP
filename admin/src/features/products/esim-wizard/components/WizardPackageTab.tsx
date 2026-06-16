import { type FormEvent, useCallback, useEffect, useState } from 'react'
import { Alert, Card, Col, Form, Row } from 'react-bootstrap'

import { createEsimPackage, updateEsimPackage } from '@/apis/esimPackagesApi'
import CountrySearchSelect from '@/features/master-data/countries/components/CountrySearchSelect'
import RequiredMark from '@/components/form/RequiredMark'
import NumberFormControl from '@/components/form/NumberFormControl'
import { useTabDirty } from '@/features/products/esim-wizard/hooks/useTabDirty'
import { useWizardTabForm } from '@/features/products/esim-wizard/hooks/useWizardTabForm'
import { toWizardPackagePayload } from '@/features/products/esim-wizard/mapPackageForm'
import type { EsimPackageForm } from '@/features/products/esim-wizard/types'
import { getDefaultPackageValues } from '@/features/products/esim-wizard/wizardDefaults'
import { slugify } from '@/modules/crud/form/slugify'

type WizardPackageTabProps = {
  productId: string
  variantId: string
  defaultCountryId: string
  initialValues: EsimPackageForm | null
  onSaved: (packageId: string) => void
  onSavingChange: (saving: boolean) => void
  onDirtyChange?: (dirty: boolean) => void
  onRegisterSave?: (fn: () => Promise<boolean>) => void
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

const WizardPackageTab = ({
  productId,
  variantId,
  defaultCountryId,
  initialValues,
  onSaved,
  onSavingChange,
  onDirtyChange,
  onRegisterSave,
}: WizardPackageTabProps) => {
  const [values, setValues] = useWizardTabForm(
    initialValues,
    () => getDefaultPackageValues(productId, variantId, defaultCountryId),
  )
  const [error, setError] = useState('')

  useTabDirty(values, initialValues, onDirtyChange)

  const savePackage = useCallback(async (): Promise<boolean> => {
    setError('')

    if (!values.name.trim() || !values.countryId) {
      setError('Vui lòng điền tên gói và quốc gia')
      return false
    }

    const payload = toWizardPackagePayload({ ...values, productId, productVariantId: variantId })

    onSavingChange(true)
    try {
      if (!values.id) {
        const id = await createEsimPackage(payload)
        onSaved(id)
      } else {
        await updateEsimPackage(values.id, payload)
        onSaved(values.id)
      }
      return true
    } catch (err) {
      setError(getErrorMessage(err, 'Không lưu được gói eSIM'))
      return false
    } finally {
      onSavingChange(false)
    }
  }, [values, productId, variantId, onSaved, onSavingChange])

  useEffect(() => {
    onRegisterSave?.(savePackage)
  }, [onRegisterSave, savePackage])

  const handleSubmit = (e: FormEvent) => {
    e.preventDefault()
    void savePackage()
  }

  return (
    <Form id="esim-wizard-package-form" onSubmit={handleSubmit}>
      <Row className="g-3">
        <Col xl={8}>
          <Card className="border shadow-none h-100">
            <Card.Body>
              <div className="mb-3">
                <h5 className="mb-1 fw-semibold">Thông tin gói</h5>
                <p className="text-muted mb-0 fs-sm">Tên gói, vùng phủ sóng và thông số data.</p>
              </div>
              <Row className="g-3">
                <Col md={6}>
                  <Form.Label className="fw-semibold">Tên gói eSIM <RequiredMark /></Form.Label>
                  <Form.Control
                    value={values.name}
                    onChange={(e) =>
                      setValues((p) => ({
                        ...p,
                        name: e.target.value,
                        slug: p.slug || slugify(e.target.value),
                      }))
                    }
                    required
                  />
                </Col>
                <Col md={6}>
                  <Form.Label className="fw-semibold">Slug <RequiredMark /></Form.Label>
                  <Form.Control
                    value={values.slug}
                    onChange={(e) => setValues((p) => ({ ...p, slug: e.target.value }))}
                    required
                  />
                </Col>
                <Col md={6}>
                  <Form.Label className="fw-semibold">Quốc gia <RequiredMark /></Form.Label>
                  <CountrySearchSelect
                    value={values.countryId}
                    onChange={(countryId) => setValues((p) => ({ ...p, countryId }))}
                  />
                </Col>
                <Col md={6}>
                  <Form.Label className="fw-semibold">Loại phủ sóng</Form.Label>
                  <Form.Select
                    value={values.coverageType}
                    onChange={(e) => setValues((p) => ({ ...p, coverageType: e.target.value }))}>
                    <option value="Country">Quốc gia</option>
                    <option value="Region">Khu vực</option>
                    <option value="Global">Toàn cầu</option>
                  </Form.Select>
                </Col>
                <Col md={4}>
                  <Form.Label className="fw-semibold">Dung lượng</Form.Label>
                  <NumberFormControl
                    min={0}
                    disabled={values.isUnlimited}
                    value={values.dataAmount ?? 0}
                    emptyWhenZero={false}
                    onChange={(dataAmount) => setValues((p) => ({ ...p, dataAmount }))}
                  />
                </Col>
                <Col md={4}>
                  <Form.Label className="fw-semibold">Đơn vị</Form.Label>
                  <Form.Select
                    value={values.dataUnit}
                    onChange={(e) => setValues((p) => ({ ...p, dataUnit: e.target.value }))}>
                    <option value="GB">GB</option>
                    <option value="MB">MB</option>
                  </Form.Select>
                </Col>
                <Col md={4}>
                  <Form.Label className="fw-semibold">Số ngày <RequiredMark /></Form.Label>
                  <NumberFormControl
                    min={1}
                    value={values.validityDays}
                    emptyWhenZero={false}
                    onChange={(validityDays) => setValues((p) => ({ ...p, validityDays }))}
                  />
                </Col>
                <Col xs={12}>
                  <Form.Check
                    type="switch"
                    id="pkg-unlimited"
                    label="Không giới hạn dung lượng"
                    checked={values.isUnlimited}
                    onChange={(e) => setValues((p) => ({ ...p, isUnlimited: e.target.checked }))}
                  />
                </Col>
                <Col xs={12}>
                  <Form.Label className="fw-semibold">Mô tả phủ sóng</Form.Label>
                  <Form.Control
                    as="textarea"
                    rows={2}
                    value={values.coverageDescription}
                    onChange={(e) => setValues((p) => ({ ...p, coverageDescription: e.target.value }))}
                  />
                </Col>
              </Row>
            </Card.Body>
          </Card>
        </Col>
        <Col xl={4}>
          <Card className="border shadow-none h-100">
            <Card.Body>
              <h5 className="mb-3 fw-semibold">Chính sách & tính năng</h5>
              <div className="d-flex flex-column gap-3">
                <div>
                  <Form.Label className="fw-semibold">Chính sách kích hoạt</Form.Label>
                  <Form.Select
                    value={values.activationPolicy}
                    onChange={(e) => setValues((p) => ({ ...p, activationPolicy: e.target.value }))}>
                    <option value="FirstUse">Kích hoạt khi dùng lần đầu</option>
                    <option value="Immediate">Kích hoạt ngay</option>
                  </Form.Select>
                </div>
                <div>
                  <Form.Label className="fw-semibold">Tốc độ mạng</Form.Label>
                  <Form.Select
                    value={values.speedPolicy}
                    onChange={(e) => setValues((p) => ({ ...p, speedPolicy: e.target.value }))}>
                    <option value="4G LTE">4G LTE</option>
                    <option value="5G">5G</option>
                    <option value="3G">3G</option>
                  </Form.Select>
                </div>
                <div className="rounded bg-light p-3 d-flex flex-column gap-2">
                  <Form.Check
                    type="switch"
                    id="pkg-hotspot"
                    label="Hỗ trợ Hotspot"
                    checked={values.hotspotSupported}
                    onChange={(e) => setValues((p) => ({ ...p, hotspotSupported: e.target.checked }))}
                  />
                  <Form.Check
                    type="switch"
                    id="pkg-phone"
                    label="Hỗ trợ số điện thoại"
                    checked={values.phoneNumberSupported}
                    onChange={(e) => setValues((p) => ({ ...p, phoneNumberSupported: e.target.checked }))}
                  />
                  <Form.Check
                    type="switch"
                    id="pkg-sms"
                    label="Hỗ trợ SMS"
                    checked={values.smsSupported}
                    onChange={(e) => setValues((p) => ({ ...p, smsSupported: e.target.checked }))}
                  />
                  <Form.Check
                    type="switch"
                    id="pkg-kyc"
                    label="Yêu cầu KYC"
                    checked={values.kycRequired}
                    onChange={(e) => setValues((p) => ({ ...p, kycRequired: e.target.checked }))}
                  />
                </div>
                <div>
                  <Form.Label className="fw-semibold">Hình thức giao QR</Form.Label>
                  <Form.Select
                    value={values.qrDeliveryType}
                    onChange={(e) => setValues((p) => ({ ...p, qrDeliveryType: e.target.value }))}>
                    <option value="Automatic">Tự động</option>
                    <option value="Manual">Thủ công</option>
                  </Form.Select>
                </div>
                <div>
                  <Form.Label className="fw-semibold">Thứ tự</Form.Label>
                  <NumberFormControl
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
                    <option value="false">Ngừng</option>
                  </Form.Select>
                </div>
              </div>
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

export default WizardPackageTab
