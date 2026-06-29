"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import { createPortal } from "react-dom";
import Image from "next/image";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { sendChatbotMessage } from "@/lib/api/chatbotApi";
import type { ChatbotProductSuggestion } from "@/lib/api/chatbotApi";

interface Message {
  id: number;
  text: string;
  sender: "bot" | "user";
  time: string;
  suggestions?: ChatbotProductSuggestion[];
}

const CHATBOT_SESSION_STORAGE_KEY = "ezsim_chatbot_session_id";
const CHATBOT_MESSAGES_STORAGE_KEY = "ezsim_chatbot_messages";

function getCurrentTime(): string {
  return new Date().toLocaleTimeString("vi-VN", { hour: "2-digit", minute: "2-digit" });
}

function formatPrice(value: number, currency: string | null): string {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: currency || "VND",
    maximumFractionDigits: 0,
  }).format(value);
}

function getSuggestionHref(suggestion: ChatbotProductSuggestion): string {
  if (suggestion.buyUrl) return suggestion.buyUrl;
  if (suggestion.productSlug) return `/esim-du-lich/${suggestion.productSlug}`;
  return "/esim-du-lich";
}

const INITIAL_MESSAGES: Message[] = [
  {
    id: 1,
    text: "Xin chào! Mình là trợ lý AI của EZSIM. Bạn muốn tìm eSIM cho quốc gia nào, đi bao nhiêu ngày và nhu cầu data ra sao?",
    sender: "bot",
    time: getCurrentTime(),
  },
];

function SuggestionCard({
  suggestion,
  onNavigate,
}: {
  suggestion: ChatbotProductSuggestion;
  onNavigate: () => void;
}) {
  const dataLabel = suggestion.isUnlimited
    ? "Không giới hạn"
    : [suggestion.dataAmount, suggestion.dataUnit].filter(Boolean).join(" ");

  return (
    <Link
      href={getSuggestionHref(suggestion)}
      onClick={onNavigate}
      className="block rounded-lg border border-blue-100 bg-blue-50/70 p-2.5 text-left transition hover:border-primary hover:bg-blue-50"
    >
      <div className="flex items-start gap-2">
        {suggestion.flagUrl && (
          <Image
            src={suggestion.flagUrl}
            alt={suggestion.countryName || "Quốc gia"}
            width={22}
            height={16}
            className="mt-0.5 rounded-sm object-cover"
          />
        )}
        <div className="min-w-0 flex-1">
          <p className="line-clamp-2 text-xs font-semibold text-slate-800">
            {suggestion.packageName || suggestion.productName || "Gói eSIM phù hợp"}
          </p>
          <p className="mt-1 text-[11px] text-slate-500">
            {dataLabel || "Data linh hoạt"} · {suggestion.validityDays} ngày
          </p>
          <p className="mt-1 text-xs font-bold text-primary">
            {formatPrice(suggestion.salePrice, suggestion.currency)}
          </p>
        </div>
      </div>
    </Link>
  );
}

