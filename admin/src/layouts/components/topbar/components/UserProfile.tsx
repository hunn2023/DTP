import { Image } from 'react-bootstrap'
import { Dropdown, DropdownDivider, DropdownItem, DropdownMenu, DropdownToggle } from 'react-bootstrap'
import { useNavigate } from 'react-router'
import { TbChevronDown, TbLogout2 } from 'react-icons/tb'

import user2 from '@/assets/images/users/user-2.jpg'
import { useAuth } from '@/context/useAuthContext'
import { getErrorMessage } from '@/features/system/shared/getErrorMessage'

const UserProfile = () => {
  const navigate = useNavigate()
  const { user, logout } = useAuth()

  const displayName = user?.fullName?.trim() || user?.email || 'Admin'
  const avatarSrc = user?.avatarUrl?.trim() || user2

  const handleLogout = () => {
    void (async () => {
      try {
        await logout()
        navigate('/login', { replace: true })
      } catch (error) {
        window.alert(getErrorMessage(error, 'Không đăng xuất được'))
      }
    })()
  }

  return (
    <div className="topbar-item nav-user">
      <Dropdown align="end">
        <DropdownToggle as="a" className="topbar-link dropdown-toggle drop-arrow-none px-2">
          <Image src={avatarSrc} width="32" height="32" className="rounded-circle me-lg-2 d-flex" alt="" />
          <div className="d-lg-flex align-items-center gap-1 d-none">
            <h5 className="my-0 text-truncate" style={{ maxWidth: 160 }}>
              {displayName}
            </h5>
            <TbChevronDown className="align-middle" />
          </div>
        </DropdownToggle>
        <DropdownMenu className="dropdown-menu-end">
          <div className="dropdown-header noti-title">
            <h6 className="text-overflow m-0">{displayName}</h6>
            {user?.email && <p className="text-muted fs-xs mb-0 text-truncate">{user.email}</p>}
          </div>
          <DropdownDivider />
          <DropdownItem
            as="button"
            type="button"
            className="text-danger fw-semibold"
            onClick={handleLogout}>
            <TbLogout2 className="me-2 fs-17 align-middle" />
            <span className="align-middle">Đăng xuất</span>
          </DropdownItem>
        </DropdownMenu>
      </Dropdown>
    </div>
  )
}

export default UserProfile
