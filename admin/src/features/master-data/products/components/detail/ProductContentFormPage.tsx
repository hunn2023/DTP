import { Button, Card, CardBody, CardHeader, Col, Form, Row, Spinner } from 'react-bootstrap'
import { LuPlus } from 'react-icons/lu'
import { TbChevronLeft, TbInfoCircle, TbSettings } from 'react-icons/tb'

import RichHtmlEditor from '@/features/content/shared/RichHtmlEditor'
import { PRODUCT_CONTENT_TYPE_OPTIONS } from '@/features/master-data/products/components/detail/contentFormConfig'
import { useProductContentFormPage } from '@/features/master-data/products/components/detail/useProductContentFormPage'

import './product-contents-grid.scss'

type Props = {
  productId: string
  contentId: string
  productName?: string
  onCancel: () => void
  onSaved: () => void
}

const ProductContentFormPage = ({ productId, contentId, productName, onCancel, onSaved }: Props) => {
  const form = useProductContentFormPage({ productId, contentId, onCancel, onSaved })

  if (form.isLoading) {
    return (
      <div className="text-center py-5">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang tải nội dung...
      </div>
    )
  }

  if (form.loadFailed) {
    return (
      <div className="text-center py-5">
        <p className="text-muted mb-3">Không tìm thấy nội dung.</p>
        <Button variant="light" onClick={onCancel}>
          <TbChevronLeft className="me-1" />
          Quay lại danh sách
        </Button>
      </div>
    )
  }

  return (
    <div>
      <div className="d-flex align-items-center justify-content-between gap-3 mb-3">
        <div>
          <Button variant="link" className="p-0 text-muted text-decoration-none mb-1" onClick={onCancel}>
            <TbChevronLeft className="me-1" />
            Quay lại danh sách nội dung
          </Button>
          <h5 className="fw-bold mb-0">{form.pageTitle}</h5>
          {productName ? <p className="text-muted mb-0 fs-sm">Sản phẩm: {productName}</p> : null}
        </div>
      </div>

      <Form
        onSubmit={(e) => {
          e.preventDefault()
          void form.submit()
        }}>
        <Row className="g-4 product-content-form-layout">
          <Col xs={12} className="product-content-form-main">
            <Card className="mb-4">
              <CardHeader className="border-light d-flex align-items-center gap-2">
                <TbInfoCircle />
                <h5 className="card-title mb-0">Thông tin nội dung</h5>
              </CardHeader>
              <CardBody>
                <Row className="g-3">
                  <Col md={8}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">
                        Tiêu đề <span className="text-danger">*</span>
                      </Form.Label>
                      <Form.Control
                        value={form.values.title}
                        isInvalid={Boolean(form.errors.title)}
                        onChange={(e) => form.updateField('title', e.target.value)}
                      />
                      {form.errors.title ? (
                        <div className="text-danger fs-xs mt-1">{form.errors.title}</div>
                      ) : null}
                    </Form.Group>
                  </Col>
                  <Col md={4}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">
                        Loại nội dung <span className="text-danger">*</span>
                      </Form.Label>
                      <Form.Select
                        value={String(form.values.contentType)}
                        isInvalid={Boolean(form.errors.contentType)}
                        onChange={(e) => form.updateField('contentType', Number(e.target.value) as typeof form.values.contentType)}>
                        {PRODUCT_CONTENT_TYPE_OPTIONS.map((opt) => (
                          <option key={opt.value} value={opt.value}>
                            {opt.label}
                          </option>
                        ))}
                      </Form.Select>
                    </Form.Group>
                  </Col>
                  <Col xs={12}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">Tóm tắt</Form.Label>
                      <Form.Control
                        as="textarea"
                        rows={2}
                        value={form.values.summary}
                        onChange={(e) => form.updateField('summary', e.target.value)}
                      />
                    </Form.Group>
                  </Col>
                </Row>
              </CardBody>
            </Card>

            <Card>
              <CardBody>
                {form.isEditorReady ? (
                  <RichHtmlEditor
                    label="Nội dung bài viết *"
                    editorKey={form.values.id || 'new'}
                    defaultTab={form.isNew ? 'visual' : 'preview'}
                    value={form.values.bodyHtml}
                    onChange={(value) => form.updateField('bodyHtml', value)}
                  />
                ) : (
                  <div className="text-center py-5 text-muted">
                    <Spinner animation="border" size="sm" className="me-2" />
                    Đang chuẩn bị editor...
                  </div>
                )}
                {form.errors.bodyHtml ? (
                  <div className="text-danger fs-xs mt-2">{form.errors.bodyHtml}</div>
                ) : null}
              </CardBody>
            </Card>
          </Col>

          <Col xs={12} className="product-content-form-side">
            <Card className="mb-4">
              <CardHeader className="border-light d-flex align-items-center gap-2">
                <TbSettings />
                <h5 className="card-title mb-0">Cấu hình</h5>
              </CardHeader>
              <CardBody>
                <Form.Group className="mb-3">
                  <Form.Label className="fw-semibold">Thứ tự</Form.Label>
                  <Form.Control
                    type="number"
                    value={form.values.sortOrder}
                    onChange={(e) => form.updateField('sortOrder', Number(e.target.value))}
                  />
                </Form.Group>
                <Form.Check
                  type="switch"
                  id="product-content-active"
                  label="Hiển thị"
                  checked={form.values.isActive}
                  onChange={(e) => form.updateField('isActive', e.target.checked)}
                />
              </CardBody>
            </Card>

            <div className="d-grid gap-2">
              <Button type="submit" variant="primary" size="lg" disabled={form.isSaving}>
                {form.isSaving ? (
                  <>
                    <Spinner animation="border" size="sm" className="me-2" />
                    Đang lưu...
                  </>
                ) : (
                  <>
                    <LuPlus className="me-1" />
                    {form.submitLabel}
                  </>
                )}
              </Button>
              <Button type="button" variant="light" size="lg" disabled={form.isSaving} onClick={onCancel}>
                Hủy
              </Button>
            </div>
          </Col>
        </Row>
      </Form>
    </div>
  )
}

export default ProductContentFormPage
