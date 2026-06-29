import sanitizeHtml from "sanitize-html";

interface SafeHtmlContentProps {
  html: string;
}

export default function SafeHtmlContent({ html }: SafeHtmlContentProps) {
  // Server-side sanitization prevents unsafe HTML/script injection from CMS content.
  const sanitized = sanitizeHtml(html, {
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
      "em",
      "blockquote",
      "a",
      "img",
      "br",
      "hr",
      "code",
      "pre",
    ],
    allowedAttributes: {
      a: ["href", "target", "rel"],
      img: ["src", "alt", "title", "width", "height"],
      code: ["class"],
    },
    allowedSchemes: ["http", "https", "mailto"],
  });

  return (
    <div
      className="text-gray-700 leading-7 [&_h2]:text-xl [&_h2]:font-bold [&_h2]:text-navy [&_h2]:mt-8 [&_h2]:mb-3 [&_p]:mb-4 [&_ul]:list-disc [&_ul]:pl-6 [&_ol]:list-decimal [&_ol]:pl-6 [&_li]:mb-2 [&_blockquote]:border-l-4 [&_blockquote]:border-primary [&_blockquote]:pl-4 [&_blockquote]:italic [&_img]:rounded-xl [&_img]:my-4"
      dangerouslySetInnerHTML={{ __html: sanitized }}
    />
  );
}
