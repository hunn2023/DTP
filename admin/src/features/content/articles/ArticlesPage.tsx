import ArticlesCrudTable from '@/features/content/articles/components/ArticlesCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const ArticlesPage = () => (
  <EntityPageLayout
    title="Bài viết"
    subtitle="Website & SEO"
    description="Quản lý blog bài viết — đồng bộ API admin/content/articles.">
    <ArticlesCrudTable />
  </EntityPageLayout>
)

export default ArticlesPage
