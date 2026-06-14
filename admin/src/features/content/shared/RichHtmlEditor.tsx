import { Suspense, useState } from 'react'
import { Button, Form, Nav, Spinner } from 'react-bootstrap'
import { TbCode, TbEye, TbWriting } from 'react-icons/tb'

import CustomQuill from '@/components/CustomQuill'
import { richHtmlEditorModules } from '@/features/content/shared/richHtmlEditorModules'

import './rich-html-editor.scss'

type EditorTab = 'visual' | 'html' | 'preview'

type RichHtmlEditorProps = {
  label?: string
  value: string
  onChange: (value: string) => void
  minHeight?: number
}

const RichHtmlEditor = ({ label, value, onChange, minHeight = 420 }: RichHtmlEditorProps) => {
  const [tab, setTab] = useState<EditorTab>('visual')

  return (
    <div className="rich-html-editor">
      {label ? <Form.Label className="fw-semibold">{label}</Form.Label> : null}

      <div className="d-flex align-items-center justify-content-between gap-2 mb-2 flex-wrap">
        <Nav variant="tabs" className="rich-html-editor__tabs">
          <Nav.Item>
            <Nav.Link active={tab === 'visual'} onClick={() => setTab('visual')}>
              <TbWriting className="me-1" />
              Soạn thảo
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <Nav.Link active={tab === 'html'} onClick={() => setTab('html')}>
              <TbCode className="me-1" />
              HTML
            </Nav.Link>
          </Nav.Item>
          <Nav.Item>
            <Nav.Link active={tab === 'preview'} onClick={() => setTab('preview')}>
              <TbEye className="me-1" />
              Xem trước
            </Nav.Link>
          </Nav.Item>
        </Nav>
        {tab === 'html' ? (
          <Button
            size="sm"
            variant="light"
            onClick={() => setTab('visual')}
            disabled={!value.trim()}>
            Áp dụng HTML
          </Button>
        ) : null}
      </div>

      {tab === 'visual' ? (
        <Suspense
          fallback={
            <div className="text-center py-4">
              <Spinner size="sm" animation="border" />
            </div>
          }>
          <CustomQuill
            theme="snow"
            modules={richHtmlEditorModules}
            value={value}
            onChange={onChange}
          />
        </Suspense>
      ) : null}

      {tab === 'html' ? (
        <Form.Control
          as="textarea"
          rows={14}
          style={{ minHeight }}
          value={value}
          placeholder="Dán hoặc chỉnh sửa HTML trực tiếp..."
          className="rich-html-editor__html-input font-monospace"
          onChange={(e) => onChange(e.target.value)}
        />
      ) : null}

      {tab === 'preview' ? (
        <div
          className="rich-html-editor__preview"
          style={{ minHeight }}
          dangerouslySetInnerHTML={{ __html: value || '<p class="text-muted">Chưa có nội dung</p>' }}
        />
      ) : null}
    </div>
  )
}

export default RichHtmlEditor
