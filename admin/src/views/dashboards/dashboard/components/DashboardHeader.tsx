import { dashboardHeader } from '../data'

const DashboardHeader = () => (
  <div className="mb-3">
    <h4 className="mb-1 fw-bold">{dashboardHeader.title}</h4>
    <p className="text-muted mb-0">{dashboardHeader.subtitle}</p>
  </div>
)

export default DashboardHeader
