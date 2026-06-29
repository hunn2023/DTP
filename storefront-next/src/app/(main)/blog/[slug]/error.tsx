"use client";

export default function BlogDetailError({ reset }: { reset: () => void }) {
  return (
    <section className="max-w-container mx-auto px-4 md:px-6 py-12">
      <div className="bg-red-50 border border-red-200 rounded-2xl p-6 md:p-8 text-center">
        <h2 className="text-lg md:text-xl font-bold text-red-700">Không thể tải bài viết</h2>
        <p className="text-red-600 mt-2">Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.</p>
        <button type="button" onClick={reset} className="btn-primary mt-5">
          Thử lại
        </button>
      </div>
    </section>
  );
}
