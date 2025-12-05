using QL_KhachSan.Models;
using System.Linq;
using System.Web.Mvc;

namespace QL_KhachSan.Controllers
{
    public class AccountController : Controller
    {
        QL_KhachSanEntities db = new QL_KhachSanEntities();

        // ================= ĐĂNG KÝ KHÁCH HÀNG =================

        [HttpGet]
        public ActionResult DangKyKhachHang()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKyKhachHang(string TenKH, string Gmail, string MatKhau)
        {
            if (string.IsNullOrWhiteSpace(TenKH) ||
                string.IsNullOrWhiteSpace(Gmail) ||
                string.IsNullOrWhiteSpace(MatKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ Tên, Gmail, Mật khẩu";
                return View();
            }

            string g = Gmail.Trim();

            bool existed = db.KhachHangs.Any(k => k.Gmail.Trim() == g);
            if (existed)
            {
                ViewBag.Error = "Gmail đã tồn tại";
                return View();
            }

            var kh = new KhachHang
            {
                TenKH = TenKH,
                Gmail = g,
                MatKhau = MatKhau.Trim()
            };

            db.KhachHangs.Add(kh);
            db.SaveChanges();

            Session["MaKH"] = kh.MaKH;
            Session["TenKH"] = kh.TenKH;
            Session["LoaiTK"] = "KhachHang";

            return RedirectToAction("Index", "Home");
        }

        // ================= ĐĂNG NHẬP KHÁCH HÀNG =================

        [HttpGet]
        public ActionResult DangNhapKhachHang()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhapKhachHang(string Gmail, string MatKhau)
        {
            if (string.IsNullOrWhiteSpace(Gmail) ||
                string.IsNullOrWhiteSpace(MatKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ Gmail và mật khẩu";
                return View();
            }

            string g = Gmail.Trim();
            string mk = MatKhau.Trim();

            var kh = db.KhachHangs
                       .FirstOrDefault(k => k.Gmail.Trim() == g && k.MatKhau.Trim() == mk);

            if (kh == null)
            {
                ViewBag.Error = "Sai Gmail hoặc mật khẩu";
                return View();
            }

            Session["MaKH"] = kh.MaKH;
            Session["TenKH"] = kh.TenKH;
            Session["LoaiTK"] = "KhachHang";

            return RedirectToAction("Index", "Home");
        }

        // ================= ĐĂNG NHẬP NHÂN VIÊN =================

        [HttpGet]
        public ActionResult DangNhapNhanVien()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangNhapNhanVien(string TaiKhoan, string MatKhau)
        {
            if (string.IsNullOrWhiteSpace(TaiKhoan) ||
                string.IsNullOrWhiteSpace(MatKhau))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ tài khoản và mật khẩu";
                return View();
            }

            string tk = TaiKhoan.Trim();
            string mk = MatKhau.Trim();

            var acc = db.TaiKhoanNVs
                        .FirstOrDefault(a => a.TaiKhoan.Trim() == tk && a.MatKhau.Trim() == mk);

            if (acc == null)
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            Session["MaNV"] = acc.MaNV;
            Session["LoaiTK"] = "NhanVien";

            return RedirectToAction("Index", "Admin");
        }

        // ================= ĐĂNG XUẤT =================

        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
