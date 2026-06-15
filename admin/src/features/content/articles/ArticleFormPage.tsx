import { Button, Card, CardBody, CardHeader, Col, Container, Form, Row, Spinner } from 'react-bootstrap'
import { LuPlus } from 'react-icons/lu'
import { TbChevronRight, TbEye, TbEyeOff, TbInfoCircle, TbSettings, TbStar, TbStarOff } from 'react-icons/tb'
import { Link } from 'react-router'

import PageMetaData from '@/components/PageMetaData'
import BannerImageUrlPreview from '@/features/content/banners/components/BannerImageUrlPreview'
import { useArticleFormPage } from '@/features/content/articles/useArticleFormPage'
import RichHtmlEditor from '@/features/content/shared/RichHtmlEditor'
import {
  getContentStatusBadgeClass,
  getContentStatusLabel,
} from '@/features/content/shared/contentStatus'
import { formatDateTime } from '@/features/system/shared/format'
import { APP_NAME } from '@/shared/config/brand'

const ArticleFormPage = () => {
  const form = useArticleFormPage()

  if (form.isLoading) {
    return (
      <Container fluid className="py-5 text-center">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang tải bài viết...
      </Container>
    )
  }

  if (form.loadFailed) {
    return (
      <Container fluid className="py-5 text-center">
        <p className="text-muted mb-3">Không tìm thấy bài viết.</p>
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
          <p className="text-muted mb-0 mt-1">Viết nội dung blog cho website bán hàng.</p>
        </div>
        <div className="breadcrumb m-0 py-0 d-flex align-items-center gap-1 justify-content-end">
          <span className="text-muted">{APP_NAME}</span>
          <TbChevronRight className="text-muted" />
          <Link to="/website/articles" className="text-muted text-decoration-none">
            Bài viết
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
                <h5 className="card-title mb-0">Thông tin bài viết</h5>
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
                        Slug <span className="text-danger">*</span>
                      </Form.Label>
                      <Form.Control
                        value={form.values.slug}
                        isInvalid={Boolean(form.errors.slug)}
                        onChange={(e) => form.updateField('slug', e.target.value)}
                      />
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
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">Tác giả</Form.Label>
                      <Form.Control
                        value={form.values.authorName}
                        onChange={(e) => form.updateField('authorName', e.target.value)}
                      />
                    </Form.Group>
                  </Col>
                  <Col md={6}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">Tags</Form.Label>
                      <Form.Control
                        value={form.values.tags}
                        placeholder="esim, du-lich, japan"
                        onChange={(e) => form.updateField('tags', e.target.value)}
                      />
                    </Form.Group>
                  </Col>
                </Row>
              </CardBody>
            </Card>

            <Card className="mb-4">
              <CardBody>
                <BannerImageUrlPreview
                  label="Ảnh thumbnail"
                  variant="desktop"
                  value={form.values.thumbnailUrl}
                  onChange={(value) => form.updateField('thumbnailUrl', value)}
                />
              </CardBody>
            </Card>

            <Card>
              <CardBody>
                {form.isEditorReady ? (
                  <RichHtmlEditor
                    label="Nội dung bài viết *"
                    editorKey={form.values.id || 'new'}
                    defaultTab={form.isNew ? 'visual' : 'preview'}
                    value={form.values.content}
                    onChange={(value) => form.updateField('content', value)}
                  />
                ) : (
                  <div className="text-center py-5 text-muted">
                    <Spinner animation="border" size="sm" className="me-2" />
                    Đang chuẩn bị editor...
                  </div>
                )}
                {form.errors.content ? (
                  <div className="text-danger fs-xs mt-2">{form.errors.content}</div>
                ) : null}
              </CardBody>
            </Card>
          </Col>

          <Col xl={4}>
            {!form.isNew ? (
              <Card className="mb-4">
                <CardHeader className="border-light">
                  <h5 className="card-title mb-0">Thống kê</h5>
                </CardHeader>
                <CardBody className="d-flex flex-column gap-2">
                  <div className="d-flex justify-content-between align-items-center gap-2">
                    <span className="text-muted">Trạng thái</span>
                    <span className={`badge ${getContentStatusBadgeClass(form.values.status)}`}>
                      {getContentStatusLabel(form.values.status)}
                    </span>
                  </div>
                  <div className="d-flex justify-content-between align-items-center gap-2">
                    <span className="text-muted">Lượt xem</span>
                    <span className="fw-semibold">{form.values.viewCount}</span>
                  </div>
                  <div className="d-flex justify-content-between align-items-center gap-2">
                    <span className="text-muted">Ngày tạo</span>
                    <span className="text-end">{formatDateTime(form.values.createdAt)}</span>
                  </div>
                  <div className="d-flex justify-content-between align-items-center gap-2">
                    <span className="text-muted">Ngày đăng</span>
                    <span className="text-end">{formatDateTime(form.values.publishedAt)}</span>
                  </div>
                </CardBody>
              </Card>
            ) : null}

            {!form.isNew ? (
              <Card className="mb-4">
                <CardHeader className="border-light">
                  <h5 className="card-title mb-0">Thao tác nhanh</h5>
                </CardHeader>
                <CardBody className="d-grid gap-2">
                  <Button
                    type="button"
                    variant="success"
                    disabled={form.isSaving || form.isActioning || form.values.status === 1}
                    onClick={() => void form.publish()}>
                    <TbEye className="me-1" />
                    Đăng bài
                  </Button>
                  <Button
                    type="button"
                    variant="warning"
                    disabled={form.isSaving || form.isActioning || form.values.status !== 1}
                    onClick={() => void form.hide()}>
                    <TbEyeOff className="me-1" />
                    Ẩn bài
                  </Button>
                  <Button
                    type="button"
                    variant="primary"
                    disabled={form.isSaving || form.isActioning || form.values.isFeatured}
                    onClick={() => void form.feature()}>
                    <TbStar className="me-1" />
                    Đánh dấu nổi bật
                  </Button>
                  <Button
                    type="button"
                    variant="light"
                    disabled={form.isSaving || form.isActioning || !form.values.isFeatured}
                    onClick={() => void form.unfeature()}>
                    <TbStarOff className="me-1" />
                    Bỏ nổi bật
                  </Button>
                </CardBody>
              </Card>
            ) : null}

            <Card className="mb-4">
              <CardHeader className="border-light d-flex align-items-center gap-2">
                <TbSettings />
                <h5 className="card-title mb-0">Cấu hình</h5>
              </CardHeader>
              <CardBody>
                <Form.Group className="mb-3">
                  <Form.Label className="fw-semibold">Danh mục</Form.Label>
                  <Form.Select
                    value={form.values.categoryCode}
                    onChange={(e) => form.updateField('categoryCode', e.target.value)}>
                    <option value="">-- Chọn danh mục --</option>
                    {form.categoryOptions.map((opt) => (
                      <option key={opt.value} value={opt.value}>
                        {opt.label}
                      </option>
                    ))}
                  </Form.Select>
                </Form.Group>
                <Form.Group className="mb-3">
                  <Form.Label className="fw-semibold">Thứ tự</Form.Label>
                  <Form.Control
                    type="number"
                    value={form.values.sortOrder}
                    onChange={(e) => form.updateField('sortOrder', Number(e.target.value))}
                  />
                </Form.Group>
                {form.isNew ? (
                  <p className="text-muted fs-xs mb-0">
                    Trạng thái và nổi bật sẽ cấu hình sau khi tạo bài viết.
                  </p>
                ) : (
                  <p className="text-muted fs-xs mb-0">
                    Dùng &quot;Thao tác nhanh&quot; để đăng, ẩn hoặc đánh dấu nổi bật.
                  </p>
                )}
              </CardBody>
            </Card>

            <div className="d-grid gap-2">
              <Button type="submit" variant="primary" size="lg" disabled={form.isSaving || form.isActioning}>
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
              <Button
                type="button"
                variant="light"
                size="lg"
                disabled={form.isSaving || form.isActioning}
                onClick={form.cancel}>
                Hủy
              </Button>
            </div>
          </Col>
        </Row>
      </Form>
    </Container>
  )
}

export default ArticleFormPage
