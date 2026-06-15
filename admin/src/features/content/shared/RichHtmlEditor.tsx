import { memo, useCallback, useEffect, useRef, useState } from 'react'
import { Button, Form, Nav } from 'react-bootstrap'
import { TbCode, TbEye, TbWriting } from 'react-icons/tb'

import QuillVisualEditor from '@/features/content/shared/QuillVisualEditor'
import { withLazyImages } from '@/features/content/shared/richHtmlContentUtils'

import './rich-html-editor.scss'

type EditorTab = 'visual' | 'html' | 'preview'

type RichHtmlEditorProps = {
  label?: string
  value: string
  onChange: (value: string) => void
  editorKey?: string
  defaultTab?: EditorTab
  minHeight?: number
}

const RichHtmlEditor = ({
  label,
  value,
  onChange,
  editorKey = 'default',
  defaultTab = 'visual',
  minHeight = 420,
}: RichHtmlEditorProps) => {
  const onChangeRef = useRef(onChange)
  onChangeRef.current = onChange

  const [tab, setTab] = useState<EditorTab>(defaultTab)
  const [editorContent, setEditorContent] = useState(value)
  const [visualMounted, setVisualMounted] = useState(defaultTab === 'visual')

  useEffect(() => {
    setEditorContent(value)
    setTab(defaultTab)
    setVisualMounted(defaultTab === 'visual')
  }, [editorKey, defaultTab])

  const handleTabChange = useCallback((next: EditorTab) => {
    if (next === 'visual') setVisualMounted(true)
    setTab(next)
  }, [])

  const handleContentChange = useCallback((next: string) => {
    setEditorContent(next)
    onChangeRef.current(next)
  }, [])

  const previewHtml = withLazyImages(
    editorContent || '<p class="text-muted">Chưa có nội dung</p>',
  )

  return (
    <div className="rich-html-editor">
      {label ? <Form.Label className="fw-semibold">{label}</Form.Label> : null}

      <div className="d-flex align-items-center justify-content-between gap-2 mb-2 flex-wrap">
        <Nav variant="tabs" className="rich-html-editor__tabs">
          <Nav.Item>
            <Nav.Link active={tab === 'visual'} onClick={() => handleTabChange('visual')}>
              <TbWriting className="me-1" />
              Soạn thảo
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <Nav.Link active={tab === 'html'} onClick={() => handleTabChange('html')}>
              <TbCode className="me-1" />
              HTML
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <Nav.Link active={tab === 'preview'} onClick={() => handleTabChange('preview')}>
              <TbEye className="me-1" />
              Xem trước
            </Nav.Link>
          </Nav.Item>
        </Nav>
        {tab === 'html' ? (
          <Button
            size="sm"
            variant="light"
            onClick={() => handleTabChange('visual')}
            disabled={!editorContent.trim()}>
            Áp dụng HTML
          </Button>
        ) : null}
      </div>

      {visualMounted ? (
        <div className={tab === 'visual' ? '' : 'd-none'}>
          <QuillVisualEditor
            editorKey={editorKey}
            value={editorContent}
            onChange={handleContentChange}
          />
        </div>
      ) : null}

      {tab === 'html' ? (
        <Form.Control
          as="textarea"
          rows={14}
          style={{ minHeight }}
          value={editorContent}
          placeholder="Dán hoặc chỉnh sửa HTML trực tiếp..."
          className="rich-html-editor__html-input font-monospace"
          onChange={(e) => handleContentChange(e.target.value)}
        />
      ) : null}

      {tab === 'preview' ? (
        <div
          className="rich-html-editor__preview"
          style={{ minHeight }}
          dangerouslySetInnerHTML={{ __html: previewHtml }}
        />
      ) : null}
    </div>
  )
}

export default memo(RichHtmlEditor)
