"use client";

import type { CardDenomination } from "@/types/cardMarketplace";

export interface DenominationGridProps {
  providerName: string;
  providerColor: string;
  providerLetter: string;
  discountLabel: string;
  denominations: CardDenomination[];
  selectedFace: number;
  onSelect: (face: number) => void;
}

export default function DenominationGrid({
  providerName,
  providerColor,
  providerLetter,
  discountLabel,
  denominations,
  selectedFace,
  onSelect,
}: DenominationGridProps) {
  return (
    <div
      className="bg-white"
      style={{
        borderRadius: "20px",
        padding: "28px",
        border: "1px solid #E2E8F0",
        marginBottom: "24px",
      }}
    >
      {/* Header */}
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between" style={{ marginBottom: "20px" }}>
        <div className="font-bold flex items-center gap-3" style={{ fontSize: "18px" }}>
          <div
            className="flex items-center justify-center text-white font-extrabold"
            style={{
              width: "36px",
              height: "36px",
              borderRadius: "8px",
              background: providerColor,
              fontSize: "14px",
            }}
          >
            {providerLetter}
          </div>
          Mệnh giá thẻ {providerName}
        </div>
        <span
          className="font-bold inline-flex items-center self-start md:self-auto"
          style={{
            background: "#FEE2E2",
            color: "#991B1B",
            padding: "6px 14px",
            borderRadius: "8px",
            fontSize: "13px",
          }}
        >
          {discountLabel}
        </span>
      </div>

      {/* Grid */}
      <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-3">
        {denominations.map((d) => {
          const isSelected = selectedFace === d.face;
          return (
            <button
              type="button"
              key={d.face}
              onClick={() => onSelect(d.face)}
              className="bg-white text-center cursor-pointer transition"
              style={{
                border: isSelected ? "2px solid #0066FF" : "2px solid #E2E8F0",
                borderRadius: "12px",
                padding: "16px",
                background: isSelected
                  ? "linear-gradient(135deg, rgba(0,102,255,0.05), rgba(0,212,255,0.05))"
                  : "white",
                minHeight: "132px",
              }}
            >
              <div className="text-gray-500" style={{ fontSize: "11px", marginBottom: "4px" }}>
                {d.description ?? "Mệnh giá"}
              </div>
              <div
                className="font-extrabold text-navy leading-tight"
                style={{ fontSize: "22px", letterSpacing: "-0.5px", marginBottom: "8px" }}
              >
                {d.label ?? `${d.face.toLocaleString("vi-VN")}đ`}
              </div>
              <span
                className="font-bold inline-block"
                style={{
                  background: "#DCFCE7",
                  color: "#166534",
                  padding: "4px 8px",
                  borderRadius: "6px",
                  fontSize: "11px",
                }}
              >
                Trả: {d.pay.toLocaleString("vi-VN")}đ
              </span>
            </button>
          );
        })}
      </div>
    </div>
  );
}
