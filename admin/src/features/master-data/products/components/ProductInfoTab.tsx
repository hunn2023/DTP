import { type FormEvent, useEffect, useMemo, useState } from 'react'
import { Card, Col, Form, Row } from 'react-bootstrap'

import { getDefaultProductValues, toProductPayload } from '@/features/master-data/products/formConfig'
import { createProduct, updateProduct } from '@/apis/productsApi'
import type { CatalogProduct } from '@/features/master-data/products/types'
import type { Country } from '@/features/master-data/types'
import type { FormFieldOption } from '@/modules/crud/form/types'
import { slugify } from '@/modules/crud/form/slugify'
import RequiredMark from '@/components/form/RequiredMark'

const SHORT_DESC_MAX = 255
const DETAIL_DESC_MAX = 2000

type ProductInfoTabProps = {
  productId: string | null
  initialValues?: CatalogProduct | null
  categoryOptions: FormFieldOption[]
  countries: Country[]
  isNew: boolean
  onCreated: (id: string) => void
  onSaved: () => void
  onSavingChange: (saving: boolean) => void
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

function CharCounter({ current, max }: { current: number; max: number }) {
  return (
    <span className={`fs-xs ${current > max ? 'text-danger' : 'text-muted'}`}>
      {current}/{max}
    </span>
  )
}

const ProductInfoTab = ({
  productId,
  initialValues,
  categoryOptions,
  countries,
  isNew,
  onCreated,
  onSaved,
  onSavingChange,
}: ProductInfoTabProps) => {
  const [values, setValues] = useState<CatalogProduct>(initialValues ?? getDefaultProductValues())
  const [error, setError] = useState('')

  useEffect(() => {
    if (initialValues) setValues(initialValues)
  }, [initialValues])

  const selectedCountry = useMemo(
    () => countries.find((item) => item.id === values.countryId),
    [countries, values.countryId],
  )

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')

    if (!values.name.trim() || !values.slug.trim() || !values.categoryId.trim()) {
      setError('Vui lòng điền tên, slug và danh mục')
      return
    }
    if (values.shortDescription.length > SHORT_DESC_MAX || values.description.length > DETAIL_DESC_MAX) {
      setError('Mô tả vượt quá giới hạn ký tự')
      return
    }

    const payloadValues = {
      ...values,
      slug: values.slug.trim() || slugify(values.name),
    }

    onSavingChange(true)
    try {
      if (isNew) {
        const id = await createProduct(toProductPayload(payloadValues, true))
        onCreated(id)
      } else if (productId) {
        await updateProduct(productId, toProductPayload(payloadValues, false))
        onSaved()
      }
    } catch (err) {
      setError(getErrorMessage(err, 'Không lưu được sản phẩm'))
    } finally {
      onSavingChange(false)
    }
  }

  return (
    <Form id="product-info-form" onSubmit={(e) => void handleSubmit(e)}>
      <Row className="g-4">
        <Col lg={8}>
          <Card className="border shadow-none h-100">
            <Card.Body>
              <h5 className="mb-3 fw-semibold">Thông tin cơ bản</h5>
              <Row className="g-3">
                <Col xs={12}>
                  <Form.Label className="fw-semibold">Tên sản phẩm <RequiredMark /></Form.Label>
                  <Form.Control
                    value={values.name}
                    placeholder="VD: eSIM Nhật Bản"
                    onChange={(e) => {
                      const name = e.target.value
                      setValues((p) => ({
                        ...p,
                        name,
                        slug: p.slug || slugify(name),
                      }))
                    }}
                    required
                  />
                </Col>
                <Col xs={12}>
                  <Form.Label className="fw-semibold">Slug <RequiredMark /></Form.Label>
                  <Form.Control
                    value={values.slug}
                    placeholder="esim-nhat-ban"
                    onChange={(e) => setValues((p) => ({ ...p, slug: e.target.value }))}
                    required
                  />
                </Col>
                <Col md={6}>
                  <Form.Label className="fw-semibold">Danh mục <RequiredMark /></Form.Label>
                  <Form.Select
                    value={values.categoryId}
                    onChange={(e) => setValues((p) => ({ ...p, categoryId: e.target.value }))}
                    required>
                    <option value="">-- Chọn danh mục --</option>
                    {categoryOptions.map((opt) => (
                      <option key={opt.value} value={opt.value}>
                        {opt.label}
                      </option>
                    ))}
                  </Form.Select>
                </Col>
                <Col md={6}>
                  <Form.Label className="fw-semibold">Quốc gia</Form.Label>
                  <div className="position-relative">
                    {selectedCountry?.flagUrl && (
                      <img
                        src={selectedCountry.flagUrl}
                        alt=""
                        className="position-absolute rounded"
                        style={{ width: 20, height: 14, left: 12, top: '50%', transform: 'translateY(-50%)', objectFit: 'cover' }}
                      />
                    )}
                    <Form.Select
                      value={values.countryId}
                      className={selectedCountry?.flagUrl ? 'ps-5' : undefined}
                      onChange={(e) => setValues((p) => ({ ...p, countryId: e.target.value }))}>
                      <option value="">-- Không chọn --</option>
                      {countries.map((item) => (
                        <option key={item.id} value={item.id}>
                          {item.name}
                        </option>
                      ))}
                    </Form.Select>
                  </div>
                </Col>
                <Col xs={12}>
                  <div className="d-flex justify-content-between align-items-center">
                    <Form.Label className="fw-semibold mb-0">Mô tả ngắn</Form.Label>
                    <CharCounter current={values.shortDescription.length} max={SHORT_DESC_MAX} />
                  </div>
                  <Form.Control
                    as="textarea"
                    rows={3}
                    maxLength={SHORT_DESC_MAX}
                    value={values.shortDescription}
                    placeholder="Mô tả ngắn hiển thị trên danh sách sản phẩm"
                    onChange={(e) => setValues((p) => ({ ...p, shortDescription: e.target.value }))}
                  />
                </Col>
                <Col xs={12}>
                  <div className="d-flex justify-content-between align-items-center">
                    <Form.Label className="fw-semibold mb-0">Mô tả chi tiết</Form.Label>
                    <CharCounter current={values.description.length} max={DETAIL_DESC_MAX} />
                  </div>
                  <Form.Control
                    as="textarea"
                    rows={5}
                    maxLength={DETAIL_DESC_MAX}
                    value={values.description}
                    placeholder="Mô tả đầy đủ về sản phẩm"
                    onChange={(e) => setValues((p) => ({ ...p, description: e.target.value }))}
                  />
                </Col>
                <Col xs={12}>
                  <Form.Label className="fw-semibold">Khu vực hiển thị</Form.Label>
                  <Form.Control
                    value={values.locationText}
                    placeholder="VD: Tokyo, Osaka, Kyoto"
                    onChange={(e) => setValues((p) => ({ ...p, locationText: e.target.value }))}
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
              <div className="d-flex flex-column gap-3">
                <div className="d-flex align-items-center justify-content-between">
                  <div>
                    <div className="fw-semibold">Nổi bật</div>
                    <div className="text-muted fs-xs">Hiển thị ở mục nổi bật</div>
                  </div>
                  <Form.Check
                    type="switch"
                    id="product-isFeatured"
                    checked={values.isFeatured}
                    onChange={(e) => setValues((p) => ({ ...p, isFeatured: e.target.checked }))}
                  />
                </div>
                <div className="d-flex align-items-center justify-content-between">
                  <div>
                    <div className="fw-semibold">HOT</div>
                    <div className="text-muted fs-xs">Gắn nhãn HOT</div>
                  </div>
                  <Form.Check
                    type="switch"
                    id="product-isHot"
                    checked={values.isHot}
                    onChange={(e) => setValues((p) => ({ ...p, isHot: e.target.checked }))}
                  />
                </div>
                <div>
                  <Form.Label className="fw-semibold">Mã (Code)</Form.Label>
                  <Form.Control
                    value={values.code}
                    placeholder="Mã sản phẩm"
                    onChange={(e) => setValues((p) => ({ ...p, code: e.target.value }))}
                  />
                </div>
                <div>
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
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      {error && <div className="text-danger mt-3">{error}</div>}
    </Form>
  )
}

export default ProductInfoTab
