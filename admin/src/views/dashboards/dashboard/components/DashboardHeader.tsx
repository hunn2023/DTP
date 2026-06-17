import { Button, Form, InputGroup } from 'react-bootstrap'

import { useDashboard } from '@/features/dashboard/DashboardContext'
import { toDateInputValue } from '@/features/reports/reportDateRange'

import { dashboardHeader } from '../data'

const todayInputMax = toDateInputValue(new Date())

const DashboardHeader = () => {
  const { filters, setFilters, applyFilters, isLoading } = useDashboard()

  return (
    <div className="dashboard-header w-100 mb-3">
      <h4 className="dashboard-header__title mb-0 fw-bold">{dashboardHeader.title}</h4>
      <InputGroup size="sm" className="dashboard-header__filter ms-auto">
        <Form.Control
          type="date"
          className="dashboard-header__date"
          value={filters.fromDate}
          onChange={(e) => setFilters({ ...filters, fromDate: e.target.value })}
          aria-label="Từ ngày"
        />
        <InputGroup.Text className="dashboard-header__sep px-1">–</InputGroup.Text>
        <Form.Control
          type="date"
          className="dashboard-header__date"
          value={filters.toDate}
          max={todayInputMax}
          onChange={(e) => setFilters({ ...filters, toDate: e.target.value })}
          aria-label="Đến ngày"
        />
        <Button variant="primary" disabled={isLoading} onClick={applyFilters}>
          {isLoading ? '...' : 'Áp dụng'}
        </Button>
      </InputGroup>
    </div>
  )
}

export default DashboardHeader
