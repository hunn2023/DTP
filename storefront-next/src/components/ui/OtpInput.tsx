"use client";

import { useCallback, useEffect, useRef } from "react";

const DEFAULT_LENGTH = 6;

const cellClassName =
  "w-11 h-12 sm:w-12 sm:h-14 flex items-center justify-center text-xl font-bold rounded-lg border-2 transition select-none";

interface OtpInputProps {
  value: string;
  onChange: (value: string) => void;
  length?: number;
  disabled?: boolean;
  autoFocus?: boolean;
}

function normalizeOtp(value: string, length: number): string {
  return value.replace(/\D/g, "").slice(0, length);
}

export default function OtpInput({
  value,
  onChange,
  length = DEFAULT_LENGTH,
  disabled = false,
  autoFocus = false,
}: OtpInputProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const onChangeRef = useRef(onChange);
  const valueRef = useRef(value);

  onChangeRef.current = onChange;

  const normalizedValue = normalizeOtp(value, length);
  const activeIndex = Math.min(normalizedValue.length, length - 1);

  const applyValue = useCallback(
    (raw: string) => {
      const normalized = normalizeOtp(raw, length);
      valueRef.current = normalized;
      onChangeRef.current(normalized);
    },
    [length]
  );

  const focusContainer = useCallback(() => {
    if (!disabled) {
      inputRef.current?.focus();
    }
  }, [disabled]);

  useEffect(() => {
    valueRef.current = normalizeOtp(value, length);
  }, [length, value]);

  useEffect(() => {
    if (autoFocus && !disabled) {
      inputRef.current?.focus();
    }
  }, [autoFocus, disabled]);

  return (
    <div className="relative inline-flex flex-col items-center">
      <div
        ref={containerRef}
        role="group"
        aria-label="Mã OTP"
        onClick={focusContainer}
        className={`relative inline-flex items-center justify-center gap-2 sm:gap-3 outline-none ${
          disabled ? "cursor-not-allowed" : "cursor-text"
        }`}
      >
        {Array.from({ length }).map((_, index) => {
          const isActive = !disabled && index === activeIndex;
          const hasValue = Boolean(normalizedValue[index]);

          return (
            <div
              key={index}
              aria-hidden
              className={`${cellClassName} ${
                isActive
                  ? "border-primary ring-2 ring-primary/20"
                  : hasValue
                    ? "border-primary/40 bg-primary-light/20"
                    : "border-gray-200 bg-white"
              } ${disabled ? "bg-gray-50" : ""}`}
            >
              {normalizedValue[index] || ""}
            </div>
          );
        })}
        <input
          ref={inputRef}
          type="text"
          inputMode="numeric"
          pattern="[0-9]*"
          autoComplete="one-time-code"
          aria-label="Mã OTP"
          disabled={disabled}
          value={normalizedValue}
          maxLength={length}
          onFocus={(event) => {
            const position = event.currentTarget.value.length;
            event.currentTarget.setSelectionRange(position, position);
          }}
          onChange={(event) => applyValue(event.target.value)}
          onPaste={(event) => {
            event.preventDefault();
            applyValue(event.clipboardData.getData("text"));
          }}
          className="absolute inset-0 z-10 h-full w-full cursor-text border-0 bg-transparent p-0 text-transparent opacity-0 outline-none caret-transparent"
        />
      </div>
    </div>
  );
}
