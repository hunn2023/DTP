import { useEffect, useState } from 'react'
import { Alert, Badge, Card, Form, Spinner, Table } from 'react-bootstrap'

import { fetchCarriers } from '@/features/master-data/carriers/carriers.api'
import { fetchCountries } from '@/features/master-data/countries/countries.api'
import type { Carrier } from '@/features/master-data/types'

type WizardCarriersTabProps = {
  selectedCarrierIds: string[]
  onChange: (carrierIds: string[]) => void
}

const WizardCarriersTab = ({ selectedCarrierIds, onChange }: WizardCarriersTabProps) => {
  const [carriers, setCarriers] = useState<Carrier[]>([])
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    setIsLoading(true)
    void Promise.all([fetchCountries(), fetchCarriers()])
      .then(([countries, items]) => {
        const countryNameById = new Map(countries.map((c) => [c.id, c.name]))
        setCarriers(
          items.map((carrier) => ({
            ...carrier,
            countryName: countryNameById.get(carrier.countryId) ?? carrier.countryName,
          })),
        )
      })
      .finally(() => setIsLoading(false))
  }, [])

  const toggleCarrier = (carrierId: string, checked: boolean) => {
    onChange(
      checked
        ? [...selectedCarrierIds, carrierId]
        : selectedCarrierIds.filter((id) => id !== carrierId),
    )
  }

  if (isLoading) {
    return (
      <div className="text-center py-5">
        <Spinner animation="border" size="sm" />
      </div>
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

        {selectedCarrierIds.length === 0 && (
          <Alert variant="info" className="fs-sm">
            Chưa có nhà mạng nào được chọn.
          </Alert>
        )}

        {carriers.length === 0 ? (
          <p className="text-muted mb-0">Chưa có nhà mạng nào.</p>
        ) : (
          <Table responsive hover className="align-middle mb-0">
            <thead className="table-light">
              <tr>
                <th style={{ width: 72 }}>Chọn</th>
                <th>Nhà mạng</th>
                <th>Quốc gia</th>
              </tr>
            </thead>
            <tbody>
              {carriers.map((carrier) => (
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
              ))}
            </tbody>
          </Table>
        )}
      </Card.Body>
    </Card>
  )
}

export default WizardCarriersTab
