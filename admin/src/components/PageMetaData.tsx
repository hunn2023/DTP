import { APP_DESCRIPTION, APP_KEYWORDS, APP_NAME } from '@/shared/config/brand'

type PageMetaDataProps = {
  title: string
  description?: string
  keywords?: string
}

const defaultPageMetaData: PageMetaDataProps = {
  title: APP_NAME,
  description: APP_DESCRIPTION,
  keywords: APP_KEYWORDS,
}

const PageMetaData = ({ title, description = defaultPageMetaData.description, keywords = defaultPageMetaData.keywords }: PageMetaDataProps) => {
  return (
    <>
      <title>{title ? `${title} | ${defaultPageMetaData.title}` : defaultPageMetaData.title}</title>
      <meta name="description" content={description} />
      <meta name="keywords" content={keywords} />
    </>
  )
}
export default PageMetaData
