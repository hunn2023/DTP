"use client";

import type { CardProvider } from "@/types/cardMarketplace";

export interface ProviderGridProps {
  providers: CardProvider[];
  selectedProviderId: string;
  onSelect: (providerId: string) => void;
}

export default function ProviderGrid({ providers, selectedProviderId, onSelect }: ProviderGridProps) {
  const renderCard = (provider: CardProvider) => {
    const isSelected = selectedProviderId === provider.id;
    return (
      <button
        type="button"
        key={provider.id}
        onClick={() => onSelect(provider.id)}
        className="bg-white text-center cursor-pointer transition relative hover:-translate-y-0.5"
        style={{
          border: isSelected ? "2px solid #0066FF" : "2px solid #E2E8F0",
          borderRadius: "16px",
          padding: "24px",
          background: isSelected ? "rgba(0,102,255,0.04)" : "white",
          boxShadow: isSelected ? "0 12px 32px rgba(0,102,255,0.08)" : undefined,
          minHeight: "176px",
        }}
      >
        {isSelected && (
          <span
            className="absolute flex items-center justify-center bg-primary text-white font-extrabold"
            style={{
              top: "12px",
              right: "12px",
              width: "24px",
              height: "24px",
              borderRadius: "50%",
              fontSize: "13px",
            }}
          >
            ✓
          </span>
        )}
        <div
          className="mx-auto flex items-center justify-center font-extrabold"
          style={{
            width: "64px",
            height: "64px",
            borderRadius: "16px",
            marginBottom: "12px",
            fontSize: "28px",
            background: provider.bg,
            color: provider.textColor ?? "white",
          }}
        >
          {provider.letter}
        </div>
        <div className="font-bold mb-1 leading-tight" style={{ fontSize: "15px", minHeight: "38px" }}>{provider.name}</div>
        <div className="font-bold" style={{ color: "#DC2626", fontSize: "12px" }}>{provider.discountLabel}</div>
      </button>
    );
  };

  return (
    <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
      {providers.map(renderCard)}
    </div>
  );
}
