import { useEffect, useState } from 'react'
import { Button, Col, Collapse, Row } from 'react-bootstrap'
import { LuCalendar, LuChevronDown, LuChevronUp, LuSearch } from 'react-icons/lu'

import {
  type AuditLogFilterForm,
  defaultAuditLogFilterForm,
  hasAdvancedAuditFilters,
} from '@/features/system/audit/auditFilterTypes'
import {
  AUDIT_ACTION_TYPE_OPTIONS,
  AUDIT_MODULE_OPTIONS,
  AUDIT_STATUS_OPTIONS,
} from '@/features/system/shared/format'

type AuditLogFiltersBarProps = {
  value: AuditLogFilterForm
  onChange: (next: AuditLogFilterForm) => void
}

type FilterOption = { value: string; label: string }

function FilterLabel({ children }: { children: string }) {
  return <label className="form-label text-muted fs-xxs text-uppercase mb-1">{children}</label>
}

function FilterSelect({
  label,
  value,
  onChange,
  options,
}: {
  label: string
  value: string
  onChange: (value: string) => void
  options: FilterOption[]
}) {
  return (
    <div>
      <FilterLabel>{label}</FilterLabel>
      <select className="form-select form-select-sm" value={value} onChange={(e) => onChange(e.target.value)}>
        {options.map((option) => (
          <option key={option.value || 'all'} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
    </div>
  )
}

function FilterInput({
  label,
  value,
  placeholder,
  onChange,
}: {
  label: string
  value: string
  placeholder: string
  onChange: (value: string) => void
}) {
  return (
    <div>
      <FilterLabel>{label}</FilterLabel>
      <input
        type="text"
        className="form-control form-control-sm"
        placeholder={placeholder}
        value={value}
        onChange={(e) => onChange(e.target.value)}
      />
    </div>
  )
}

function DateRangeFilter({
  fromDate,
  toDate,
  onFromChange,
  onToChange,
}: {
  fromDate: string
  toDate: string
  onFromChange: (value: string) => void
  onToChange: (value: string) => void
}) {
  return (
    <div>
      <FilterLabel>Khoảng thời gian</FilterLabel>
      <div className="d-flex gap-1 align-items-center">
        <div className="app-search flex-grow-1">
          <input
            type="date"
            className="form-control form-control-sm"
            value={fromDate}
            onChange={(e) => onFromChange(e.target.value)}
          />
          <LuCalendar className="app-search-icon text-muted" />
        </div>
        <span className="text-muted flex-shrink-0">—</span>
        <input
          type="date"
          className="form-control form-control-sm flex-grow-1"
          value={toDate}
          onChange={(e) => onToChange(e.target.value)}
        />
      </div>
    </div>
  )
}

const AuditLogFiltersBar = ({ value, onChange }: AuditLogFiltersBarProps) => {
  const [expanded, setExpanded] = useState(() => hasAdvancedAuditFilters(value))
  const advancedActive = hasAdvancedAuditFilters(value)

  useEffect(() => {
    if (advancedActive) setExpanded(true)
  }, [advancedActive])

  const setField = <K extends keyof AuditLogFilterForm>(key: K, fieldValue: AuditLogFilterForm[K]) => {
    onChange({ ...value, [key]: fieldValue })
  }

  const moduleOptions: FilterOption[] = [
    { value: '', label: 'Tất cả module' },
    ...AUDIT_MODULE_OPTIONS.map((module) => ({ value: module, label: module })),
  ]

  const actionTypeOptions: FilterOption[] = [
    { value: '', label: 'Tất cả loại' },
    ...AUDIT_ACTION_TYPE_OPTIONS.map((item) => ({
      value: String(item.value),
      label: item.label,
    })),
  ]

  const statusOptions: FilterOption[] = [
    { value: '', label: 'Tất cả trạng thái' },
    ...AUDIT_STATUS_OPTIONS.map((item) => ({
      value: String(item.value),
      label: item.label,
    })),
  ]

  return (
    <div className="border-bottom border-light pb-3 mb-0">
      <Row className="g-2 align-items-end">
        <Col xl={3} lg={3} md={6}>
          <FilterLabel>Từ khóa</FilterLabel>
          <div className="app-search">
            <input
              type="search"
              className="form-control form-control-sm"
              placeholder="IP, User, Đối tượng..."
              value={value.keyword}
              onChange={(e) => setField('keyword', e.target.value)}
            />
            <LuSearch className="app-search-icon text-muted" />
          </div>
        </Col>
        <Col xl={2} lg={2} md={6}>
          <FilterSelect
            label="Module"
            value={value.module}
            onChange={(module) => setField('module', module)}
            options={moduleOptions}
          />
        </Col>
        <Col xl={2} lg={2} md={6}>
          <FilterInput
            label="Hành động"
            placeholder="Tất cả hành động"
            value={value.action}
            onChange={(action) => setField('action', action)}
          />
        </Col>
        <Col xl={2} lg={2} md={6}>
          <FilterSelect
            label="Loại"
            value={value.actionType}
            onChange={(actionType) => setField('actionType', actionType)}
            options={actionTypeOptions}
          />
        </Col>
        <Col xl={3} lg={3} md={6} className="d-flex gap-2 align-items-end">
          <div className="flex-grow-1" style={{ minWidth: 0 }}>
            <FilterSelect
              label="Trạng thái"
              value={value.status}
              onChange={(status) => setField('status', status)}
              options={statusOptions}
            />
          </div>
          <Button
            variant="light"
            size="sm"
            className="flex-shrink-0 text-nowrap"
            onClick={() => onChange({ ...defaultAuditLogFilterForm })}>
            Xóa bộ lọc
          </Button>
        </Col>
      </Row>

      <Row className="g-2 align-items-end mt-2">
        <Col xl={4} lg={5} md={7}>
          <DateRangeFilter
            fromDate={value.fromDate}
            toDate={value.toDate}
            onFromChange={(fromDate) => setField('fromDate', fromDate)}
            onToChange={(toDate) => setField('toDate', toDate)}
          />
        </Col>
        <Col xl={8} lg={7} md={5} className="d-flex align-items-end justify-content-end">
          <Button
            variant="link"
            size="sm"
            className="text-decoration-none p-0 d-inline-flex align-items-center gap-1"
            onClick={() => setExpanded((open) => !open)}
            aria-expanded={expanded}>
            {expanded ? <LuChevronUp /> : <LuChevronDown />}
            {expanded ? 'Thu gọn' : 'Bộ lọc nâng cao'}
            {!expanded && advancedActive && (
              <span className="badge bg-primary-subtle text-primary rounded-pill">Đang lọc</span>
            )}
          </Button>
        </Col>
      </Row>

      <Collapse in={expanded}>
        <div className="bg-light bg-opacity-50 rounded border border-light mt-2 p-2">
          <Row className="g-2 align-items-end">
            <Col lg={4} md={6}>
              <FilterInput
                label="User ID"
                placeholder="UUID người dùng"
                value={value.userId}
                onChange={(userId) => setField('userId', userId)}
              />
            </Col>
            <Col lg={4} md={6}>
              <FilterInput
                label="Đối tượng"
                placeholder="Entity name"
                value={value.entityName}
                onChange={(entityName) => setField('entityName', entityName)}
              />
            </Col>
            <Col lg={4} md={6}>
              <FilterInput
                label="Entity ID"
                placeholder="UUID đối tượng"
                value={value.entityId}
                onChange={(entityId) => setField('entityId', entityId)}
              />
            </Col>
          </Row>
        </div>
      </Collapse>
    </div>
  )
}

export default AuditLogFiltersBar
