import { create } from "zustand";
import { persist } from "zustand/middleware";
import { CartItem, CartState } from "@/types/cart";

const FALLBACK_CART_IMAGE = "/images/product-placeholder.svg";

function normalizeQuantity(value: unknown, fallback = 1) {
  if (typeof value !== "number" || Number.isNaN(value)) {
    return fallback;
  }

  return Math.max(1, Math.floor(value));
}

function normalizeCartItem(rawItem: unknown): CartItem | null {
  if (!rawItem || typeof rawItem !== "object") {
    return null;
  }

  const item = rawItem as Partial<CartItem>;
  const id = typeof item.id === "string" ? item.id.trim() : "";
  if (!id) {
    return null;
  }

  const stock = normalizeQuantity(item.stock, 999);
  const quantity = Math.min(normalizeQuantity(item.quantity, 1), stock);
  const price = typeof item.price === "number" && Number.isFinite(item.price) ? item.price : 0;

  return {
    id,
    name: typeof item.name === "string" && item.name.trim() ? item.name : "Sản phẩm EZSIM",
    slug: typeof item.slug === "string" && item.slug.trim() ? item.slug : id,
    href: typeof item.href === "string" && item.href.trim() ? item.href : undefined,
    image: typeof item.image === "string" && item.image.trim() ? item.image : FALLBACK_CART_IMAGE,
    price: Math.max(0, price),
    quantity,
    stock,
    // Preserve order-related IDs
    productId: typeof item.productId === "string" ? item.productId : undefined,
    productVariantId: typeof item.productVariantId === "string" ? item.productVariantId : undefined,
    esimPackageId: typeof item.esimPackageId === "string" ? item.esimPackageId : undefined,
    phoneCardId: typeof item.phoneCardId === "string" ? item.phoneCardId : undefined,
    itemType: typeof item.itemType === "number" ? item.itemType : undefined,
  };
}

function normalizeCartItems(rawItems: unknown) {
  if (!Array.isArray(rawItems)) {
    return [] as CartItem[];
  }

  return rawItems
    .map((item) => normalizeCartItem(item))
    .filter((item): item is CartItem => item !== null);
}

function normalizePersistedCartState(rawState: unknown) {
  if (!rawState || typeof rawState !== "object") {
    return { items: [] as CartItem[], buyNowItem: null as CartItem | null };
  }

  const persistedState = rawState as { items?: unknown; buyNowItem?: unknown };
  return {
    items: normalizeCartItems(persistedState.items),
    buyNowItem: normalizeCartItem(persistedState.buyNowItem),
  };
}

export const useCartStore = create<CartState>()(
  persist(
    (set, get) => ({
      items: [],
      buyNowItem: null,

      addToCart: (item: CartItem) => {
        const normalizedItem = normalizeCartItem(item);
        if (!normalizedItem) {
          return;
        }

        set((state) => {
          const existing = state.items.find((i) => i.id === normalizedItem.id);

          if (existing) {
            // Không vượt quá stock
            const newQty = Math.min(existing.quantity + normalizedItem.quantity, existing.stock);
            return {
              items: state.items.map((i) =>
                i.id === normalizedItem.id
                  ? {
                      ...i,
                      quantity: newQty,
                      // Merge API IDs if not already set
                      productId: i.productId || normalizedItem.productId,
                      productVariantId: i.productVariantId || normalizedItem.productVariantId,
                      esimPackageId: i.esimPackageId || normalizedItem.esimPackageId,
                      phoneCardId: i.phoneCardId || normalizedItem.phoneCardId,
                      itemType: i.itemType || normalizedItem.itemType,
                    }
                  : i
              ),
            };
          }

          // Sản phẩm mới: clamp quantity theo stock
          const clampedQty = Math.min(normalizedItem.quantity, normalizedItem.stock);
          return {
            items: [...state.items, { ...normalizedItem, quantity: clampedQty }],
          };
        });
      },

      setBuyNowItem: (item: CartItem) => {
        const normalizedItem = normalizeCartItem(item);
        if (!normalizedItem) {
          return;
        }

        set({
          buyNowItem: {
            ...normalizedItem,
            quantity: Math.min(normalizedItem.quantity, normalizedItem.stock),
          },
        });
      },

      clearBuyNowItem: () => {
        set({ buyNowItem: null });
      },

      removeFromCart: (productId: string) => {
        set((state) => ({
          items: state.items.filter((i) => i.id !== productId),
        }));
      },

      increaseQuantity: (productId: string) => {
        set((state) => ({
          items: state.items.map((i) =>
            i.id === productId
              ? { ...i, quantity: Math.min(i.quantity + 1, i.stock) }
              : i
          ),
        }));
      },

      decreaseQuantity: (productId: string) => {
        set((state) => ({
          items: state.items.map((i) =>
            i.id === productId
              ? { ...i, quantity: Math.max(i.quantity - 1, 1) }
              : i
          ),
        }));
      },

      updateQuantity: (productId: string, quantity: number) => {
        set((state) => ({
          items: state.items.map((i) =>
            i.id === productId
              ? { ...i, quantity: Math.max(1, Math.min(quantity, i.stock)) }
              : i
          ),
        }));
      },

      clearCart: () => {
        set({ items: [] });
      },

      // Computed: dùng get() để đọc state hiện tại, tránh stale closure
      getTotalQuantity: () => {
        return get().items.reduce((sum, i) => sum + i.quantity, 0);
      },

      getTotalAmount: () => {
        return get().items.reduce((sum, i) => sum + i.price * i.quantity, 0);
      },
    }),
    {
      name: "ezsim-cart",
      version: 1,
      merge: (persistedState, currentState) => {
        const normalizedPersistedState = normalizePersistedCartState(
          persistedState && typeof persistedState === "object" && "state" in persistedState
            ? (persistedState as { state?: unknown }).state
            : persistedState
        );

        return {
          ...currentState,
          ...normalizedPersistedState,
        };
      },
    }
  )
);
