using System;
using System.Linq;
using System.Web.Mvc;
using QuanLyKhachSan_LTW.Models;

namespace QL_KhachSan.Controllers
{
    public class PhongController : Controller
    {
        QL_KhachSanEntities db = new QL_KhachSanEntities();

        public ActionResult DanhSachBienThe(string keyword, decimal? minPrice, decimal? maxPrice, string sort = "gia_asc")
        {
            var list = db.BienThePhong
                         .Include("LoaiPhong")
                         .Include("Phong")
                         .ToList();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim();
                list = list.Where(x => x.BienThe.Contains(kw)).ToList();
            }

            if (minPrice.HasValue)
                list = list.Where(x => x.GiaBan >= minPrice.Value).ToList();

            if (maxPrice.HasValue)
                list = list.Where(x => x.GiaBan <= maxPrice.Value).ToList();

            switch (sort)
            {
                case "gia_desc":
                    list = list.OrderByDescending(x => x.GiaBan).ToList();
                    break;
                case "trong_asc":
                    list = list.OrderBy(x => x.Phong.Count(p => p.TinhTrang.Trim() == "Trống")).ToList();
                    break;
                case "trong_desc":
                    list = list.OrderByDescending(x => x.Phong.Count(p => p.TinhTrang.Trim() == "Trống")).ToList();
                    break;
                default:
                    list = list.OrderBy(x => x.GiaBan).ToList();
                    break;
            }

            ViewBag.Keyword = keyword;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.Sort = sort;

            return View(list);
        }

        public ActionResult ChiTietBienThe(int id)
        {
            var btp = db.BienThePhong
                        .Include("LoaiPhong")
                        .Include("Phong")
                        .FirstOrDefault(x => x.MaBienThe == id);

            if (btp == null)
                return HttpNotFound();

            var lienQuan = db.BienThePhong
                             .Where(x => x.MaLoai == btp.MaLoai && x.MaBienThe != id)
                             .ToList();

            ViewBag.LienQuan = lienQuan;

            DateTime? ngayDen = null, ngayDi = null;
            if (TempData["NgayDen"] != null) ngayDen = (DateTime)TempData["NgayDen"];
            if (TempData["NgayDi"] != null) ngayDi = (DateTime)TempData["NgayDi"];

            ViewBag.NgayDen = ngayDen;
            ViewBag.NgayDi = ngayDi;

            return View(btp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DatPhong(int maBienThe, int[] maPhong, DateTime? ngayDen, DateTime? ngayDi)
        {
            if (Session["MaKH"] == null)
                return RedirectToAction("DangNhapKhachHang", "Account");

            if (maPhong == null || maPhong.Length == 0)
            {
                TempData["Message"] = "Vui lòng chọn ít nhất một phòng để đặt.";
                return RedirectToAction("ChiTietBienThe", new { id = maBienThe });
            }

            DateTime start = ngayDen ?? DateTime.Today;
            DateTime end = ngayDi ?? DateTime.Today.AddDays(1);
            if (end <= start)
            {
                end = start.AddDays(1);
            }
            int soNgay = (end - start).Days;

            var bienThe = db.BienThePhong.FirstOrDefault(b => b.MaBienThe == maBienThe);
            if (bienThe == null)
                return RedirectToAction("DanhSachBienThe");

            decimal gia = bienThe.GiaBan ?? 0;

            int maKh = Convert.ToInt32(Session["MaKH"]);
            var hd = new HoaDon
            {
                MaKH = maKh,
                NgayLap = DateTime.Now,
                TinhTrang = false,
                TongTien = 0
            };
            db.HoaDon.Add(hd);
            db.SaveChanges();

            decimal tongTien = 0;

            var listPhong = db.Phong.Where(p => maPhong.Contains(p.MaPhong)).ToList();

            foreach (var p in listPhong)
            {
                if ((p.TinhTrang ?? "").Trim() == "Trống")
                {
                    var ct = new ChiTietHD
                    {
                        MaHD = hd.MaHD,
                        MaPhong = p.MaPhong,
                        NgayBD = start,
                        NgayKT = end,
                        SoNgay = soNgay
                    };
                    db.ChiTietHD.Add(ct);

                    tongTien += gia * soNgay;

                    p.TinhTrang = "Đã đặt";
                }
            }

            hd.TongTien = tongTien;
            db.SaveChanges();

            TempData["Message"] = "Đặt phòng thành công! Mã hóa đơn: " + hd.MaHD;
            TempData["NgayDen"] = start;
            TempData["NgayDi"] = end;

            return RedirectToAction("ChiTietBienThe", new { id = maBienThe });
        }
    }
}
