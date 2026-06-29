export default function AccountSkeleton() {
  return (
    <div className="bg-gradient-to-br from-slate-50 via-blue-50/30 to-cyan-50/20 px-4 py-10 md:py-14 min-h-screen">
      <div className="max-w-5xl mx-auto animate-pulse">
        {/* Page heading skeleton */}
        <div className="mb-8 space-y-2">
          <div className="h-7 w-48 bg-gray-200 rounded-lg" />
          <div className="h-4 w-72 bg-gray-100 rounded" />
        </div>

        <div className="flex flex-col lg:flex-row gap-6">
          {/* Left card skeleton */}
          <div className="lg:w-72 flex-shrink-0 bg-white rounded-2xl overflow-hidden shadow-sm border border-gray-100">
            <div className="h-16 bg-gray-200" />
            <div className="px-6 pb-6">
              <div className="w-20 h-20 rounded-full bg-gray-200 mx-auto -mt-10 mb-4 border-4 border-white" />
              <div className="text-center space-y-2 mb-6">
                <div className="h-5 w-36 bg-gray-200 rounded mx-auto" />
                <div className="h-4 w-20 bg-gray-100 rounded mx-auto" />
              </div>
              <div className="space-y-3 mb-6">
                <div className="flex items-center gap-2.5">
                  <div className="w-7 h-7 rounded-full bg-gray-100 flex-shrink-0" />
                  <div className="h-4 flex-1 bg-gray-100 rounded" />
                </div>
                <div className="flex items-center gap-2.5">
                  <div className="w-7 h-7 rounded-full bg-gray-100 flex-shrink-0" />
                  <div className="h-4 w-28 bg-gray-100 rounded" />
                </div>
              </div>
              <div className="h-10 bg-gray-100 rounded-lg" />
            </div>
          </div>

          {/* Right card skeleton */}
          <div className="flex-1 bg-white rounded-2xl shadow-sm border border-gray-100 p-6 md:p-8">
            <div className="flex items-center gap-3 mb-6 pb-5 border-b border-gray-100">
              <div className="w-9 h-9 rounded-xl bg-gray-200 flex-shrink-0" />
              <div className="space-y-1.5">
                <div className="h-4 w-40 bg-gray-200 rounded" />
                <div className="h-3 w-56 bg-gray-100 rounded" />
              </div>
            </div>
            <div className="space-y-5">
              {[1, 2, 3, 4].map((i) => (
                <div key={i} className="space-y-1.5">
                  <div className="h-4 w-24 bg-gray-100 rounded" />
                  <div className="h-11 bg-gray-100 rounded-lg" />
                </div>
              ))}
              <div className="h-12 bg-gray-200 rounded-lg mt-2" />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
