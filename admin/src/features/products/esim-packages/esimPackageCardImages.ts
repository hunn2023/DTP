import esim1 from '@/assets/images/gallery/esim1.png'
import esim2 from '@/assets/images/gallery/esim2.png'
import esim3 from '@/assets/images/gallery/esim3.png'
import esim4 from '@/assets/images/gallery/esim4.png'
import esim5 from '@/assets/images/gallery/esim5.png'

const ESIM_CARD_IMAGES = [esim1, esim2, esim3, esim4, esim5]

export function getEsimPackageCardImage(packageId: string): string {
  let hash = 0
  for (let i = 0; i < packageId.length; i += 1) {
    hash = (hash + packageId.charCodeAt(i)) % ESIM_CARD_IMAGES.length
  }
  return ESIM_CARD_IMAGES[hash] ?? ESIM_CARD_IMAGES[0]
}
