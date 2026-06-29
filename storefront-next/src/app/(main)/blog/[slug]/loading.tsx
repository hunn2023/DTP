export default function BlogDetailLoading() {
  return (
    <section className="max-w-container mx-auto px-4 md:px-6 py-8">
      <div className="max-w-4xl mx-auto animate-pulse space-y-4">
        <div className="h-4 bg-gray-200 rounded w-44" />
        <div className="h-10 bg-gray-200 rounded w-4/5" />
        <div className="aspect-[16/9] bg-gray-200 rounded-2xl" />
        <div className="h-4 bg-gray-200 rounded w-full" />
        <div className="h-4 bg-gray-200 rounded w-11/12" />
        <div className="h-4 bg-gray-200 rounded w-10/12" />
      </div>
    </section>
  );
}
