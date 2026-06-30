import { Badge, Button, Modal, Spinner, Table } from 'react-bootstrap'

import type { DeliveryRow } from '@/apis/deliveriesApi'
import {
  DELIVERY_STATUS_LABELS,
  DELIVERY_TYPE_LABELS,
  enumLabel,
  formatDateTime,
} from '@/features/sales/shared/format'

type DeliveryDetailModalProps = {
  delivery: DeliveryRow | null
  loading: boolean
  onClose: () => void
}

const emptyText = '—'

const DeliveryDetailModal = ({ delivery, loading, onClose }: DeliveryDetailModalProps) => (
  <Modal show={delivery !== null || loading} onHide={onClose} centered scrollable size="xl">
    <Modal.Header closeButton>
      <Modal.Title>Chi tiết giao hàng</Modal.Title>
    </Modal.Header>
    <Modal.Body>
      {loading ? (
        <div className="text-center py-4">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải...
        </div>
      ) : delivery ? (
        <dl className="row mb-0">
          <dt className="col-sm-4">Mã đơn</dt>
          <dd className="col-sm-8"><code>{delivery.orderCode}</code></dd>
          <dt className="col-sm-4">Loại</dt>
          <dd className="col-sm-8">{enumLabel(delivery.deliveryType, DELIVERY_TYPE_LABELS)}</dd>
          <dt className="col-sm-4">Trạng thái</dt>
          <dd className="col-sm-8">{enumLabel(delivery.status, DELIVERY_STATUS_LABELS)}</dd>
          <dt className="col-sm-4">Khách hàng</dt>
          <dd className="col-sm-8">{delivery.customerName || emptyText}</dd>
          <dt className="col-sm-4">Customer ID</dt>
          <dd className="col-sm-8"><code>{delivery.customerId || emptyText}</code></dd>
          <dt className="col-sm-4">Email</dt>
          <dd className="col-sm-8">{delivery.customerEmail || emptyText}</dd>
          <dt className="col-sm-4">Lần thử</dt>
          <dd className="col-sm-8">{delivery.attemptCount}</dd>
          <dt className="col-sm-4">IP</dt>
          <dd className="col-sm-8">{delivery.ipAddress || emptyText}</dd>
          <dt className="col-sm-4">Ghi chú</dt>
          <dd className="col-sm-8">{delivery.note || emptyText}</dd>
          <dt className="col-sm-4">Lỗi gần nhất</dt>
          <dd className="col-sm-8">{delivery.lastError || emptyText}</dd>
          <dt className="col-sm-4">Giao lúc</dt>
          <dd className="col-sm-8">{formatDateTime(delivery.deliveredAt)}</dd>
          <dt className="col-sm-4">Email giao hàng</dt>
          <dd className="col-sm-8">
            {delivery.emailError || (delivery.emailSent ? 'Đã gửi' : 'Chưa gửi')}
            {delivery.emailSentAt ? ` · ${formatDateTime(delivery.emailSentAt)}` : ''}
          </dd>
          <dt className="col-sm-4">Lỗi lúc</dt>
          <dd className="col-sm-8">{formatDateTime(delivery.failedAt)}</dd>
          <dt className="col-sm-4">Tạo lúc</dt>
          <dd className="col-sm-8">{formatDateTime(delivery.createdAt)}</dd>

          <dt className="col-12 mt-3 mb-2">Sản phẩm đã giao ({delivery.items.length})</dt>
          <dd className="col-12 mb-0">
            {delivery.items.length === 0 ? (
              emptyText
            ) : (
              <div className="d-flex flex-column gap-3">
                {delivery.items.map((item, index) => (
                  <div key={item.id || `${item.orderItemId}-${index}`} className="border rounded p-3">
                    <div className="d-flex flex-wrap align-items-start justify-content-between gap-2 mb-2">
                      <div>
                        <div className="fw-semibold">{item.productName || emptyText}</div>
                        <div className="text-muted fs-xxs">
                          Item #{index + 1} · SL {item.quantity}
                        </div>
                      </div>
                      <Badge bg={item.isDelivered ? 'success' : 'secondary'}>
                        {item.isDelivered ? 'Đã giao' : 'Chưa giao'}
                      </Badge>
                    </div>

                    <Table responsive bordered size="sm" className="mb-0 align-middle">
                      <tbody>
                        <tr>
                          <th className="w-25">Mã bản ghi giao hàng</th>
                          <td className="text-break"><code>{item.id || emptyText}</code></td>
                        </tr>
                        <tr>
                          <th>Mã dòng đơn hàng</th>
                          <td className="text-break"><code>{item.orderItemId || emptyText}</code></td>
                        </tr>
                        <tr>
                          <th>Mã sản phẩm</th>
                          <td className="text-break"><code>{item.productId || emptyText}</code></td>
                        </tr>
                        <tr>
                          <th>Mã biến thể sản phẩm</th>
                          <td className="text-break"><code>{item.productVariantId || emptyText}</code></td>
                        </tr>
                        <tr>
                          <th>Tên sản phẩm</th>
                          <td className="text-break">{item.productName || emptyText}</td>
                        </tr>
                        <tr>
                          <th>Mã SKU</th>
                          <td><code>{item.sku || emptyText}</code></td>
                        </tr>
                        <tr>
                          <th>Số lượng</th>
                          <td>{item.quantity}</td>
                        </tr>
                        <tr>
                          <th>Link QR code</th>
                          <td className="text-break">
                            {item.qrCodeUrl ? (
                              <Button
                                href={item.qrCodeUrl}
                                target="_blank"
                                rel="noreferrer"
                                variant="link"
                                size="sm"
                                className="p-0 text-start text-break"
                              >
                                {item.qrCodeUrl}
                              </Button>
                            ) : (
                              emptyText
                            )}
                          </td>
                        </tr>
                        <tr>
                          <th>Mã kích hoạt</th>
                          <td className="text-break"><code>{item.activationCode || emptyText}</code></td>
                        </tr>
                        <tr>
                          <th>Số serial</th>
                          <td><code>{item.serialNumber || emptyText}</code></td>
                        </tr>
                        <tr>
                          <th>Mã tham chiếu nhà cung cấp</th>
                          <td><code>{item.providerReference || emptyText}</code></td>
                        </tr>
                        <tr>
                          <th>Giao lúc</th>
                          <td>{formatDateTime(item.deliveredAt)}</td>
                        </tr>
                      </tbody>
                    </Table>
                  </div>
                ))}
              </div>
            )}
          </dd>
        </dl>
      ) : null}
    </Modal.Body>
  </Modal>
)

export default DeliveryDetailModal
