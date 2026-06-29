interface SectionHeadingProps {
  eyebrow?: string;
  title: string;
  highlight?: string;
  align?: "left" | "center";
}

export default function SectionHeading({
  eyebrow,
  title,
  highlight,
  align = "center",
}: SectionHeadingProps) {
  const isCenter = align === "center";
  return (
    <div className={`flex flex-col gap-1.5 ${isCenter ? "items-center text-center" : "items-start text-left"}`}>
      {eyebrow && (
        <span className="inline-block text-xs font-bold uppercase tracking-widest text-primary bg-primary-light px-3 py-1 rounded-full w-fit">
          {eyebrow}
        </span>
      )}
      <h2>
        {title}
        {highlight && (
          <>
            {" "}
            <span className="gradient-text">{highlight}</span>
          </>
        )}
      </h2>
    </div>
  );
}
