"use client";

import { useState } from "react";
import Image from "next/image";

interface Props {
  images: string[];
  productName: string;
}

export default function ProductImages({ images, productName }: Props) {
  const [activeIndex, setActiveIndex] = useState(0);
  const [imgErrors, setImgErrors] = useState<Set<number>>(new Set());

  const handleError = (index: number) => {
    setImgErrors((prev) => new Set(prev).add(index));
  };

  return (
    <div className="space-y-4">
      {/* Main image */}
      <div className="relative aspect-square overflow-hidden rounded-xl bg-gray-100 border border-gray-200">
        <Image
          src={imgErrors.has(activeIndex) ? "/images/product-placeholder.svg" : images[activeIndex]}
          alt={`${productName} - Ảnh ${activeIndex + 1}`}
          fill
          sizes="(max-width: 768px) 100vw, 50vw"
          className="object-cover"
          priority
          onError={() => handleError(activeIndex)}
        />
      </div>

      {/* Thumbnails */}
      {images.length > 1 && (
        <div className="flex gap-3 overflow-x-auto pb-2">
          {images.map((img, i) => (
            <button
              key={i}
              type="button"
              onClick={() => setActiveIndex(i)}
              aria-label={`Xem ảnh ${i + 1}`}
              className={`relative w-16 h-16 md:w-20 md:h-20 flex-shrink-0 rounded-lg overflow-hidden border-2 transition ${
                i === activeIndex
                  ? "border-primary ring-2 ring-primary/20"
                  : "border-gray-200 hover:border-gray-300"
              }`}
            >
              <Image
                src={imgErrors.has(i) ? "/images/product-placeholder.svg" : img}
                alt={`${productName} - Thumbnail ${i + 1}`}
                fill
                sizes="80px"
                className="object-cover"
                onError={() => handleError(i)}
              />
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
