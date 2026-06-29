interface Props {
  name: string;
  sku: string;
  description?: string;
  longDescription?: string;
}

export default function ProductInfo({ name, sku, description, longDescription }: Props) {
  return (
    <div className="space-y-4">
      <div>
        <h1 className="text-xl md:text-2xl font-bold text-navy">{name}</h1>
        <p className="text-xs text-gray-400 mt-1">SKU: {sku}</p>
      </div>

      {description && (
        <p className="text-gray-600 text-sm leading-relaxed">{description}</p>
      )}

      {longDescription && (
        <div className="pt-4 border-t border-gray-100">
          <h3 className="text-sm font-semibold text-navy mb-2">Mô tả chi tiết</h3>
          <p className="text-gray-600 text-sm leading-relaxed">{longDescription}</p>
        </div>
      )}
    </div>
  );
}
