import PopularDestinationsClient from "@/components/home/PopularDestinationsClient";
import { getHomeEsimProducts } from "@/lib/api/esimApi";

export default async function PopularDestinations() {
  const products = await getHomeEsimProducts();
  return <PopularDestinationsClient products={products} />;
}
