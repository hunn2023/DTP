"use client";

import { useEffect, useState } from "react";
import Icon from "@/components/ui/Icon";

export default function SuccessCard() {
  const [cardVisible, setCardVisible] = useState(false);
  const [iconVisible, setIconVisible] = useState(false);

  useEffect(() => {
    const t1 = setTimeout(() => setCardVisible(true), 50);
    const t2 = setTimeout(() => setIconVisible(true), 250);
    return () => {
      clearTimeout(t1);
      clearTimeout(t2);
    };
  }, []);

  return (
    <div
      className={`bg-white rounded-2xl shadow-card px-6 py-10 md:px-10 text-center
        transition-all duration-700 ease-out
        ${cardVisible ? "opacity-100 translate-y-0" : "opacity-0 translate-y-5"}`}
    >
      {/* Layered rings + icon */}
      <div className="relative inline-flex items-center justify-center mx-auto mb-6">
        <div
          className={`absolute w-28 h-28 rounded-full bg-success/10
            transition-all duration-500
            ${iconVisible ? "scale-100 opacity-100" : "scale-50 opacity-0"}`}
        />
        <div
          className={`absolute w-20 h-20 rounded-full bg-success/15
            transition-all duration-500 delay-75
            ${iconVisible ? "scale-100 opacity-100" : "scale-50 opacity-0"}`}
        />
        <div
          className={`relative w-14 h-14 rounded-full bg-success-light flex items-center justify-center
            transition-all duration-500 delay-150
            ${iconVisible ? "scale-100 opacity-100" : "scale-0 opacity-0"}`}
        >
          <Icon icon="check-circle" className="text-2xl text-success" />
        </div>
      </div>

      <h1
        className={`text-2xl md:text-3xl font-bold text-navy mb-3
          transition-all duration-500 delay-300
          ${cardVisible ? "opacity-100 translate-y-0" : "opacity-0 translate-y-2"}`}
      >
        Đặt hàng thành công!
      </h1>

      <p
        className={`text-gray-500 text-sm md:text-base leading-relaxed max-w-sm mx-auto
          transition-all duration-500 delay-500
          ${cardVisible ? "opacity-100" : "opacity-0"}`}
      >
        Cảm ơn bạn đã mua hàng tại{" "}
        <span className="font-semibold text-primary">EZSIM</span>.{" "}
        Đơn hàng sẽ được xử lý và xác nhận trong thời gian sớm nhất.
      </p>
    </div>
  );
}
