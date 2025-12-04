using System.Web.Mvc;

namespace QL_KhachSan.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            // Chỉ cho nhân viên truy cập
            if (Session["LoaiTK"] == null || Session["LoaiTK"].ToString() != "NhanVien")
            {
                return RedirectToAction("DangNhapNhanVien", "Account");
            }

            return View();
        }
    }
}