export default function ChatbotWidget() {
  const pathname = usePathname();
  const [open, setOpen] = useState(false);
  const [messages, setMessages] = useState<Message[]>(INITIAL_MESSAGES);
  const [input, setInput] = useState("");
  const [isSending, setIsSending] = useState(false);
  const [hasLoadedStoredMessages, setHasLoadedStoredMessages] = useState(false);
  const [mounted, setMounted] = useState(false);
  const [expandOrigin, setExpandOrigin] = useState("100% 100%");
  const sessionIdRef = useRef<string | null>(null);
  const messagesScrollRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const chatWindowRef = useRef<HTMLDivElement>(null);
  const buttonRef = useRef<HTMLButtonElement>(null);
  const previousPathnameRef = useRef(pathname);

  const closeChat = useCallback(() => setOpen(false), []);

  const openChat = useCallback(() => {
    if (buttonRef.current) {
      const rect = buttonRef.current.getBoundingClientRect();
      setExpandOrigin(`${rect.left + rect.width / 2}px ${rect.top + rect.height / 2}px`);
    }
    setOpen(true);
  }, []);

  const toggleChat = useCallback(() => {
    if (open) {
      setOpen(false);
    } else {
      openChat();
    }
  }, [open, openChat]);

  useEffect(() => {
    setMounted(true);
  }, []);

  const handleClickOutside = useCallback((e: MouseEvent) => {
    if (window.innerWidth < 768) return;
    if (
      chatWindowRef.current &&
      !chatWindowRef.current.contains(e.target as Node) &&
      buttonRef.current &&
      !buttonRef.current.contains(e.target as Node)
    ) {
      setOpen(false);
    }
  }, []);

  useEffect(() => {
    sessionIdRef.current = window.localStorage.getItem(CHATBOT_SESSION_STORAGE_KEY);
    const storedMessages = window.localStorage.getItem(CHATBOT_MESSAGES_STORAGE_KEY);

    if (storedMessages) {
      try {
        const parsedMessages = JSON.parse(storedMessages) as Message[];
        if (Array.isArray(parsedMessages) && parsedMessages.length > 0) {
          setMessages(parsedMessages);
        }
      } catch {
        window.localStorage.removeItem(CHATBOT_MESSAGES_STORAGE_KEY);
      }
    }

    setHasLoadedStoredMessages(true);
  }, []);

  useEffect(() => {
    if (!hasLoadedStoredMessages) return;
    window.localStorage.setItem(CHATBOT_MESSAGES_STORAGE_KEY, JSON.stringify(messages));
  }, [hasLoadedStoredMessages, messages]);

  useEffect(() => {
    if (!open) return;

    const previousOverflow = document.body.style.overflow;
    document.body.style.overflow = "hidden";

    return () => {
      document.body.style.overflow = previousOverflow;
    };
  }, [open]);

  useEffect(() => {
    if (open) {
      document.addEventListener("mousedown", handleClickOutside);
      return () => document.removeEventListener("mousedown", handleClickOutside);
    }
  }, [open, handleClickOutside]);

  useEffect(() => {
    if (!open) return;
    const scrollContainer = messagesScrollRef.current;
    if (!scrollContainer) return;
    scrollContainer.scrollTop = scrollContainer.scrollHeight;
  }, [messages, isSending, open]);

  useEffect(() => {
    if (open && inputRef.current && window.matchMedia("(min-width: 768px)").matches) {
      inputRef.current.focus();
    }
  }, [open]);

  useEffect(() => {
    if (previousPathnameRef.current !== pathname) {
      setOpen(false);
      previousPathnameRef.current = pathname;
    }
  }, [pathname]);

  const appendBotMessage = useCallback((text: string, suggestions?: ChatbotProductSuggestion[]) => {
    const botMsg: Message = {
      id: Date.now() + Math.random(),
      text,
      sender: "bot",
      time: getCurrentTime(),
      suggestions,
    };
    setMessages((prev) => [...prev, botMsg]);
  }, []);

  const handleSend = async () => {
    const text = input.trim();
    if (!text || isSending) return;

    const userMsg: Message = { id: Date.now(), text, sender: "user", time: getCurrentTime() };
    setMessages((prev) => [...prev, userMsg]);
    setInput("");
    setIsSending(true);

    try {
      const response = await sendChatbotMessage({
        sessionId: sessionIdRef.current,
        message: text,
      });

      if (response.sessionId) {
        sessionIdRef.current = response.sessionId;
        window.localStorage.setItem(CHATBOT_SESSION_STORAGE_KEY, response.sessionId);
      }

      appendBotMessage(
        response.message || "Mình chưa có câu trả lời phù hợp. Bạn có thể nói rõ hơn nhu cầu eSIM của mình không?",
        response.suggestions ?? undefined
      );
    } catch (error) {
      appendBotMessage(
        error instanceof Error
          ? error.message
          : "Chatbot đang bận. Vui lòng thử lại sau hoặc liên hệ Zalo/Hotline để được hỗ trợ nhanh."
      );
    } finally {
      setIsSending(false);
    }
  };

  const chatPanel =
    open && mounted ? (
      <div
        ref={chatWindowRef}
        role="dialog"
        aria-modal="true"
        aria-label="EZSIM Assistant"
        className="chatbot-panel-enter fixed z-[350] flex min-h-0 flex-col overflow-hidden bg-white max-md:left-0 max-md:right-0 max-md:top-0 max-md:bottom-0 max-md:h-[100dvh] max-md:w-[100dvw] max-md:max-h-[100dvh] max-md:rounded-none md:right-6 md:bottom-24 md:left-auto md:top-auto md:h-[min(460px,calc(100dvh-6rem))] md:w-[340px] md:max-w-[calc(100dvw-2rem)] md:rounded-2xl md:border md:border-gray-200 md:shadow-2xl"
        style={{ transformOrigin: expandOrigin }}
      >
        <div className="gradient-primary flex shrink-0 items-center gap-3 px-4 pb-3 pt-[max(0.75rem,env(safe-area-inset-top,0px))] md:py-3">
          <div className="flex h-9 w-9 items-center justify-center overflow-hidden rounded-full border-2 border-white/30 bg-white/20">
            <Image
              src="/logo_chatbot.jpg"
              alt="EZSIM Bot"
              width={36}
              height={36}
              className="h-full w-full rounded-full object-cover"
            />
          </div>
          <div className="min-w-0 flex-1">
            <p className="text-sm font-bold leading-tight text-white">EZSIM Assistant</p>
            <p className="flex items-center gap-1 text-xs text-white/70">
              <span className="inline-block h-1.5 w-1.5 rounded-full bg-green-400" />
              Trực tuyến
            </p>
          </div>
          <button
            type="button"
            onClick={closeChat}
            className="flex h-8 w-8 items-center justify-center rounded-full text-white/70 transition hover:bg-white/10 hover:text-white"
            aria-label="Đóng chat"
          >
            <svg width="16" height="16" viewBox="0 0 14 14" fill="none" aria-hidden>
              <path d="M1 1L13 13M13 1L1 13" stroke="currentColor" strokeWidth="2" strokeLinecap="round" />
            </svg>
          </button>
        </div>

        <div
          ref={messagesScrollRef}
          className="min-h-0 flex-1 overflow-y-auto overscroll-y-contain bg-gray-50/50 p-4 space-y-3"
          style={{ WebkitOverflowScrolling: "touch" }}
        >
          {messages.map((msg) => (
            <div key={msg.id} className={`flex ${msg.sender === "user" ? "justify-end" : "justify-start"}`}>
              <div
                className={`max-w-[82%] rounded-2xl px-3.5 py-2.5 text-sm leading-relaxed ${
                  msg.sender === "user"
                    ? "rounded-br-md bg-primary text-white"
                    : "rounded-bl-md border border-gray-100 bg-white text-gray-700 shadow-sm"
                }`}
              >
                <p>{msg.text}</p>
                {msg.suggestions && msg.suggestions.length > 0 && (
                  <div className="mt-2 space-y-2">
                    {msg.suggestions.slice(0, 3).map((suggestion) => (
                      <SuggestionCard
                        key={`${suggestion.esimPackageId}-${suggestion.productVariantId}`}
                        suggestion={suggestion}
                        onNavigate={closeChat}
                      />
                    ))}
                  </div>
                )}
                <p className={`mt-1 text-[10px] ${msg.sender === "user" ? "text-white/60" : "text-gray-400"}`}>
                  {msg.time}
                </p>
              </div>
            </div>
          ))}
          {isSending && (
            <div className="flex justify-start">
              <div className="rounded-2xl rounded-bl-md border border-gray-100 bg-white px-3.5 py-2.5 text-sm text-gray-500 shadow-sm">
                Đang trả lời...
              </div>
            </div>
          )}
        </div>

        <div className="shrink-0 border-t border-gray-100 bg-white p-3 pb-[max(0.75rem,env(safe-area-inset-bottom,0px))]">
          <form
            onSubmit={(e) => {
              e.preventDefault();
              void handleSend();
            }}
            className="flex items-center gap-2"
          >
            <input
              ref={inputRef}
              type="text"
              value={input}
              onChange={(e) => setInput(e.target.value)}
              placeholder="Nhập tin nhắn..."
              disabled={isSending}
              className="flex-1 rounded-full border border-gray-200 bg-gray-50 px-4 py-2.5 text-[16px] outline-none transition focus:border-primary focus:bg-white disabled:cursor-not-allowed disabled:opacity-60"
            />
            <button
              type="submit"
              disabled={!input.trim() || isSending}
              className="gradient-primary flex h-9 w-9 flex-shrink-0 items-center justify-center rounded-full text-white transition hover:scale-105 active:scale-95 disabled:opacity-40"
              aria-label="Gửi tin nhắn"
            >
              <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" aria-hidden>
                <path d="M2.01 21L23 12 2.01 3 2 10l15 2-15 2z" />
              </svg>
            </button>
          </form>
        </div>
      </div>
    ) : null;

  return (
    <>
      <div className="relative">
        <span
          aria-hidden
          className="absolute inset-0 animate-ping rounded-full bg-primary/30 [animation-delay:400ms] [animation-duration:2.4s]"
        />
        <button
          ref={buttonRef}
          type="button"
          onClick={toggleChat}
          aria-label="Mở chatbot hỗ trợ"
          aria-expanded={open}
          className="group relative inline-flex h-11 w-11 items-center justify-center overflow-hidden rounded-full border border-gray-200 bg-white shadow-[0_10px_30px_rgba(0,0,0,0.15)] transition-all duration-300 hover:-translate-y-0.5 hover:scale-110 hover:shadow-[0_14px_36px_rgba(0,0,0,0.2)] active:scale-95"
        >
          <Image
            src="/logo_chatbot.jpg"
            alt="AI Chatbot"
            width={44}
            height={44}
            className="h-full w-full rounded-full object-cover"
          />
        </button>
      </div>

      {chatPanel ? createPortal(chatPanel, document.body) : null}
    </>
  );
}
