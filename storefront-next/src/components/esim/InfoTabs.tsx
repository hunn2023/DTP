"use client";

import { useMemo, useState } from "react";
import sanitizeHtml from "sanitize-html";
import Icon from "@/components/ui/Icon";
import type { ProductContent, ProductFaq } from "@/types/productContent";

interface InfoTabsProps {
  contents: ProductContent[];
  faqs: ProductFaq[];
}

type Tab =
  | { key: string; label: string; type: "content"; content: ProductContent }
  | { key: string; label: string; type: "faq" };

const FALLBACK_CONTENT_LABELS: Record<number, string> = {
  1: "Giới thiệu",
  2: "Hướng dẫn cài đặt",
  3: "Thiết bị tương thích",
  4: "Chính sách sử dụng",
  5: "Phạm vi phủ sóng",
  6: "Thông tin thêm",
};

function getContentLabel(content: ProductContent) {
  return (
    content.contentTypeName ||
    content.title ||
    FALLBACK_CONTENT_LABELS[content.contentType] ||
    "Thông tin"
  );
}

function sanitizeProductHtml(html: string) {
  return sanitizeHtml(html, {
    allowedTags: [
      "p",
      "h1",
      "h2",
      "h3",
      "h4",
      "h5",
      "h6",
      "ul",
      "ol",
      "li",
      "strong",
      "b",
      "em",
      "i",
      "u",
      "blockquote",
      "a",
      "img",
      "br",
      "hr",
      "code",
      "pre",
      "span",
      "table",
      "thead",
      "tbody",
      "tr",
      "th",
      "td",
    ],
    allowedAttributes: {
      a: ["href", "target", "rel"],
      img: ["src", "alt", "title", "width", "height"],
      span: ["class"],
      li: ["data-list"],
      code: ["class"],
    },
    allowedSchemes: ["http", "https", "mailto"],
  });
}

export default function InfoTabs({ contents, faqs }: InfoTabsProps) {
  const [activeTab, setActiveTab] = useState(0);
  const [openFaq, setOpenFaq] = useState(0);

  const tabs = useMemo<Tab[]>(() => {
    const contentTabs = contents.map((content) => ({
      key: `content-${content.id}`,
      label: getContentLabel(content),
      type: "content" as const,
      content,
    }));

    return faqs.length > 0
      ? [...contentTabs, { key: "faqs", label: "Câu hỏi thường gặp", type: "faq" as const }]
      : contentTabs;
  }, [contents, faqs]);

  const sanitizedContentHtml = useMemo(() => {
    return contents.reduce<Record<string, string>>((acc, content) => {
      acc[content.id] = sanitizeProductHtml(content.bodyHtml || "");
      return acc;
    }, {});
  }, [contents]);

  if (tabs.length === 0) return null;

  const currentTab = tabs[Math.min(activeTab, tabs.length - 1)];

  return (
    <div className="mt-8 bg-white rounded-2xl p-5 md:p-8 border border-gray-200">
      <div className="flex gap-4 md:gap-6 border-b border-gray-200 mb-6 overflow-x-auto">
        {tabs.map((tab, i) => (
          <button
            key={tab.key}
            type="button"
            onClick={() => setActiveTab(i)}
            className={`shrink-0 py-3 font-semibold text-sm cursor-pointer border-b-2 -mb-px transition bg-transparent ${
              activeTab === i ? "text-primary border-primary" : "text-gray-500 border-transparent"
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {currentTab.type === "content" ? (
        <div className="text-sm text-gray-700 leading-7">
          {currentTab.content.title && (
            <h3 className="text-lg font-bold text-navy mb-3">{currentTab.content.title}</h3>
          )}
          {currentTab.content.summary && (
            <p className="mb-4 text-gray-600">{currentTab.content.summary}</p>
          )}
          {currentTab.content.bodyHtml && (
            <div
              className="product-content-html [&_h1]:text-xl [&_h1]:font-bold [&_h1]:text-navy [&_h1]:mb-3 [&_h2]:text-lg [&_h2]:font-bold [&_h2]:text-navy [&_h2]:mt-6 [&_h2]:mb-3 [&_h3]:font-bold [&_h3]:text-navy [&_h3]:mt-5 [&_h3]:mb-2 [&_p]:mb-4 [&_ul]:list-disc [&_ul]:pl-6 [&_ol]:list-decimal [&_ol]:pl-6 [&_li]:mb-2 [&_blockquote]:border-l-4 [&_blockquote]:border-primary [&_blockquote]:pl-4 [&_blockquote]:italic [&_img]:rounded-xl [&_img]:my-4 [&_a]:text-primary [&_a]:font-semibold"
              dangerouslySetInnerHTML={{ __html: sanitizedContentHtml[currentTab.content.id] }}
            />
          )}
        </div>
      ) : (
        <div className="flex flex-col gap-2.5">
          {faqs.map((faq, i) => {
            const open = openFaq === i;
            return (
              <div
                key={faq.id}
                className="bg-white rounded-xl overflow-hidden cursor-pointer"
                style={{
                  border: open ? "1.5px solid #0066FF" : "1.5px solid #E2E8F0",
                  boxShadow: open ? "0 2px 16px rgba(0,102,255,0.08)" : "none",
                  transition: "border-color 0.2s, box-shadow 0.2s",
                }}
                onClick={() => setOpenFaq(open ? -1 : i)}
              >
                {/* Question row */}
                <div
                  className="flex items-start gap-3"
                  style={{ padding: "14px 16px" }}
                >
                  <span
                    className="shrink-0 font-bold text-xs leading-none flex items-center justify-center"
                    style={{
                      width: "26px",
                      height: "26px",
                      borderRadius: "7px",
                      background: open ? "#0066FF" : "#F1F5F9",
                      color: open ? "#fff" : "#94A3B8",
                      transition: "background 0.2s, color 0.2s",
                      marginTop: "1px",
                    }}
                    aria-hidden
                  >
                    {String(i + 1).padStart(2, "0")}
                  </span>

                  <span className="flex-1 font-semibold text-sm text-navy leading-snug">
                    {faq.question}
                  </span>

                  <span
                    className="shrink-0"
                    style={{
                      transform: open ? "rotate(180deg)" : "rotate(0deg)",
                      transition: "transform 0.2s",
                      marginTop: "2px",
                    }}
                  >
                    <Icon
                      icon="chevron-down"
                      className={open ? "text-primary" : "text-slate-400"}
                    />
                  </span>
                </div>

                {/* Answer */}
                {open && (
                  <p
                    className="text-[13px] text-gray-700 leading-7"
                    style={{ padding: "0 16px 14px calc(16px + 26px + 12px)" }}
                  >
                    {faq.answer}
                  </p>
                )}
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}

