import { Breadcrumb } from "@/components/ui";
import { CountryHero, EsimCountryBrowser } from "@/components/esim";
import type { EsimCountryDetail } from "@/types/esim";
import type { ProductContent, ProductFaq } from "@/types/productContent";

const COUNTRY_FLAG_CODES: Record<string, string> = {
  "nhat-ban": "jp",
  "han-quoc": "kr",
  "thai-lan": "th",
  "chau-au": "eu",
  "my": "us",
};

interface EsimCountryViewProps {
  country: EsimCountryDetail;
  contents: ProductContent[];
  faqs: ProductFaq[];
}

export default function EsimCountryView({ country, contents, faqs }: EsimCountryViewProps) {
  const displayCountry = country.name.replace(/^eSIM\s+/, "");

  return (
    <>
      <Breadcrumb
        items={[
          { label: "eSIM Du lịch", href: "/esim-du-lich" },
          ...(country.region
            ? [{ label: country.region, href: `/esim-du-lich?region=${encodeURIComponent(country.region)}` }]
            : []),
          { label: displayCountry },
        ]}
      />

      <CountryHero
        slug={country.slug}
        flag={country.flag}
        flagCode={COUNTRY_FLAG_CODES[country.slug]}
        name={country.name}
        nameEn={country.nameEn}
        description={country.description}
        tags={country.tags}
        stats={country.stats}
        gradient={country.gradient}
        textColor={country.textColor}
        tagBg={country.tagBg}
      />

      <EsimCountryBrowser country={country} contents={contents} faqs={faqs} />
    </>
  );
}
