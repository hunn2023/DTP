import { Modal, Spinner } from 'react-bootstrap'

import type { AuditLogDetail } from '@/apis/auditLogsApi'
import {
  AUDIT_ACTION_TYPE_LABELS,
  AUDIT_STATUS_LABELS,
  enumLabel,
  formatDateTime,
} from '@/features/system/shared/format'

function formatJsonBlock(value: string): string {
  if (!value.trim()) return '—'
  try {
    return JSON.stringify(JSON.parse(value), null, 2)
  } catch {
    return value
  }
}

type AuditLogDetailModalProps = {
  open: boolean
  loading: boolean
  detail: AuditLogDetail | null
  onClose: () => void
}

function DetailRow({ label, value }: { label: string; value: string }) {
  return (
    <>
      <dt className="col-sm-3 text-muted">{label}</dt>
      <dd className="col-sm-9">{value || '—'}</dd>
    </>
  )
}

const AuditLogDetailModal = ({ open, loading, detail, onClose }: AuditLogDetailModalProps) => (
  <Modal show={open} onHide={onClose} centered size="lg" scrollable>
    <Modal.Header closeButton>
      <Modal.Title>Chi tiết nhật ký</Modal.Title>
    </Modal.Header>
    <Modal.Body>
      {loading ? (
        <div className="text-center py-4">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải chi tiết...
        </div>
      ) : detail ? (
        <>
          <dl className="row mb-3">
            <DetailRow label="Thời gian" value={formatDateTime(detail.createdAt)} />
            <DetailRow label="Người dùng" value={detail.userName} />
            <DetailRow label="User ID" value={detail.userId} />
            <DetailRow label="Module" value={detail.module} />
            <DetailRow label="Hành động" value={detail.action} />
            <DetailRow
              label="Loại"
              value={enumLabel(detail.actionType, AUDIT_ACTION_TYPE_LABELS)}
            />
            <DetailRow label="Trạng thái" value={enumLabel(detail.status, AUDIT_STATUS_LABELS)} />
            <DetailRow label="Đối tượng" value={detail.entityName} />
            <DetailRow label="Entity ID" value={detail.entityId} />
            <DetailRow label="Mô tả" value={detail.description} />
            <DetailRow label="IP" value={detail.ipAddress} />
            <DetailRow label="Request" value={`${detail.requestMethod} ${detail.requestPath}`} />
            <DetailRow label="User-Agent" value={detail.userAgent} />
            <DetailRow label="Correlation ID" value={detail.correlationId} />
            <DetailRow label="Lỗi" value={detail.errorMessage} />
          </dl>

          {detail.oldValues ? (
            <div className="mb-3">
              <div className="text-muted small mb-1">Giá trị cũ</div>
              <pre className="bg-light rounded p-2 small mb-0">{formatJsonBlock(detail.oldValues)}</pre>
            </div>
          ) : null}

          {detail.newValues ? (
            <div>
              <div className="text-muted small mb-1">Giá trị mới</div>
              <pre className="bg-light rounded p-2 small mb-0">{formatJsonBlock(detail.newValues)}</pre>
            </div>
          ) : null}
        </>
      ) : null}
    </Modal.Body>
  </Modal>
)

export default AuditLogDetailModal
