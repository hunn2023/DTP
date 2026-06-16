import { useEffect, useMemo, useRef, useState } from 'react'
import { Alert, Badge, Card, Form, Placeholder, Table } from 'react-bootstrap'

import { fetchCarriers } from '@/apis/carriersApi'
import { fetchCountries } from '@/apis/countriesApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { Carrier } from '@/features/master-data/types'

type WizardCarriersTabProps = {
  selectedCarrierIds: string[]
  savedCarrierIds: string[]
  countryId?: string
  onChange: (carrierIds: string[]) => void
  onDirtyChange?: (dirty: boolean) => void
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

const LOADING_ROW_COUNT = 4

const WizardCarriersTab = ({
  selectedCarrierIds,
  savedCarrierIds,
  countryId,
  onChange,
  onDirtyChange,
}: WizardCarriersTabProps) => {
  const { showNotification } = useNotificationContext()
  const [carriers, setCarriers] = useState<Carrier[]>([])
  const [isLoading, setIsLoading] = useState(true)

  const isDirty = useMemo(() => {
    const current = [...selectedCarrierIds].sort().join(',')
    const saved = [...savedCarrierIds].sort().join(',')
    return current !== saved
  }, [selectedCarrierIds, savedCarrierIds])

  const onDirtyChangeRef = useRef(onDirtyChange)
  onDirtyChangeRef.current = onDirtyChange

  useEffect(() => {
    onDirtyChangeRef.current?.(isDirty)
  }, [isDirty])

  useEffect(() => {
    let active = true
    setIsLoading(true)

    void Promise.all([fetchCountries(), fetchCarriers()])
      .then(([countries, items]) => {
        if (!active) return

        const countryNameById = new Map(countries.map((c) => [c.id, c.name]))
        const mapped = items.map((carrier) => ({
          ...carrier,
          countryName: countryNameById.get(carrier.countryId) ?? carrier.countryName,
        }))

        const filtered = countryId
          ? mapped.filter((carrier) => carrier.countryId === countryId)
          : mapped

        setCarriers(filtered)
      })
      .catch((error) => {
        if (!active) return
        setCarriers([])
        showNotification({
          title: 'Lỗi',
          message: getErrorMessage(error, 'Không tải được danh sách nhà mạng'),
          variant: 'danger',
          delay: 4000,
        })
      })
      .finally(() => {
        if (active) setIsLoading(false)
      })

    return () => {
      active = false
    }
  }, [countryId, showNotification])

  const toggleCarrier = (carrierId: string, checked: boolean) => {
    onChange(
      checked
        ? [...selectedCarrierIds, carrierId]
        : selectedCarrierIds.filter((id) => id !== carrierId),
    )
  }

  return (
    <Card className="border shadow-none">
      <Card.Body>
        <div className="d-flex justify-content-between gap-2 flex-wrap mb-3">
          <div>
            <h5 className="fw-semibold mb-1">Nhà mạng hỗ trợ</h5>
            <p className="text-muted mb-0 fs-sm">Chọn ít nhất một nhà mạng để gói eSIM có thể bán ra.</p>
          </div>
          <Badge bg="primary-subtle" text="primary" className="align-self-start">
            {selectedCarrierIds.length} đã chọn
          </Badge>
        </div>

        {!isLoading && selectedCarrierIds.length === 0 && (
          <Alert variant="info" className="fs-sm">
            Chưa có nhà mạng nào được chọn.
          </Alert>
        )}

        <Table responsive hover className="align-middle mb-0">
          <thead className="table-light">
            <tr>
              <th style={{ width: 72 }}>Chọn</th>
              <th>Nhà mạng</th>
              <th>Quốc gia</th>
            </tr>
          </thead>
          <tbody>
            {isLoading ? (
              Array.from({ length: LOADING_ROW_COUNT }, (_, index) => (
                <tr key={`loading-${index}`}>
                  <td>
                    <Form.Check disabled />
                  </td>
                  <td>
                    <Placeholder animation="glow">
                      <Placeholder xs={8} />
                    </Placeholder>
                  </td>
                  <td>
                    <Placeholder animation="glow">
                      <Placeholder xs={6} />
                    </Placeholder>
                  </td>
                </tr>
              ))
            ) : carriers.length === 0 ? (
              <tr>
                <td colSpan={3} className="text-muted">
                  {countryId
                    ? 'Chưa có nhà mạng cho quốc gia của gói eSIM. Thêm nhà mạng trong Cấu hình hệ thống.'
                    : 'Chưa có nhà mạng nào.'}
                </td>
              </tr>
            ) : (
              carriers.map((carrier) => (
                <tr key={carrier.id}>
                  <td>
                    <Form.Check
                      type="checkbox"
                      checked={selectedCarrierIds.includes(carrier.id)}
                      onChange={(e) => toggleCarrier(carrier.id, e.target.checked)}
                    />
                  </td>
                  <td className="fw-semibold">{carrier.name}</td>
                  <td className="text-muted">{carrier.countryName || '-'}</td>
                </tr>
              ))
            )}
          </tbody>
        </Table>
      </Card.Body>
    </Card>
  )
}

export default WizardCarriersTab
