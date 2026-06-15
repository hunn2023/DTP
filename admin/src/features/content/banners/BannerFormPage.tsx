import { Button, Card, CardBody, CardHeader, Col, Container, Form, Row, Spinner } from 'react-bootstrap'
import { LuPlus } from 'react-icons/lu'
import {
  TbAdjustmentsHorizontal,
  TbChevronRight,
  TbInfoCircle,
  TbPhoto,
  TbRefresh,
} from 'react-icons/tb'
import { Link } from 'react-router'

import PageMetaData from '@/components/PageMetaData'
import { BANNER_POSITION_OPTIONS } from '@/features/content/bannerPosition'
import BannerImageUrlPreview from '@/features/content/banners/components/BannerImageUrlPreview'
import { useBannerFormPage } from '@/features/content/banners/useBannerFormPage'
import { APP_NAME } from '@/shared/config/brand'

const BannerFormPage = () => {
  const form = useBannerFormPage()

  if (form.isLoading) {
    return (
      <Container fluid className="py-5 text-center">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang tải banner...
      </Container>
    )
  }

  if (form.loadFailed) {
    return (
      <Container fluid className="py-5 text-center">
        <p className="text-muted mb-3">Không tìm thấy banner.</p>
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
          <p className="text-muted mb-0 mt-1">
            {form.isNew
              ? 'Cấu hình banner marketing mới trên website.'
              : 'Cập nhật thông tin và hình ảnh banner.'}
          </p>
        </div>
        <div className="text-end">
          <div className="breadcrumb m-0 py-0 d-flex align-items-center gap-1 justify-content-end">
            <span className="text-muted">{APP_NAME}</span>
            <TbChevronRight className="text-muted" />
            <Link to="/website/banners" className="text-muted text-decoration-none">
              Banners
            </Link>
            <TbChevronRight className="text-muted" />
            <span className="fw-semibold">{form.isNew ? 'New' : 'Edit'}</span>
          </div>
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
                <span className="avatar-xs rounded-circle bg-primary-subtle text-primary d-inline-flex align-items-center justify-content-center">
                  <TbInfoCircle />
                </span>
                <h5 className="card-title mb-0">Thông tin cơ bản</h5>
              </CardHeader>
              <CardBody>
                <Form.Group className="mb-3">
                  <Form.Label className="fw-semibold">
                    Tiêu đề banner <span className="text-danger">*</span>
                  </Form.Label>
                  <Form.Control
                    type="text"
                    value={form.values.title}
                    placeholder="VD: Khám phá Nhật Bản với eSIM tốc độ cao"
                    isInvalid={Boolean(form.errors.title)}
                    onChange={(e) => form.updateField('title', e.target.value)}
                  />
                  {form.errors.title ? (
                    <div className="text-danger fs-xs mt-1">{form.errors.title}</div>
                  ) : null}
                </Form.Group>

                <Form.Group className="mb-3">
                  <Form.Label className="fw-semibold">Đường dẫn (Link)</Form.Label>
                  <Form.Control
                    type="url"
                    value={form.values.linkUrl}
                    placeholder="https://example.com/offers/japan-2024"
                    onChange={(e) => form.updateField('linkUrl', e.target.value)}
                  />
                </Form.Group>

                <Form.Group>
                  <Form.Label className="fw-semibold">Mô tả ngắn</Form.Label>
                  <Form.Control
                    as="textarea"
                    rows={3}
                    value={form.values.description}
                    placeholder="Mô tả ngắn về chiến dịch banner..."
                    onChange={(e) => form.updateField('description', e.target.value)}
                  />
                </Form.Group>
              </CardBody>
            </Card>

            <Card>
              <CardHeader className="border-light d-flex align-items-center gap-2">
                <span className="avatar-xs rounded-circle bg-primary-subtle text-primary d-inline-flex align-items-center justify-content-center">
                  <TbPhoto />
                </span>
                <h5 className="card-title mb-0">Hình ảnh quảng bá</h5>
              </CardHeader>
              <CardBody>
                <Row className="g-4">
                  <Col md={8}>
                    <BannerImageUrlPreview
                      label="Ảnh desktop"
                      required
                      hint="1920x600px"
                      variant="desktop"
                      value={form.values.imageUrl}
                      error={form.errors.imageUrl}
                      onChange={(value) => form.updateField('imageUrl', value)}
                    />
                  </Col>
                  <Col md={4}>
                    <BannerImageUrlPreview
                      label="Ảnh mobile"
                      required
                      hint="800x800px"
                      variant="mobile"
                      value={form.values.mobileImageUrl}
                      error={form.errors.mobileImageUrl}
                      onChange={(value) => form.updateField('mobileImageUrl', value)}
                    />
                  </Col>
                </Row>
              </CardBody>
            </Card>
          </Col>

          <Col xl={4}>
            <Card className="mb-4">
              <CardHeader className="border-light d-flex align-items-center gap-2">
                <span className="avatar-xs rounded-circle bg-primary-subtle text-primary d-inline-flex align-items-center justify-content-center">
                  <TbAdjustmentsHorizontal />
                </span>
                <h5 className="card-title mb-0">Cấu hình hiển thị</h5>
              </CardHeader>
              <CardBody>
                <Form.Group className="mb-3">
                  <Form.Label className="fw-semibold">
                    Vị trí <span className="text-danger">*</span>
                  </Form.Label>
                  <Form.Select
                    value={String(form.values.position)}
                    isInvalid={Boolean(form.errors.position)}
                    onChange={(e) => form.updateField('position', Number(e.target.value) as typeof form.values.position)}>
                    {BANNER_POSITION_OPTIONS.map((opt) => (
                      <option key={opt.value} value={opt.value}>
                        {opt.label}
                      </option>
                    ))}
                  </Form.Select>
                </Form.Group>

                <Form.Group className="mb-3">
                  <Form.Label className="fw-semibold">
                    Thứ tự (Sort Order) <span className="text-danger">*</span>
                  </Form.Label>
                  <Form.Control
                    type="number"
                    value={form.values.sortOrder}
                    onChange={(e) => form.updateField('sortOrder', Number(e.target.value))}
                  />
                </Form.Group>

                <Row className="g-3">
                  <Col sm={6}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">Từ ngày</Form.Label>
                      <Form.Control
                        type="date"
                        value={form.values.startDate}
                        onChange={(e) => form.updateField('startDate', e.target.value)}
                      />
                    </Form.Group>
                  </Col>
                  <Col sm={6}>
                    <Form.Group>
                      <Form.Label className="fw-semibold">Đến ngày</Form.Label>
                      <Form.Control
                        type="date"
                        value={form.values.endDate}
                        onChange={(e) => form.updateField('endDate', e.target.value)}
                      />
                    </Form.Group>
                  </Col>
                </Row>
              </CardBody>
            </Card>

            <Card className="mb-4">
              <CardHeader className="border-light d-flex align-items-center gap-2">
                <span className="avatar-xs rounded-circle bg-primary-subtle text-primary d-inline-flex align-items-center justify-content-center">
                  <TbRefresh />
                </span>
                <h5 className="card-title mb-0">Trạng thái</h5>
              </CardHeader>
              <CardBody>
                <div className="d-flex align-items-center justify-content-between mb-3">
                  <span className="fw-semibold">Trạng thái</span>
                  <div className="d-flex align-items-center gap-2">
                    <Form.Check
                      type="switch"
                      id="banner-is-active"
                      checked={form.values.isActive}
                      onChange={(e) => form.updateField('isActive', e.target.checked)}
                    />
                    <span className="text-muted">{form.values.isActive ? 'Hiển thị' : 'Ẩn'}</span>
                  </div>
                </div>
                <div className="rounded bg-light p-3 d-flex gap-2">
                  <TbInfoCircle className="text-muted flex-shrink-0 mt-1" />
                  <p className="text-muted fs-sm mb-0">
                    Banner đang bật sẽ hiển thị ngay trên vị trí đã chọn cho khách truy cập website.
                  </p>
                </div>
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

export default BannerFormPage
