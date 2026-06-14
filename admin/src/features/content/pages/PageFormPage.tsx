import { Button, Card, CardBody, Col, Container, Form, Row, Spinner } from 'react-bootstrap'
import { LuPlus } from 'react-icons/lu'
import { TbChevronRight } from 'react-icons/tb'
import { Link } from 'react-router'

import PageMetaData from '@/components/PageMetaData'
import { usePageFormPage } from '@/features/content/pages/usePageFormPage'
import RichHtmlEditor from '@/features/content/shared/RichHtmlEditor'
import { CONTENT_STATUS_OPTIONS } from '@/features/content/shared/contentStatus'
import { APP_NAME } from '@/shared/config/brand'

const PageFormPage = () => {
  const form = usePageFormPage()

  if (form.isLoading) {
    return (
      <Container fluid className="py-5 text-center">
        <Spinner size="sm" animation="border" className="me-2" />
        Đang tải trang...
      </Container>
    )
  }

  if (form.loadFailed) {
    return (
      <Container fluid className="py-5 text-center">
        <p className="text-muted mb-3">Không tìm thấy trang.</p>
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
          <p className="text-muted mb-0 mt-1">Quản lý nội dung trang tĩnh trên website.</p>
        </div>
        <div className="breadcrumb m-0 py-0 d-flex align-items-center gap-1 justify-content-end">
          <span className="text-muted">{APP_NAME}</span>
          <TbChevronRight className="text-muted" />
          <Link to="/website/pages" className="text-muted text-decoration-none">
            Trang tĩnh
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
        <Card>
          <CardBody>
            <Row className="g-3 align-items-end">
              {form.isNew ? (
                <Col md={3}>
                  <Form.Label className="fw-semibold">Mã trang *</Form.Label>
                  <Form.Control
                    value={form.values.code}
                    onChange={(e) => form.updateField('code', e.target.value)}
                  />
                </Col>
              ) : null}
              <Col md={form.isNew ? 6 : 9}>
                <Form.Label className="fw-semibold">Tiêu đề *</Form.Label>
                <Form.Control
                  value={form.values.title}
                  onChange={(e) => form.updateField('title', e.target.value)}
                />
              </Col>
              <Col md={3} className="d-flex gap-2 justify-content-md-end">
                <Button type="button" variant="light" disabled={form.isSaving} onClick={form.cancel}>
                  Hủy
                </Button>
                <Button type="submit" variant="primary" disabled={form.isSaving} className="text-nowrap">
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
              </Col>
            </Row>

            <Row className="g-3 mt-1">
              <Col md={6}>
                <Form.Label className="fw-semibold">Slug *</Form.Label>
                <Form.Control
                  value={form.values.slug}
                  onChange={(e) => form.updateField('slug', e.target.value)}
                />
              </Col>
              <Col md={6}>
                <Form.Label className="fw-semibold">Trạng thái</Form.Label>
                <Form.Select
                  value={String(form.values.status)}
                  onChange={(e) =>
                    form.updateField('status', Number(e.target.value) as typeof form.values.status)
                  }>
                  {CONTENT_STATUS_OPTIONS.map((o) => (
                    <option key={o.value} value={o.value}>
                      {o.label}
                    </option>
                  ))}
                </Form.Select>
              </Col>
              <Col xs={12}>
                <Form.Label className="fw-semibold">Tóm tắt</Form.Label>
                <Form.Control
                  as="textarea"
                  rows={2}
                  value={form.values.summary}
                  onChange={(e) => form.updateField('summary', e.target.value)}
                />
              </Col>
            </Row>
            <div className="mt-4">
              <RichHtmlEditor
                label="Nội dung *"
                value={form.values.content}
                onChange={(v) => form.updateField('content', v)}
              />
            </div>
          </CardBody>
        </Card>
      </Form>
    </Container>
  )
}

export default PageFormPage
