import type { Config } from "tailwindcss";

const config: Config = {
  content: ["./src/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        // Mockup exact colors
        primary: "#0066FF",
        "primary-dark": "#0052CC",
        "primary-light": "#E6F0FF",
        cyan: "#00D4FF",
        navy: "#0A1628",
        "navy-light": "#1A2A44",

        // Mockup grayscale (exact from :root)
        gray: {
          50: "#F8FAFC",
          100: "#F1F5F9",
          200: "#E2E8F0",
          300: "#CBD5E1",
          400: "#94A3B8",
          500: "#64748B",
          600: "#475569",
          700: "#334155",
          800: "#1E293B",
          900: "#0F172A",
        },

        // Utility colors (keep for logic)
        danger: "#EF4444",
        "danger-light": "#FEE2E2",
        success: "#10B981",
        "success-light": "#D1FAE5",
        warning: "#F59E0B",
        "warning-light": "#FEF3C7",
      },
      fontFamily: {
        sans: ['"Be Vietnam Pro"', "sans-serif"],
      },
      fontSize: {
        // Mockup exact typography scale
        xs: ["12px", { lineHeight: "16px" }],
        sm: ["13px", { lineHeight: "18px" }],
        base: ["14px", { lineHeight: "22px" }],  // body: 14px / 1.6
        md: ["15px", { lineHeight: "24px" }],
        lg: ["18px", { lineHeight: "28px" }],
        xl: ["20px", { lineHeight: "28px" }],
        "2xl": ["24px", { lineHeight: "32px" }],
        "3xl": ["32px", { lineHeight: "40px" }],  // h2: 32px
        "4xl": ["48px", { lineHeight: "56px" }],  // h1: 48px
      },
      letterSpacing: {
        tighter: "-1px",      // h1
        tight: "-0.5px",      // h2
      },
      maxWidth: {
        container: "1280px",
      },
      borderRadius: {
        sm: "6px",
        DEFAULT: "8px",
        md: "10px",
        lg: "12px",
        xl: "16px",
        "2xl": "20px",        // mockup card radius
      },
      boxShadow: {
        card: "0 2px 8px rgba(0,0,0,0.06)",
        "card-hover": "0 8px 24px rgba(0,0,0,0.1)",
        btn: "0 2px 4px rgba(0,102,255,0.2)",
        hero: "0 20px 60px rgba(0,0,0,0.2)",  // mockup hero search shadow
      },
      spacing: {
        // Mockup uses 64px section padding, 28px card padding
        18: "4.5rem",
        22: "5.5rem",
        28: "7rem",           // 112px for large sections
      },
      backgroundImage: {
        "gradient-primary": "linear-gradient(135deg, #0066FF 0%, #00D4FF 100%)",
      },
    },
  },
  plugins: [],
};
export default config;
