import type { Metadata } from "next";
import { SITE } from "@/lib/constants";

export const SITE_URL =
  process.env.NEXT_PUBLIC_SITE_URL ?? "https://ezsim.vn";

const DEFAULT_OG_IMAGE = "/images/og-default.jpg";

interface BuildMetadataOptions {
  /** Page-level title — will be wrapped by layout's title.template ("%s | EZSIM") */
  title?: string;
  /** Bypass the template entirely — use for homepage or brand-critical titles */
  absoluteTitle?: string;
  description: string;
  image?: string;
  imageAlt?: string;
  canonicalPath?: string;
  type?: "website" | "article";
  noIndex?: boolean;
}

export function buildMetadata({
  title,
  absoluteTitle,
  description,
  image = DEFAULT_OG_IMAGE,
  imageAlt,
  canonicalPath,
  type = "website",
  noIndex = false,
}: BuildMetadataOptions): Metadata {
  const ogTitle = absoluteTitle ?? title ?? SITE.name;
  const resolvedAlt = imageAlt ?? ogTitle;

  return {
    ...(absoluteTitle
      ? { title: { absolute: absoluteTitle } }
      : title
        ? { title }
        : {}),
    description,
    ...(canonicalPath && { alternates: { canonical: canonicalPath } }),
    openGraph: {
      title: ogTitle,
      description,
      ...(canonicalPath && { url: canonicalPath }),
      type,
      siteName: SITE.name,
      locale: "vi_VN",
      images: [{ url: image, width: 1200, height: 630, alt: resolvedAlt }],
    },
    twitter: {
      card: "summary_large_image",
      title: ogTitle,
      description,
      images: [{ url: image, alt: resolvedAlt }],
    },
    ...(noIndex && { robots: { index: false, follow: false } }),
  };
}
