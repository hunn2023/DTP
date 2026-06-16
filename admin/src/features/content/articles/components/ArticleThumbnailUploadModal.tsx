import { useEffect, useState } from 'react'
import { Button, Modal, ModalBody, ModalFooter, ModalHeader, ModalTitle } from 'react-bootstrap'

import FileUploader from '@/components/FileUploader'

type ArticleThumbnailUploadModalProps = {
  show: boolean
  isUploading: boolean
  onHide: () => void
  onUpload: (file: File) => void
}

const ArticleThumbnailUploadModal = ({
  show,
  isUploading,
  onHide,
  onUpload,
}: ArticleThumbnailUploadModalProps) => {
  const [files, setFiles] = useState<File[] | undefined>()

  useEffect(() => {
    if (!show) setFiles(undefined)
  }, [show])

  const handleUpload = () => {
    const file = files?.[0]
    if (!file) return
    onUpload(file)
  }

  return (
    <Modal show={show} onHide={onHide} centered>
      <ModalHeader closeButton>
        <ModalTitle>Upload ảnh thumbnail</ModalTitle>
      </ModalHeader>
      <ModalBody>
        <p className="text-muted fs-sm mb-3">Chọn ảnh đại diện cho bài viết (JPG, PNG, WebP).</p>
        <FileUploader files={files} setFiles={setFiles} maxFileCount={1} disabled={isUploading} />
      </ModalBody>
      <ModalFooter>
        <Button variant="light" onClick={onHide} disabled={isUploading}>
          Bỏ qua
        </Button>
        <Button variant="primary" onClick={handleUpload} disabled={isUploading || !files?.[0]}>
          {isUploading ? 'Đang upload...' : 'Upload'}
        </Button>
      </ModalFooter>
    </Modal>
  )
}

export default ArticleThumbnailUploadModal
