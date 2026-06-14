export function withLazyImages(html: string): string {
  if (!html) return html
  return html.replace(/<img\b([^>]*)>/gi, (tag, attrs: string) => {
    if (/loading\s*=/i.test(attrs)) return tag
    return `<img loading="lazy" decoding="async"${attrs}>`
  })
}
