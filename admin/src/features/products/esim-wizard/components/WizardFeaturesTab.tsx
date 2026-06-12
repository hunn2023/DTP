import {
  DndContext,
  type DragEndEvent,
  KeyboardSensor,
  PointerSensor,
  closestCenter,
  useSensor,
  useSensors,
} from '@dnd-kit/core'
import {
  SortableContext,
  arrayMove,
  sortableKeyboardCoordinates,
  useSortable,
  verticalListSortingStrategy,
} from '@dnd-kit/sortable'
import { CSS } from '@dnd-kit/utilities'
import { useCallback, useEffect, useState } from 'react'
import { Alert, Button, Card, Form, Spinner } from 'react-bootstrap'
import { LuGripVertical, LuPlus, LuX } from 'react-icons/lu'

import {
  createVariantFeature,
  deleteVariantFeature,
  fetchVariantFeatures,
  updateVariantFeature,
} from '@/apis/productVariantFeaturesApi'

type FeatureDraft = {
  clientId: string
  id?: string
  text: string
  sortOrder: number
}

type WizardFeaturesTabProps = {
  variantId: string
  onRegisterSave: (fn: () => Promise<boolean>) => void
  onSavingChange: (saving: boolean) => void
}

function createClientId(): string {
  return crypto.randomUUID()
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

type SortableFeatureRowProps = {
  item: FeatureDraft
  canRemove: boolean
  onTextChange: (text: string) => void
  onRemove: () => void
}

function SortableFeatureRow({ item, canRemove, onTextChange, onRemove }: SortableFeatureRowProps) {
  const { attributes, listeners, setNodeRef, setActivatorNodeRef, transform, transition, isDragging } =
    useSortable({ id: item.clientId })

  const style = {
    transform: CSS.Transform.toString(transform),
    transition,
    opacity: isDragging ? 0.85 : 1,
  }

  return (
    <div ref={setNodeRef} style={style} className="d-flex gap-2 align-items-center rounded border p-2 bg-white">
      <button
        type="button"
        ref={setActivatorNodeRef}
        className="btn btn-link text-muted p-0 flex-shrink-0"
        style={{ cursor: 'grab', touchAction: 'none' }}
        aria-label="Kéo để sắp xếp"
        {...attributes}
        {...listeners}>
        <LuGripVertical className="fs-20" />
      </button>
      <Form.Control
        className="border-0 bg-light"
        value={item.text}
        placeholder="VD: Mạng 4G ổn định tại sân bay và trung tâm thành phố"
        onChange={(e) => onTextChange(e.target.value)}
      />
      <Button
        type="button"
        variant="link"
        className="text-danger p-0 flex-shrink-0"
        onClick={onRemove}
        disabled={!canRemove}>
        <LuX />
      </Button>
    </div>
  )
}

const WizardFeaturesTab = ({ variantId, onRegisterSave, onSavingChange }: WizardFeaturesTabProps) => {
  const [items, setItems] = useState<FeatureDraft[]>([
    { clientId: createClientId(), text: '', sortOrder: 1 },
  ])
  const [removedIds, setRemovedIds] = useState<string[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [error, setError] = useState('')

  const sensors = useSensors(
    useSensor(PointerSensor, { activationConstraint: { distance: 6 } }),
    useSensor(KeyboardSensor, { coordinateGetter: sortableKeyboardCoordinates }),
  )

  useEffect(() => {
    let active = true
    setIsLoading(true)

    void fetchVariantFeatures(variantId)
      .then((features) => {
        if (!active || features.length === 0) return
        setItems(
          features
            .sort((a, b) => a.sortOrder - b.sortOrder)
            .map((f) => ({
              clientId: f.id,
              id: f.id,
              text: f.text,
              sortOrder: f.sortOrder,
            })),
        )
      })
      .finally(() => {
        if (active) setIsLoading(false)
      })

    return () => {
      active = false
    }
  }, [variantId])

  const reorderItems = (activeId: string, overId: string) => {
    setItems((prev) => {
      const oldIndex = prev.findIndex((item) => item.clientId === activeId)
      const newIndex = prev.findIndex((item) => item.clientId === overId)
      if (oldIndex < 0 || newIndex < 0) return prev
      return arrayMove(prev, oldIndex, newIndex).map((item, index) => ({
        ...item,
        sortOrder: index + 1,
      }))
    })
  }

  const handleDragEnd = (event: DragEndEvent) => {
    const { active, over } = event
    if (!over || active.id === over.id) return
    reorderItems(String(active.id), String(over.id))
  }

  const saveFeatures = useCallback(async (): Promise<boolean> => {
    setError('')
    const validItems = items.filter((item) => item.text.trim())
    if (validItems.length === 0) {
      setError('Vui lòng thêm ít nhất 1 tính năng')
      return false
    }

    onSavingChange(true)
    try {
      for (const id of removedIds) {
        await deleteVariantFeature(id)
      }

      for (let i = 0; i < validItems.length; i++) {
        const item = validItems[i]
        const sortOrder = i + 1
        if (item.id) {
          await updateVariantFeature(item.id, {
            text: item.text.trim(),
            sortOrder,
            isActive: true,
          })
        } else {
          await createVariantFeature({
            productVariantId: variantId,
            text: item.text.trim(),
            sortOrder,
            isActive: true,
          })
        }
      }
      return true
    } catch (err) {
      setError(getErrorMessage(err, 'Không lưu được tính năng'))
      return false
    } finally {
      onSavingChange(false)
    }
  }, [items, removedIds, variantId, onSavingChange])

  useEffect(() => {
    onRegisterSave(saveFeatures)
  }, [onRegisterSave, saveFeatures])

  const addItem = () => {
    setItems((prev) => [
      ...prev,
      { clientId: createClientId(), text: '', sortOrder: prev.length + 1 },
    ])
  }

  const removeItem = (clientId: string) => {
    setItems((prev) => {
      const target = prev.find((item) => item.clientId === clientId)
      if (target?.id) setRemovedIds((ids) => [...ids, target.id!])
      return prev
        .filter((item) => item.clientId !== clientId)
        .map((item, index) => ({ ...item, sortOrder: index + 1 }))
    })
  }

  const updateText = (clientId: string, text: string) => {
    setItems((prev) => prev.map((item) => (item.clientId === clientId ? { ...item, text } : item)))
  }

  return (
    <Card className="border shadow-none">
      <Card.Body>
        <div className="d-flex justify-content-between align-items-center mb-3 gap-2 flex-wrap">
          <div>
            <h5 className="fw-semibold mb-1">Tính năng nổi bật</h5>
            <span className="text-muted fs-sm">Kéo thả để sắp xếp thứ tự hiển thị trên card bán hàng.</span>
          </div>
          <div className="d-flex align-items-center gap-2">
            {isLoading && <Spinner animation="border" size="sm" />}
            <Button type="button" variant="outline-primary" size="sm" onClick={addItem}>
              <LuPlus className="me-1" />
              Thêm tính năng
            </Button>
          </div>
        </div>

        <DndContext sensors={sensors} collisionDetection={closestCenter} onDragEnd={handleDragEnd}>
          <SortableContext items={items.map((item) => item.clientId)} strategy={verticalListSortingStrategy}>
            <div className="d-flex flex-column gap-2">
              {items.map((item) => (
                <SortableFeatureRow
                  key={item.clientId}
                  item={item}
                  canRemove={items.length > 1 || Boolean(item.text)}
                  onTextChange={(text) => updateText(item.clientId, text)}
                  onRemove={() => removeItem(item.clientId)}
                />
              ))}
            </div>
          </SortableContext>
        </DndContext>

        {error && (
          <Alert variant="danger" className="mt-3 mb-0">
            {error}
          </Alert>
        )}
      </Card.Body>
    </Card>
  )
}

export default WizardFeaturesTab
