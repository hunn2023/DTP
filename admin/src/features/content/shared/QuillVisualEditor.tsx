import { Component, type ErrorInfo, type ReactNode, Suspense, useEffect, useState } from 'react'
import { Alert, Spinner } from 'react-bootstrap'

import CustomQuill from '@/components/CustomQuill'
import { richHtmlEditorModules } from '@/features/content/shared/richHtmlEditorModules'

type QuillVisualEditorProps = {
  editorKey: string
  value: string
  onChange: (value: string) => void
}

type QuillErrorBoundaryProps = {
  children: ReactNode
}

type QuillErrorBoundaryState = {
  hasError: boolean
}

class QuillErrorBoundary extends Component<QuillErrorBoundaryProps, QuillErrorBoundaryState> {
  state: QuillErrorBoundaryState = { hasError: false }

  static getDerivedStateFromError(): QuillErrorBoundaryState {
    return { hasError: true }
  }

  componentDidCatch(error: Error, info: ErrorInfo): void {
    console.error('[QuillVisualEditor] Error', error, info)
  }

  render() {
    if (this.state.hasError) {
      return (
        <Alert variant="warning" className="mb-0">
          Không mở được trình soạn thảo với nội dung này. Vui lòng dùng tab HTML hoặc Xem trước.
        </Alert>
      )
    }
    return this.props.children
  }
}

function applyLazyAttrsToEditorImages(root: HTMLElement | null): void {
  if (!root) return
  root.querySelectorAll('img').forEach((img) => {
    img.loading = 'lazy'
    img.decoding = 'async'
  })
}

const QuillVisualEditor = ({ editorKey, value, onChange }: QuillVisualEditorProps) => {
  const [isReady, setIsReady] = useState(false)

  useEffect(() => {
    setIsReady(false)
    const idleId = requestIdleCallback(() => setIsReady(true), { timeout: 400 })
    return () => cancelIdleCallback(idleId)
  }, [editorKey])

  useEffect(() => {
    if (!isReady) return
    const frameId = requestAnimationFrame(() => {
      applyLazyAttrsToEditorImages(document.querySelector('.rich-html-editor .ql-editor'))
    })
    return () => cancelAnimationFrame(frameId)
  }, [isReady, value])

  if (!isReady) {
    return (
      <div className="rich-html-editor__quill-loading text-center py-5">
        <Spinner animation="border" size="sm" className="me-2" />
        Đang chuẩn bị trình soạn thảo...
      </div>
    )
  }

  return (
    <QuillErrorBoundary>
      <Suspense
        fallback={
          <div className="rich-html-editor__quill-loading text-center py-5">
            <Spinner size="sm" animation="border" className="me-2" />
            Đang tải editor...
          </div>
        }>
        <CustomQuill
          key={editorKey}
          theme="snow"
          modules={richHtmlEditorModules}
          value={value}
          onChange={onChange}
        />
      </Suspense>
    </QuillErrorBoundary>
  )
}

export default QuillVisualEditor
