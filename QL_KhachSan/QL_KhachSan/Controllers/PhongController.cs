using QL_KhachSan.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;


namespace QL_KhachSan.Controllers
{
    public class PhongController : Controller
    {
        QL_KhachSanEntities db = new QL_KhachSanEntities();

        public ActionResult DanhSachBienThe(string keyword, string priceRange, string sort = "gia_asc")
        {
            var listKhoangGia = new List<SelectListItem>
    {
        new SelectListItem { Text = "Dưới 500.000 VNĐ", Value = "0-500000" },
        new SelectListItem { Text = "500.000 - 1.000.000 VNĐ", Value = "500000-1000000" },
        new SelectListItem { Text = "1.000.000 - 2.000.000 VNĐ", Value = "1000000-2000000" },
        new SelectListItem { Text = "2.000.000 - 5.000.000 VNĐ", Value = "2000000-5000000" },
        new SelectListItem { Text = "Trên 5.000.000 VNĐ", Value = "5000000-100000000" }
    };
            ViewBag.PriceRangeList = new SelectList(listKhoangGia, "Value", "Text", priceRange);

            var danhSachHangPhong = db.BienThePhongs.Select(x => x.BienThe).Distinct().ToList();
            ViewBag.HangPhongList = new SelectList(danhSachHangPhong, keyword);

            var query = db.BienThePhongs
                          .Include("LoaiPhong")
                          .Include("Phongs")
                          .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(x => x.BienThe == keyword);
            }

            if (!string.IsNullOrEmpty(priceRange))
            {
                var parts = priceRange.Split('-');
                if (parts.Length == 2)
                {
                    decimal min = decimal.Parse(parts[0]);
                    decimal max = decimal.Parse(parts[1]);

                    query = query.Where(x => x.GiaBan >= min && x.GiaBan <= max);
                }
            }

            var list = query.ToList();

            switch (sort)
            {
                case "gia_desc": list = list.OrderByDescending(x => x.GiaBan).ToList(); break;
                case "trong_asc": list = list.OrderBy(x => x.Phongs.Count(p => p.TinhTrang.Trim() == "Trống")).ToList(); break;
                case "trong_desc": list = list.OrderByDescending(x => x.Phongs.Count(p => p.TinhTrang.Trim() == "Trống")).ToList(); break;
                default: list = list.OrderBy(x => x.GiaBan).ToList(); break;
            }

            ViewBag.Keyword = keyword;
            ViewBag.PriceRange = priceRange;
            ViewBag.Sort = sort;

            return View(list);
        }

        public ActionResult ChiTietBienThe(int id)
        {
            var btp = db.BienThePhongs
                        .Include("LoaiPhong")
                        .Include("Phongs")
                        .FirstOrDefault(x => x.MaBienThe == id);

            if (btp == null)
                return HttpNotFound();

            var lienQuan = db.BienThePhongs
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

            var bienThe = db.BienThePhongs.FirstOrDefault(b => b.MaBienThe == maBienThe);
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
            db.HoaDons.Add(hd);
            db.SaveChanges();

            decimal tongTien = 0;

            var listPhong = db.Phongs.Where(p => maPhong.Contains(p.MaPhong)).ToList();

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
                    db.ChiTietHDs.Add(ct);

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
