import FAQSectionClient from "@/components/home/FAQSectionClient";
import { getFaqs } from "@/lib/api/faqApi";

export default async function FAQSection() {
  const items = await getFaqs();
  return <FAQSectionClient items={items} />;
}
