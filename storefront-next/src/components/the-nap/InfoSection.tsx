import Icon from "@/components/ui/Icon";
import type { IconName } from "@fortawesome/fontawesome-svg-core";

const infoItems: { icon: IconName; title: string; desc: string }[] = [
  {
    icon: "bolt",
    title: "Nhận mã trong 30 giây",
    desc: "Mã thẻ tự động gửi qua email + lưu trong tài khoản EZSIM ngay sau khi thanh toán thành công.",
  },
  {
    icon: "shield-alt",
    title: "Bảo hành 100% mã thẻ",
    desc: "Cam kết mã thẻ chính hãng, sử dụng được. Nếu lỗi → hoàn tiền hoặc đổi thẻ mới ngay lập tức.",
  },
  {
    icon: "percentage",
    title: "Tích điểm thưởng",
    desc: "Mỗi 10.000đ chi tiêu = 1 điểm. Tích điểm đổi thẻ free, giảm giá đơn sau, nâng hạng VIP.",
  },
];

const howtoSteps = [
  { num: 1, title: "Chọn nhà cung cấp", desc: "Click vào logo Viettel, Garena, Zing... muốn mua" },
  { num: 2, title: "Chọn mệnh giá",     desc: "10K đến 1 triệu - thấy giá sau chiết khấu ngay lập tức" },
  { num: 3, title: "Thanh toán",        desc: "Momo, ZaloPay, VNPay, Banking, QR — chọn cách bạn thích" },
  { num: 4, title: "Nhận mã thẻ",       desc: "Mã gửi qua email trong 30 giây, copy & nạp ngay" },
];

export function InfoRow() {
  return (
    <div
      className="grid grid-cols-1 md:grid-cols-3 gap-4"
      style={{ margin: "32px 0" }}
    >
      {infoItems.map((item) => (
        <div
          key={item.title}
          className="bg-white flex gap-4 items-start"
          style={{
            borderRadius: "16px",
            padding: "24px",
            border: "1px solid #E2E8F0",
          }}
        >
          <div
            className="flex items-center justify-center text-primary flex-shrink-0"
            style={{
              width: "48px",
              height: "48px",
              borderRadius: "12px",
              background: "rgba(0,102,255,0.1)",
              fontSize: "22px",
            }}
          >
            <Icon icon={item.icon} />
          </div>
          <div>
            <h4 className="font-bold" style={{ fontSize: "15px", marginBottom: "6px" }}>
              {item.title}
            </h4>
            <p className="text-gray-500" style={{ fontSize: "13px", lineHeight: 1.6 }}>
              {item.desc}
            </p>
          </div>
        </div>
      ))}
    </div>
  );
}

export function HowToBuy() {
  return (
    <div
      className="bg-white"
      style={{
        borderRadius: "20px",
        padding: "32px",
        border: "1px solid #E2E8F0",
        marginBottom: "32px",
      }}
    >
      <h2
        className="font-extrabold flex items-center gap-2.5"
        style={{
          fontSize: "24px",
          letterSpacing: "-0.5px",
          marginBottom: "20px",
        }}
      >
        <div
          className="gradient-primary"
          style={{ width: "4px", height: "24px", borderRadius: "2px" }}
        />
        Mua thẻ online tại EZSIM siêu dễ
      </h2>

      <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
        {howtoSteps.map((step) => (
          <div key={step.num} className="text-center">
            <div
              className="gradient-primary text-white font-extrabold flex items-center justify-center mx-auto"
              style={{
                width: "56px",
                height: "56px",
                borderRadius: "50%",
                fontSize: "22px",
                marginBottom: "12px",
              }}
            >
              {step.num}
            </div>
            <h4 className="font-bold" style={{ fontSize: "14px", marginBottom: "6px" }}>
              {step.title}
            </h4>
            <p className="text-gray-500" style={{ fontSize: "12px" }}>
              {step.desc}
            </p>
          </div>
        ))}
      </div>
    </div>
  );
}
