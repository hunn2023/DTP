import { type FormEvent, useState } from 'react'
import { Alert, Button, Card, Col, Container, Form, Row } from 'react-bootstrap'
import { Navigate, useLocation, useNavigate } from 'react-router'
import { TbEye, TbEyeOff } from 'react-icons/tb'

import AppLogo from '@/components/AppLogo'
import Loader from '@/components/Loader'
import PageMetaData from '@/components/PageMetaData'
import { useAuth } from '@/context/useAuthContext'
import { getErrorMessage } from '@/features/system/shared/getErrorMessage'
import { author, currentYear } from '@/helpers'

const LoginPage = () => {
  const navigate = useNavigate()
  const location = useLocation()
  const { login, isAuthenticated, isBootstrapping } = useAuth()

  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  const redirectTo =
    (location.state as { from?: { pathname?: string } } | null)?.from?.pathname ?? '/dashboard'

  if (isBootstrapping) {
    return <Loader height="100vh" />
  }

  if (isAuthenticated) {
    return <Navigate to={redirectTo} replace />
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      await login({ email, password })
      navigate(redirectTo, { replace: true })
    } catch (err) {
      setError(getErrorMessage(err, 'Đăng nhập thất bại'))
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <>
      <PageMetaData title="Đăng nhập" />
      <div className="auth-box overflow-hidden align-items-center d-flex" style={{ minHeight: '100vh' }}>
        <Container>
          <Row className="justify-content-center">
            <Col xxl={4} md={6} sm={8}>
              <div className="auth-brand text-center mb-4">
                <AppLogo />
                <h4 className="fw-bold mt-3">Đăng nhập Admin</h4>
                <p className="text-muted w-lg-75 mx-auto">
                  Nhập email và mật khẩu để truy cập hệ thống quản trị.
                </p>
              </div>

              <Card className="p-4 rounded-4">
                <Form onSubmit={(e) => void handleSubmit(e)}>
                  {error && (
                    <Alert variant="danger" className="py-2">
                      {error}
                    </Alert>
                  )}

                  <div className="mb-3 form-group">
                    <Form.Label>
                      Email <span className="text-danger">*</span>
                    </Form.Label>
                    <Form.Control
                      type="email"
                      placeholder="admin@example.com"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      required
                      autoComplete="username"
                    />
                  </div>

                  <div className="mb-3 form-group">
                    <Form.Label>
                      Mật khẩu <span className="text-danger">*</span>
                    </Form.Label>
                    <div className="position-relative">
                      <Form.Control
                        type={showPassword ? 'text' : 'password'}
                        className="pe-5"
                        placeholder="••••••••"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                        autoComplete="current-password"
                      />
                      <button
                        type="button"
                        className="position-absolute top-50 end-0 translate-middle-y d-flex align-items-center justify-content-center border-0 bg-transparent text-muted p-0 me-3"
                        aria-label={showPassword ? 'Ẩn mật khẩu' : 'Hiện mật khẩu'}
                        onClick={() => setShowPassword((prev) => !prev)}>
                        {showPassword ? <TbEyeOff className="fs-lg" /> : <TbEye className="fs-lg" />}
                      </button>
                    </div>
                  </div>

                  <div className="d-grid">
                    <Button type="submit" className="btn-primary fw-semibold py-2" disabled={isSubmitting}>
                      {isSubmitting ? 'Đang đăng nhập...' : 'Đăng nhập'}
                    </Button>
                  </div>
                </Form>
              </Card>

              <p className="text-center text-muted mt-4 mb-0">
                © 2014 - {currentYear} INSPINIA — by <span className="fw-semibold">{author}</span>
              </p>
            </Col>
          </Row>
        </Container>
      </div>
    </>
  )
}

export default LoginPage
