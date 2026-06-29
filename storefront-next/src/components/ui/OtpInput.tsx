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

function getDigitFromCode(code: string): string | null {
  if (/^Digit[0-9]$/.test(code)) {
    return code.slice(5);
  }
  if (/^Numpad[0-9]$/.test(code)) {
    return code.slice(6);
  }
  return null;
}

export default function OtpInput({
  value,
  onChange,
  length = DEFAULT_LENGTH,
  disabled = false,
  autoFocus = false,
}: OtpInputProps) {
  const containerRef = useRef<HTMLDivElement>(null);
  const autofillRef = useRef<HTMLInputElement>(null);
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
      containerRef.current?.focus();
    }
  }, [disabled]);

  useEffect(() => {
    valueRef.current = normalizeOtp(value, length);
  }, [length, value]);

  useEffect(() => {
    const container = containerRef.current;
    if (!container || disabled) return;

    const onKeyDown = (event: KeyboardEvent) => {
      if (event.isComposing || event.keyCode === 229) {
        event.preventDefault();
        return;
      }

      const digit = getDigitFromCode(event.code);
      if (digit !== null) {
        event.preventDefault();
        event.stopImmediatePropagation();

        const current = normalizeOtp(valueRef.current, length);
        if (current.length < length) {
          applyValue(current + digit);
        }
        return;
      }

      if (event.key === "Backspace" || event.key === "Delete") {
        event.preventDefault();
        event.stopImmediatePropagation();
        applyValue(normalizeOtp(valueRef.current, length).slice(0, -1));
        return;
      }

      if (
        event.key.length === 1 &&
        !event.ctrlKey &&
        !event.metaKey &&
        !event.altKey &&
        event.key !== "Tab"
      ) {
        event.preventDefault();
      }
    };

    const onPaste = (event: ClipboardEvent) => {
      event.preventDefault();
      event.stopImmediatePropagation();
      applyValue(event.clipboardData?.getData("text") ?? "");
    };

    container.addEventListener("keydown", onKeyDown, true);
    container.addEventListener("paste", onPaste, true);

    return () => {
      container.removeEventListener("keydown", onKeyDown, true);
      container.removeEventListener("paste", onPaste, true);
    };
  }, [applyValue, disabled, length]);

  useEffect(() => {
    if (autoFocus && !disabled) {
      containerRef.current?.focus();
    }
  }, [autoFocus, disabled]);

  return (
    <div className="relative inline-flex flex-col items-center">
      <div
        ref={containerRef}
        role="group"
        aria-label="Mã OTP"
        tabIndex={disabled ? -1 : 0}
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
      </div>

      <input
        ref={autofillRef}
        type="text"
        inputMode="numeric"
        autoComplete="one-time-code"
        tabIndex={-1}
        aria-hidden
        disabled={disabled}
        value={normalizedValue}
        onChange={(event) => applyValue(event.target.value)}
        className="pointer-events-none absolute h-px w-px opacity-0"
        style={{ left: "-9999px" }}
      />
    </div>
  );
}
