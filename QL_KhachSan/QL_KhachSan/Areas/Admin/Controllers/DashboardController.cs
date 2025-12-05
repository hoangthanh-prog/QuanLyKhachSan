using System;
using System.Linq;
using System.Web.Mvc;
using QL_KhachSan.Models;
using QL_KhachSan.Models.ViewModels;

namespace QL_KhachSan.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private QL_KhachSanEntities db = new QL_KhachSanEntities();

        public ActionResult Index()
        {
            var model = new DashboardViewModel();

            // Tổng số phòng
            model.TongSoPhong = db.Phongs.Count();

            // Phòng đã đặt / phòng trống
            model.SoPhongDaDat = db.Phongs.Count(p => p.TinhTrang == "Đã đặt");
            model.SoPhongTrong = db.Phongs.Count(p => p.TinhTrang == "Trống");

            // Doanh thu tháng
            model.DoanhThuThang = db.HoaDons
                .Where(h => h.NgayLap.HasValue && h.NgayLap.Value.Month == DateTime.Now.Month)
                .Sum(h => (decimal?)h.TongTien) ?? 0;

            // Phòng theo loại
            model.PhongTheoLoai = db.BienThePhongs
                .Select(bt => new PhongTheoLoaiVM
                {
                    TenLoai = bt.BienThe,
                    Tong = bt.Phongs.Count(),
                    DaDat = bt.Phongs.Count(p => p.TinhTrang == "Đã đặt")
                }).ToList();

            // Đặt phòng gần đây
            model.BookingGanDay = db.ChiTietHDs
                .OrderByDescending(c => c.NgayBD)
                .Take(5)
                .Select(c => new BookingGanDayVM
                {
                    KhachHang = c.HoaDon.KhachHang.TenKH,
                    TenPhong = c.Phong.TenPhong,
                    TrangThai = "Đã đặt",
                    ThoiGian = c.NgayBD ?? DateTime.Now
                }).ToList();

            return View(model);
        }
    }
}
