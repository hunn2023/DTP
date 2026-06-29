"use client";

import { ImgHTMLAttributes, useEffect, useRef, useState } from "react";

interface OptimizedImageProps extends Omit<ImgHTMLAttributes<HTMLImageElement>, "src"> {
  src: string;
  alt: string;
  width?: number;
  height?: number;
  /** Load immediately (above the fold) */
  priority?: boolean;
  /** CSS object-fit */
  objectFit?: "cover" | "contain" | "fill" | "none";
  /** Show shimmer placeholder while loading */
  shimmer?: boolean;
}

/**
 * Optimized image component for static export (output: "export").
 * - Native lazy loading with IntersectionObserver preload
 * - Fade-in on load
 * - Shimmer placeholder
 * - Proper decoding="async"
 * - fetchpriority for above-the-fold images
 */
export default function OptimizedImage({
  src,
  alt,
  width,
  height,
  priority = false,
  objectFit = "cover",
  shimmer = true,
  className = "",
  style,
  ...rest
}: OptimizedImageProps) {
  const [loaded, setLoaded] = useState(false);
  const [inView, setInView] = useState(priority);
  const imgRef = useRef<HTMLImageElement>(null);

  useEffect(() => {
    if (priority) return;
    const el = imgRef.current;
    if (!el) return;

    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) {
          setInView(true);
          observer.disconnect();
        }
      },
      { rootMargin: "200px" }
    );
    observer.observe(el);
    return () => observer.disconnect();
  }, [priority]);

  return (
    <span
      ref={imgRef}
      className={`inline-block overflow-hidden ${className}`}
      style={{
        position: "relative",
        width: width ? `${width}px` : "100%",
        height: height ? `${height}px` : "100%",
        ...style,
      }}
    >
      {/* Shimmer placeholder */}
      {shimmer && !loaded && (
        <span
          aria-hidden
          className="absolute inset-0 bg-gradient-to-r from-gray-200 via-gray-100 to-gray-200 animate-[shimmer_1.5s_infinite]"
          style={{ backgroundSize: "200% 100%" }}
        />
      )}

      {inView && (
        <img
          src={src}
          alt={alt}
          width={width}
          height={height}
          loading={priority ? "eager" : "lazy"}
          decoding="async"
          fetchPriority={priority ? "high" : "auto"}
          onLoad={() => setLoaded(true)}
          className={`transition-opacity duration-300 ${loaded ? "opacity-100" : "opacity-0"}`}
          style={{
            objectFit,
            width: "100%",
            height: "100%",
          }}
          {...rest}
        />
      )}
    </span>
  );
}
