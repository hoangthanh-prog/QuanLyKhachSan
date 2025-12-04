using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyKhachSan_LTW.Models;

namespace QL_KhachSan.Controllers
{
    public class HomeController : Controller
    {
        QL_KhachSanEntities db = new QL_KhachSanEntities();

        public ActionResult Index()
        {
            var loaiPhongList = db.BienThePhong
                                  .Include("HinhAnh")
                                  .Include("LoaiPhong")
                                  .Include("Phong")
                                  .ToList();

            var phongNoiBat = loaiPhongList
                                .OrderByDescending(x => x.GiaBan)
                                .Take(3)
                                .ToList();

            ViewBag.LoaiPhongList = loaiPhongList;
            ViewBag.PhongNoiBat = phongNoiBat;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TimPhong(DateTime? ngayDen, DateTime? ngayDi, int? maBienThe)
        {
            TempData["NgayDen"] = ngayDen;
            TempData["NgayDi"] = ngayDi;

            if (maBienThe.HasValue && maBienThe.Value > 0)
            {
                return RedirectToAction("ChiTietBienThe", "Phong", new { id = maBienThe.Value });
            }

            return RedirectToAction("DanhSachBienThe", "Phong");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}