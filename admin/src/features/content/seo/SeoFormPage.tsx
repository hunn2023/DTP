import { Button, Card, CardBody, CardHeader, Col, Container, Form, Row, Spinner } from 'react-bootstrap'
import { LuPlus } from 'react-icons/lu'
import { TbChevronRight, TbInfoCircle, TbSettings, TbShare } from 'react-icons/tb'
import { Link } from 'react-router'

import PageMetaData from '@/components/PageMetaData'
import BannerImageUrlPreview from '@/features/content/banners/components/BannerImageUrlPreview'
import { SEO_ENTITY_TYPE_OPTIONS } from '@/features/content/seo/data'
import { useSeoFormPage } from '@/features/content/seo/useSeoFormPage'
import { formatDateTime } from '@/features/system/shared/format'
import { APP_NAME } from '@/shared/config/brand'

const SeoFormPage = () => {
  const form = useSeoFormPage()

  if (form.isLoading) {
    return (
      <Container fluid className="py-5 text-center">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang tải cấu hình SEO...
      </Container>
    )
  }

  if (form.loadFailed) {
    return (
      <Container fluid className="py-5 text-center">
        <p className="text-muted mb-3">Không tìm thấy cấu hình SEO.</p>
        <Button variant="primary" onClick={form.cancel}>
          Quay lại danh sách
        </Button>
      </Container>
    )
  }

  return (
    <Container fluid>
      <PageMetaData title={form.pageTitle} />

      <div className="page-title-head d-flex align-items-start justify-content-between gap-3 mb-4">
        <div>
          <h4 className="fw-bold m-0">{form.pageTitle}</h4>
          <p className="text-muted mb-0 mt-1">Cấu hình meta SEO cho trang website bán hàng.</p>
        </div>
        <div className="breadcrumb m-0 py-0 d-flex align-items-center gap-1 justify-content-end">
          <span className="text-muted">{APP_NAME}</span>
          <TbChevronRight className="text-muted" />
          <Link to="/website/seo" className="text-muted text-decoration-none">
            Cấu hình SEO
          </Link>
          <TbChevronRight className="text-muted" />
          <span className="fw-semibold">{form.isNew ? 'New' : 'Edit'}</span>
        </div>
      </div>

      <Form
        onSubmit={(e) => {
          e.preventDefault()
          void form.submit()
        }}>
        <Row className="g-4">
          <Col xl={8}>
            <Card className="mb-4">
              <CardHeader className="border-light d-flex align-items-center gap-2">
                <TbInfoCircle />
                <h5 className="card-title mb-0">Thông tin SEO</h5>
              </CardHeader>
              <CardBody>
                <Row className="g-3">
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">
                        Entity type <span className="text-danger">*</span>
                      </Form.Label>
                      <Form.Select
                        value={form.values.entityType}
                        isInvalid={Boolean(form.errors.entityType)}
                        onChange={(e) => form.updateField('entityType', e.target.value)}>
                        <option value="">-- Chọn entity type --</option>
                        {SEO_ENTITY_TYPE_OPTIONS.map((opt) => (
                          <option key={opt.value} value={opt.value}>
                            {opt.label}
                          </option>
                        ))}
                      </Form.Select>
                      {form.errors.entityType ? (
                        <div className="text-danger fs-xs mt-1">{form.errors.entityType}</div>
                      ) : null}
                    </Form.Group>
                  </Col>
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">Entity ID</Form.Label>
                      <Form.Control
                        value={form.values.entityId}
                        placeholder="UUID hoặc mã entity"
                        onChange={(e) => form.updateField('entityId', e.target.value)}
                      />
                    </Form.Group>
                  </Col>
                  <Col xs={12}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">Route path</Form.Label>
                      <Form.Control
                        value={form.values.routePath}
                        placeholder="/esim/japan"
                        isInvalid={Boolean(form.errors.routePath)}
                        onChange={(e) => form.updateField('routePath', e.target.value)}
                      />
                      {form.errors.routePath ? (
                        <div className="text-danger fs-xs mt-1">{form.errors.routePath}</div>
                      ) : (
                        <Form.Text className="text-muted">Cần Entity ID hoặc Route path.</Form.Text>
                      )}
                    </Form.Group>
                  </Col>
                  <Col xs={12}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">
                        Meta title <span className="text-danger">*</span>
                      </Form.Label>
                      <Form.Control
                        value={form.values.metaTitle}
                        isInvalid={Boolean(form.errors.metaTitle)}
                        onChange={(e) => form.updateField('metaTitle', e.target.value)}
                      />
                      {form.errors.metaTitle ? (
                        <div className="text-danger fs-xs mt-1">{form.errors.metaTitle}</div>
                      ) : null}
                    </Form.Group>
                  </Col>
                  <Col xs={12}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">Meta description</Form.Label>
                      <Form.Control
                        as="textarea"
                        rows={3}
                        value={form.values.metaDescription}
                        onChange={(e) => form.updateField('metaDescription', e.target.value)}
                      />
                    </Form.Group>
                  </Col>
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">Meta keywords</Form.Label>
                      <Form.Control
                        value={form.values.metaKeywords}
                        placeholder="esim, du lịch, japan"
                        onChange={(e) => form.updateField('metaKeywords', e.target.value)}
                      />
                    </Form.Group>
                  </Col>
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">Canonical URL</Form.Label>
                      <Form.Control
                        value={form.values.canonicalUrl}
                        placeholder="https://..."
                        onChange={(e) => form.updateField('canonicalUrl', e.target.value)}
                      />
                    </Form.Group>
                  </Col>
                </Row>
              </CardBody>
            </Card>

            <Card>
              <CardHeader className="border-light d-flex align-items-center gap-2">
                <TbShare />
                <h5 className="card-title mb-0">Open Graph</h5>
              </CardHeader>
              <CardBody>
                <Row className="g-3">
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">OG title</Form.Label>
                      <Form.Control
                        value={form.values.ogTitle}
                        onChange={(e) => form.updateField('ogTitle', e.target.value)}
                      />
                    </Form.Group>
                  </Col>
                  <Col xs={12}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">OG description</Form.Label>
                      <Form.Control
                        as="textarea"
                        rows={2}
                        value={form.values.ogDescription}
                        onChange={(e) => form.updateField('ogDescription', e.target.value)}
                      />
                    </Form.Group>
                  </Col>
                  <Col xs={12}>
                    <BannerImageUrlPreview
                      label="OG image URL"
                      variant="desktop"
                      value={form.values.ogImageUrl}
                      onChange={(value) => form.updateField('ogImageUrl', value)}
                    />
                  </Col>
                </Row>
              </CardBody>
            </Card>
          </Col>

          <Col xl={4}>
            {!form.isNew ? (
              <Card className="mb-4">
                <CardHeader className="border-light">
                  <h5 className="card-title mb-0">Thông tin hệ thống</h5>
                </CardHeader>
                <CardBody className="d-flex flex-column gap-2">
                  <div className="d-flex justify-content-between align-items-center gap-2">
                    <span className="text-muted">Ngày tạo</span>
                    <span className="text-end">{formatDateTime(form.values.createdAt)}</span>
                  </div>
                  <div className="d-flex justify-content-between align-items-center gap-2">
                    <span className="text-muted">Cập nhật</span>
                    <span className="text-end">{formatDateTime(form.values.updatedAt)}</span>
                  </div>
                </CardBody>
              </Card>
            ) : null}

            <Card className="mb-4">
              <CardHeader className="border-light d-flex align-items-center gap-2">
                <TbSettings />
                <h5 className="card-title mb-0">Cấu hình</h5>
              </CardHeader>
              <CardBody>
                <Form.Group>
                  <Form.Label className="fw-semibold">Robots</Form.Label>
                  <Form.Control
                    value={form.values.robots}
                    placeholder="index,follow"
                    onChange={(e) => form.updateField('robots', e.target.value)}
                  />
                </Form.Group>
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
              <Button type="button" variant="light" size="lg" disabled={form.isSaving} onClick={form.cancel}>
                Hủy
              </Button>
            </div>
          </Col>
        </Row>
      </Form>
    </Container>
  )
}

export default SeoFormPage
