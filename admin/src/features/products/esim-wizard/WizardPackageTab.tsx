import { type FormEvent, useEffect, useState } from 'react'
import { Alert, Card, Col, Form, Row } from 'react-bootstrap'

import { createEsimPackage, updateEsimPackage } from '@/features/products/esim-packages/esim-packages.api'
import { toPackagePayload } from '@/features/products/esim-wizard/mapPackageForm'
import type { EsimPackageForm } from '@/features/products/esim-wizard/types'
import { getDefaultPackageValues } from '@/features/products/esim-wizard/wizardDefaults'
import { slugify } from '@/modules/crud/form/slugify'
import type { FormFieldOption } from '@/modules/crud/form/types'

type WizardPackageTabProps = {
  productId: string
  variantId: string
  defaultCountryId: string
  providerOptions: FormFieldOption[]
  countryOptions: FormFieldOption[]
  initialValues: EsimPackageForm | null
  onSaved: (packageId: string) => void
  onSavingChange: (saving: boolean) => void
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

const WizardPackageTab = ({
  productId,
  variantId,
  defaultCountryId,
  providerOptions,
  countryOptions,
  initialValues,
  onSaved,
  onSavingChange,
}: WizardPackageTabProps) => {
  const [values, setValues] = useState<EsimPackageForm>(
    initialValues ?? getDefaultPackageValues(productId, variantId, defaultCountryId),
  )
  const [error, setError] = useState('')

  useEffect(() => {
    if (initialValues) setValues(initialValues)
  }, [initialValues])

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')

    if (!values.name.trim() || !values.providerId || !values.countryId || !values.providerPackageCode.trim()) {
      setError('Vui lòng điền tên gói, nhà cung cấp, quốc gia và mã gói')
      return
    }

    const payload = toPackagePayload({ ...values, productId, productVariantId: variantId })

    onSavingChange(true)
    try {
      if (!values.id) {
        const id = await createEsimPackage(payload)
        onSaved(id)
      } else {
        await updateEsimPackage(values.id, payload)
        onSaved(values.id)
      }
    } catch (err) {
      setError(getErrorMessage(err, 'Không lưu được gói eSIM'))
    } finally {
      onSavingChange(false)
    }
  }

  return (
    <Form id="esim-wizard-package-form" onSubmit={(e) => void handleSubmit(e)}>
      <Row className="g-3">
        <Col xl={8}>
          <Card className="border shadow-none h-100">
            <Card.Body>
              <div className="mb-3">
                <h5 className="mb-1 fw-semibold">Thông tin gói</h5>
                <p className="text-muted mb-0 fs-sm">Tên gói, nguồn cung cấp, vùng phủ sóng và thông số data.</p>
              </div>
              <Row className="g-3">
                <Col md={6}>
                  <Form.Label className="fw-semibold">Tên gói eSIM *</Form.Label>
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
                  <Form.Label className="fw-semibold">Slug *</Form.Label>
                  <Form.Control
                    value={values.slug}
                    onChange={(e) => setValues((p) => ({ ...p, slug: e.target.value }))}
                    required
                  />
                </Col>
                <Col md={6}>
                  <Form.Label className="fw-semibold">Mã gói nhà cung cấp *</Form.Label>
                  <Form.Control
                    value={values.providerPackageCode}
                    onChange={(e) => setValues((p) => ({ ...p, providerPackageCode: e.target.value }))}
                    required
                  />
                </Col>
                <Col md={6}>
                  <Form.Label className="fw-semibold">Nhà cung cấp *</Form.Label>
                  <Form.Select
                    value={values.providerId}
                    onChange={(e) => setValues((p) => ({ ...p, providerId: e.target.value }))}
                    required>
                    <option value="">-- Chọn --</option>
                    {providerOptions.map((opt) => (
                      <option key={opt.value} value={opt.value}>
                        {opt.label}
                      </option>
                    ))}
                  </Form.Select>
                </Col>
                <Col md={6}>
                  <Form.Label className="fw-semibold">Quốc gia *</Form.Label>
                  <Form.Select
                    value={values.countryId}
                    onChange={(e) => setValues((p) => ({ ...p, countryId: e.target.value }))}
                    required>
                    <option value="">-- Chọn --</option>
                    {countryOptions.map((opt) => (
                      <option key={opt.value} value={opt.value}>
                        {opt.label}
                      </option>
                    ))}
                  </Form.Select>
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
                  <Form.Control
                    type="number"
                    min={0}
                    disabled={values.isUnlimited}
                    value={values.dataAmount ?? 0}
                    onChange={(e) => setValues((p) => ({ ...p, dataAmount: Number(e.target.value) }))}
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
                  <Form.Label className="fw-semibold">Số ngày *</Form.Label>
                  <Form.Control
                    type="number"
                    min={1}
                    value={values.validityDays}
                    onChange={(e) => setValues((p) => ({ ...p, validityDays: Number(e.target.value) }))}
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
                  <Form.Control
                    type="number"
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
