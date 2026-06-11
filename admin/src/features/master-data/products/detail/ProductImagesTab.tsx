import { useCallback, useEffect, useRef, useState } from 'react'

import { Button, Col, Image, Row, Spinner } from 'react-bootstrap'

import Dropzone, { type FileRejection } from 'react-dropzone'

import { TbCloudUpload, TbStar, TbStarFilled } from 'react-icons/tb'



import { useNotificationContext } from '@/context/useNotificationContext'

import { formatBytes } from '@/helpers/file'

import * as imagesApi from '@/features/master-data/products/product-images.api'

import type { ProductImageRow } from '@/features/master-data/products/types'



type ProductImagesTabProps = { productId: string }



function getErrorMessage(error: unknown, fallback: string): string {

  return error instanceof Error ? error.message : fallback

}



function getImageFileName(image: ProductImageRow): string {

  if (image.altText.trim()) return image.altText.trim()

  const parts = image.imageUrl.split('/')

  return parts[parts.length - 1] || 'image'

}



const ProductImagesTab = ({ productId }: ProductImagesTabProps) => {

  const { showNotification } = useNotificationContext()

  const [data, setData] = useState<ProductImageRow[]>([])

  const [isLoading, setIsLoading] = useState(true)

  const [isUploading, setIsUploading] = useState(false)

  const loadSeqRef = useRef(0)



  const notifyErrorRef = useRef(showNotification)
  notifyErrorRef.current = showNotification

  const reload = useCallback(async () => {
    const seq = ++loadSeqRef.current
    setIsLoading(true)
    try {
      const items = await imagesApi.fetchProductImages(productId)
      if (seq !== loadSeqRef.current) return
      setData(items.sort((a, b) => a.sortOrder - b.sortOrder))
    } catch (e) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current({
        title: 'Lỗi',
        message: getErrorMessage(e, 'Không tải được hình ảnh'),
        variant: 'danger',
        delay: 4000,
      })
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [productId])

  useEffect(() => {
    void reload()
  }, [productId, reload])



  const uploadFiles = async (files: File[]) => {

    if (files.length === 0) return



    setIsUploading(true)

    try {

      for (const file of files) {

        const isFirst = data.length === 0 && files.indexOf(file) === 0

        await imagesApi.uploadProductImage(productId, file, { isThumbnail: isFirst })

      }

      showNotification({

        title: 'Thành công',

        message: `Đã tải lên ${files.length} ảnh`,

        variant: 'success',

        delay: 2500,

      })

      await reload()

    } catch (e) {

      showNotification({

        title: 'Lỗi',

        message: getErrorMessage(e, 'Không tải lên được hình ảnh'),

        variant: 'danger',

        delay: 4000,

      })

    } finally {

      setIsUploading(false)

    }

  }



  const handleDrop = (accepted: File[], rejected: FileRejection[]) => {

    if (rejected.length > 0) {

      showNotification({

        title: 'Lỗi',

        message: 'Ảnh không hợp lệ hoặc vượt quá 10MB',

        variant: 'danger',

        delay: 3000,

      })

    }

    void uploadFiles(accepted)

  }



  const handleSetThumbnail = async (imageId: string) => {

    try {

      await imagesApi.setProductThumbnail(productId, imageId)

      showNotification({ title: 'Thành công', message: 'Đã đặt làm ảnh đại diện', variant: 'success', delay: 2500 })

      await reload()

    } catch (e) {

      showNotification({

        title: 'Lỗi',

        message: getErrorMessage(e, 'Không đặt được ảnh đại diện'),

        variant: 'danger',

        delay: 4000,

      })

    }

  }



  return (

    <div>

      <Dropzone

        onDrop={handleDrop}

        accept={{ 'image/jpeg': ['.jpg', '.jpeg'], 'image/png': ['.png'], 'image/webp': ['.webp'] }}

        maxSize={10 * 1024 * 1024}

        multiple

        disabled={isUploading}>

        {({ getRootProps, getInputProps, isDragActive }) => (

          <div

            {...getRootProps()}

            className="border border-2 border-dashed rounded-3 p-4 p-md-5 text-center mb-4"

            style={{

              cursor: isUploading ? 'not-allowed' : 'pointer',

              backgroundColor: isDragActive ? 'rgba(var(--bs-primary-rgb), 0.04)' : undefined,

            }}>

            <input {...getInputProps()} />

            <div className="avatar-lg mx-auto mb-3">

              <span className="avatar-title bg-primary-subtle text-primary rounded-circle">

                {isUploading ? <Spinner animation="border" size="sm" /> : <TbCloudUpload className="fs-24" />}

              </span>

            </div>

            <h5 className="mb-2">Kéo thả ảnh vào đây hoặc Chọn ảnh từ máy tính</h5>

            <p className="text-muted mb-3 fs-sm">

              Hỗ trợ JPG, PNG, WEBP — tối đa 10MB/ảnh

            </p>

            <Button type="button" variant="light" size="sm" disabled={isUploading}>

              Chọn ảnh

            </Button>

          </div>

        )}

      </Dropzone>



      <div className="d-flex align-items-center justify-content-between mb-3">

        <h5 className="mb-0 fw-semibold">Ảnh đã tải lên</h5>

        <span className="text-muted fs-sm">Kéo thả để sắp xếp thứ tự</span>

      </div>



      {isLoading ? (

        <div className="text-center py-5">

          <Spinner animation="border" size="sm" />

        </div>

      ) : data.length === 0 ? (

        <div className="text-center text-muted py-5 border rounded-3">Chưa có hình ảnh</div>

      ) : (

        <Row className="g-3">

          {data.map((image) => (

            <Col key={image.id} xs={6} md={4} lg={3}>

              <div className="border rounded-3 overflow-hidden position-relative bg-light">

                {image.isThumbnail && (

                  <span className="badge bg-primary position-absolute top-0 start-0 m-2 z-1">

                    Ảnh đại diện

                  </span>

                )}

                <div className="ratio ratio-4x3">

                  <Image

                    src={image.imageUrl}

                    alt={image.altText || getImageFileName(image)}

                    className="object-fit-cover"

                    fluid

                  />

                </div>

                <div className="p-2 d-flex align-items-center justify-content-between gap-2">

                  <div className="text-truncate">

                    <div className="fw-semibold fs-sm text-truncate">{getImageFileName(image)}</div>

                    <div className="text-muted fs-xs">

                      {image.size > 0 ? formatBytes(image.size) : '—'}

                    </div>

                  </div>

                  <Button

                    type="button"

                    variant="link"

                    size="sm"

                    className={`p-0 ${image.isThumbnail ? 'text-warning' : 'text-muted'}`}

                    title={image.isThumbnail ? 'Ảnh đại diện' : 'Đặt làm ảnh đại diện'}

                    onClick={() => void handleSetThumbnail(image.id)}>

                    {image.isThumbnail ? <TbStarFilled className="fs-20" /> : <TbStar className="fs-20" />}

                  </Button>

                </div>

              </div>

            </Col>

          ))}

        </Row>

      )}

    </div>

  )

}



export default ProductImagesTab

