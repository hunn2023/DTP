"use client";

export default function BlogError({ reset }: { reset: () => void }) {
  return (
    <section className="max-w-container mx-auto px-4 md:px-6 py-12">
      <div className="bg-red-50 border border-red-200 rounded-2xl p-6 md:p-8 text-center">
        <h2 className="text-lg md:text-xl font-bold text-red-700">Có lỗi xảy ra</h2>
        <p className="text-red-600 mt-2">Không thể tải nội dung blog vào lúc này.</p>
        <button type="button" onClick={reset} className="btn-primary mt-5">
          Thử lại
        </button>
      </div>
    </section>
  );
}
