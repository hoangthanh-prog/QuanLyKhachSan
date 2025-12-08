using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QL_KhachSan.Models;


namespace QL_KhachSan.Areas.Admin.Controllers
{
    public class DatPhongGiupKhachController : Controller
    {
        private QL_KhachSanEntities db = new QL_KhachSanEntities();
        // GET: Admin/DatPhongGiupKhach
        public ActionResult ChonPhong()
        {
            var phong = db.Phongs.Where(p => p.TinhTrang == "Trống").ToList();
            return View(phong);
        }

        public ActionResult DatPhong(int id)
        {
            var phong = db.Phongs.Find(id);
            if (phong == null)
                return HttpNotFound();

            ViewBag.MaPhong = id;
            ViewBag.TenPhong = phong.TenPhong;

            return View();
        }

        [HttpPost]
        public ActionResult DatPhong(int maPhong, string tenKH, string gmail, DateTime ngayBD, DateTime ngayKT)
        {
            var kh = db.KhachHangs.FirstOrDefault(x => x.Gmail == gmail);
            if (kh == null)
            {
                kh = new KhachHang()
                {
                    TenKH = tenKH,
                    Gmail = gmail,
                    MatKhau = "",
                    GioiTinh = "",
                    TinhThanh = "",
                };
                db.KhachHangs.Add(kh);
                db.SaveChanges();
            }

            HoaDon hd = new HoaDon
            {
                MaKH = kh.MaKH,
                NgayLap = DateTime.Now,
                TinhTrang = false,  // chưa thanh toán
                MaNV = MaNhanVienDatPhong()
            };
            db.HoaDons.Add(hd);
            db.SaveChanges();


            ChiTietHD ct = new ChiTietHD
            {
                MaHD = hd.MaHD,
                MaPhong = maPhong,
                NgayBD = ngayBD,
                NgayKT = ngayKT
            };

            db.ChiTietHDs.Add(ct);

            // cập nhật trạng thái phòng
            var phong = db.Phongs.Find(maPhong);
            phong.TinhTrang = "Đã đặt";

            db.SaveChanges();

            TempData["success"] = "Đặt phòng thành công!";
            return RedirectToAction("DanhSachDatPhong");
        }

        public ActionResult DanhSachDatPhong()
        {
            var ds = db.HoaDons.OrderByDescending(h => h.NgayLap).ToList();
            return View(ds);
        }

        private int? MaNhanVienDatPhong()
        {
            if (Session["MaNV"] != null)
            {
                return Convert.ToInt32(Session["MaNV"]);
            }

            //nếu chưa login thì để null
            return null;
        }


    }
}