using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTP.Modules.Catalog.Domain.Enums
{
    public enum ProductContentType
    {
        Overview = 1,          // Tổng quan
        HowToUse = 2,          // Hướng dẫn sử dụng
        ActivationGuide = 3,   // Hướng dẫn kích hoạt eSIM
        Policy = 4,            // Chính sách
        Note = 5,              // Lưu ý
        LandingBlock = 6       // Block nội dung landing page
    }
}
