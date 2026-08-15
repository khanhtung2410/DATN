using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuahangchamsocthucung.Enum
{
    public enum TrangThaiLichChamSoc
    {
        ChoXacNhan = 0,   // Khách vừa đặt lịch, chờ nhân viên xác nhận
        DaXacNhan = 1,    // Đã xác nhận lịch
        DangDienRa = 2,   // Đang thực hiện dịch vụ
        DaHuy = 3,        // Lịch bị hủy
        HoanThanh = 4     // Đã hoàn thành dịch vụ
    }
}
