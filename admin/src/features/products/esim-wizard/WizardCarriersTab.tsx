import { useEffect, useState } from 'react'
import { Alert, Form, Spinner, Table } from 'react-bootstrap'

import { fetchCountries } from '@/features/master-data/countries/countries.api'
import { fetchCarriers } from '@/features/master-data/carriers/carriers.api'
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
    <div>
      <h5 className="fw-semibold mb-2">Chọn nhà mạng hỗ trợ</h5>
      <Alert variant="info" className="fs-sm">
        Chọn các nhà mạng mà gói eSIM này hỗ trợ. Ít nhất chọn 1 nhà mạng.
      </Alert>

      {carriers.length === 0 ? (
        <p className="text-muted">Chưa có nhà mạng nào.</p>
      ) : (
        <Table responsive bordered hover className="align-middle mb-0">
          <thead className="table-light">
            <tr>
              <th>Chọn</th>
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
                <td>{carrier.countryName || '—'}</td>
              </tr>
            ))}
          </tbody>
        </Table>
      )}
    </div>
  )
}

export default WizardCarriersTab
