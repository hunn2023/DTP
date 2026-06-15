import { type FormEvent, useEffect, useState } from 'react'
import { Button, Col, Form, Modal, ModalBody, ModalFooter, ModalHeader, ModalTitle, Row, Spinner } from 'react-bootstrap'

import type { UserFormValues } from '@/features/system/users/types'
import type { PickerOption } from '@/features/system/shared/CheckboxPickerModal'

type UserCreateModalProps = {
  show: boolean
  initialValues: UserFormValues
  roleOptions: PickerOption[]
  rolesLoading: boolean
  isSaving?: boolean
  onHide: () => void
  onSubmit: (values: UserFormValues) => void
}

function toggleRoleId(ids: string[], id: string): string[] {
  return ids.includes(id) ? ids.filter((item) => item !== id) : [...ids, id]
}

const UserCreateModal = ({
  show,
  initialValues,
  roleOptions,
  rolesLoading,
  isSaving = false,
  onHide,
  onSubmit,
}: UserCreateModalProps) => {
  const [values, setValues] = useState(initialValues)
  const [errors, setErrors] = useState<Partial<Record<string, string>>>({})

  useEffect(() => {
    if (show) {
      setValues(initialValues)
      setErrors({})
    }
  }, [show, initialValues])

  const validate = (): boolean => {
    const next: Partial<Record<string, string>> = {}
    if (!values.email.trim()) next.email = 'Trường này là bắt buộc'
    if (!values.fullName.trim()) next.fullName = 'Trường này là bắt buộc'
    if (!values.password.trim()) next.password = 'Trường này là bắt buộc'
    if (values.roleIds.length === 0) next.roleIds = 'Chọn ít nhất một vai trò'
    setErrors(next)
    return Object.keys(next).length === 0
  }

  const handleSubmit = (event: FormEvent) => {
    event.preventDefault()
    if (!validate()) return
    onSubmit(values)
  }

  return (
    <Modal show={show} onHide={onHide} centered size="lg" scrollable>
      <Form onSubmit={handleSubmit}>
        <ModalHeader closeButton>
          <ModalTitle>Thêm tài khoản</ModalTitle>
        </ModalHeader>
        <ModalBody>
          <Row className="g-3">
            <Col md={6}>
              <Form.Label className="fw-semibold">
                Email <span className="text-danger">*</span>
              </Form.Label>
              <Form.Control
                type="text"
                value={values.email}
                required
                onChange={(e) => setValues((prev) => ({ ...prev, email: e.target.value }))}
              />
              {errors.email && <div className="text-danger fs-xs mt-1">{errors.email}</div>}
            </Col>
            <Col md={6}>
              <Form.Label className="fw-semibold">
                Họ tên <span className="text-danger">*</span>
              </Form.Label>
              <Form.Control
                type="text"
                value={values.fullName}
                required
                onChange={(e) => setValues((prev) => ({ ...prev, fullName: e.target.value }))}
              />
              {errors.fullName && <div className="text-danger fs-xs mt-1">{errors.fullName}</div>}
            </Col>
            <Col md={6}>
              <Form.Label className="fw-semibold">SĐT</Form.Label>
              <Form.Control
                type="text"
                value={values.phone}
                onChange={(e) => setValues((prev) => ({ ...prev, phone: e.target.value }))}
              />
            </Col>
            <Col md={6}>
              <Form.Label className="fw-semibold">
                Mật khẩu <span className="text-danger">*</span>
              </Form.Label>
              <Form.Control
                type="password"
                value={values.password}
                required
                onChange={(e) => setValues((prev) => ({ ...prev, password: e.target.value }))}
              />
              {errors.password && <div className="text-danger fs-xs mt-1">{errors.password}</div>}
            </Col>
            <Col xs={12}>
              <Form.Label className="fw-semibold">
                Vai trò <span className="text-danger">*</span>
              </Form.Label>
              {rolesLoading ? (
                <div className="text-muted py-2">
                  <Spinner animation="border" size="sm" className="me-2" />
                  Đang tải vai trò...
                </div>
              ) : roleOptions.length === 0 ? (
                <div className="text-muted">Chưa có vai trò để gán</div>
              ) : (
                <div className="d-flex flex-column gap-2">
                  {roleOptions.map((role) => (
                    <Form.Check
                      key={role.value}
                      type="checkbox"
                      id={`create-user-role-${role.value}`}
                      label={
                        <span>
                          <span className="fw-medium">{role.label}</span>
                          {role.hint ? <span className="text-muted ms-2 fs-xs">{role.hint}</span> : null}
                        </span>
                      }
                      checked={values.roleIds.includes(role.value)}
                      onChange={() =>
                        setValues((prev) => ({
                          ...prev,
                          roleIds: toggleRoleId(prev.roleIds, role.value),
                        }))
                      }
                    />
                  ))}
                </div>
              )}
              {errors.roleIds && <div className="text-danger fs-xs mt-1">{errors.roleIds}</div>}
            </Col>
          </Row>
        </ModalBody>
        <ModalFooter>
          <Button variant="light" onClick={onHide} disabled={isSaving}>
            Hủy
          </Button>
          <Button variant="primary" type="submit" disabled={isSaving || rolesLoading}>
            {isSaving ? 'Đang tạo...' : 'Tạo mới'}
          </Button>
        </ModalFooter>
      </Form>
    </Modal>
  )
}

export default UserCreateModal
