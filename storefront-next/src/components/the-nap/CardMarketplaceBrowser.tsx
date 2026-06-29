"use client";

import { useMemo, useState } from "react";
import { toast } from "sonner";
import { useCartStore } from "@/lib/cartStore";
import type { CardMarketplaceContent } from "@/types/cardMarketplace";
import ProviderGrid from "./ProviderGrid";
import DenominationGrid from "./DenominationGrid";
import CheckoutSection from "./CheckoutSection";
import { InfoRow, HowToBuy } from "./InfoSection";

function SectionH2({ children }: { children: React.ReactNode }) {
  return (
    <h2
      className="font-extrabold flex items-center gap-2.5"
      style={{ fontSize: "24px", letterSpacing: "-0.5px" }}
    >
      <div className="gradient-primary" style={{ width: "4px", height: "24px", borderRadius: "2px" }} />
      {children}
    </h2>
  );
}

export default function CardMarketplaceBrowser({ content }: { content: CardMarketplaceContent }) {
  const addToCart = useCartStore((state) => state.addToCart);
  const [selectedProviderId, setSelectedProviderId] = useState(content.providers[0]?.id ?? "");
  const selectedProvider = useMemo(
    () => content.providers.find((provider) => provider.id === selectedProviderId) ?? content.providers[0],
    [content.providers, selectedProviderId]
  );
  const [selectedFace, setSelectedFace] = useState(selectedProvider?.denominations[0]?.face ?? 0);

  const selectedDenomination = useMemo(() => {
    const denomination = selectedProvider?.denominations.find((item) => item.face === selectedFace);
    return denomination ?? selectedProvider?.denominations[0];
  }, [selectedFace, selectedProvider]);

  const selectedOptionLabel = selectedDenomination
    ? selectedDenomination.label ?? `${selectedDenomination.face.toLocaleString("vi-VN")}đ`
    : "";

  const handleProviderSelect = (providerId: string) => {
    const provider = content.providers.find((item) => item.id === providerId);
    setSelectedProviderId(providerId);
    if (provider?.denominations[0]) {
      setSelectedFace(provider.denominations[0].face);
    }
  };

  const handleCheckout = ({ quantity }: { quantity: number; paymentMethod: string; email: string; phone: string }) => {
    if (!selectedProvider || !selectedDenomination) {
      toast.error("Chưa chọn nhà cung cấp hoặc mệnh giá phù hợp.");
      return;
    }

    addToCart({
      id: selectedDenomination.phoneCardId || `${selectedProvider.id}-${selectedDenomination.face}`,
      name: `${content.productLabel} ${selectedProvider.name} ${selectedOptionLabel}`,
      slug: `${selectedProvider.id}-${selectedDenomination.face}`,
      href: `/the-nap?tab=${content.tab}`,
      image: `https://picsum.photos/seed/${selectedProvider.id}-${selectedDenomination.face}/640/480`,
      price: selectedDenomination.pay,
      quantity,
      stock: 999,
      phoneCardId: selectedDenomination.phoneCardId,
      productVariantId: selectedDenomination.productVariantId,
      itemType: 2, // PhoneCard
    });
  };

  if (!selectedProvider || !selectedDenomination) {
    return null;
  }

  return (
    <>
      <SectionH2>{content.step1Title}</SectionH2>
      <p className="text-gray-500" style={{ fontSize: "14px", marginBottom: "20px", marginTop: "4px" }}>
        {content.step1Desc}
      </p>
      <ProviderGrid
        providers={content.providers}
        selectedProviderId={selectedProvider.id}
        onSelect={handleProviderSelect}
      />

      <div style={{ marginTop: "40px" }}>
        <SectionH2>{content.step2Title}</SectionH2>
        <p className="text-gray-500" style={{ fontSize: "14px", marginBottom: "20px", marginTop: "4px" }}>
          {content.step2Desc}
        </p>
        <DenominationGrid
          providerName={selectedProvider.name}
          providerColor={selectedProvider.bg}
          providerLetter={selectedProvider.letter}
          discountLabel={selectedProvider.discountLabel}
          denominations={selectedProvider.denominations}
          selectedFace={selectedDenomination.face}
          onSelect={setSelectedFace}
        />
      </div>

      <SectionH2>Bước 3: Hoàn tất đơn hàng</SectionH2>
      <div style={{ marginTop: "16px" }}>
        <CheckoutSection
          providerName={`${content.productLabel} ${selectedProvider.name}`}
          optionLabel={selectedOptionLabel}
          faceValue={selectedDenomination.face}
          payValue={selectedDenomination.pay}
          discountPercent={selectedProvider.discountPercent}
          providerDescription={selectedProvider.description}
          deliveryTime={selectedProvider.deliveryTime}
          itemUnit={content.itemUnit}
          onCheckout={handleCheckout}
        />
      </div>

      <InfoRow />
      <HowToBuy />
    </>
  );
}
