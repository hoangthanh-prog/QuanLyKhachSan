using System.Linq;
using System.Net;
using System.Web.Mvc;
using QL_KhachSan.Models;
using System.Data.Entity;
using QL_KhachSan.Models.ViewModels;


namespace QL_KhachSan.Areas.Admin.Controllers
{
    public class PhongController : Controller
    {
        private QL_KhachSanEntities1 db = new QL_KhachSanEntities1();

        // GET: Admin/Phong
        public ActionResult Index()
        {
            var phongs = db.Phongs.ToList();
            return View(phongs);
        }

        // GET: Admin/Phong/Details/5
        public ActionResult Details(int? id)
        {
            var phong = db.Phongs.Find(id);
            var hinh = db.HinhAnhs.FirstOrDefault(h => h.MaBienThe == id);

            var vm = new PhongDetailVM
            {
                Phong = phong,
                Hinh = hinh
            };

            return View(vm);
        }

        // GET: Admin/Phong/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Phong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Phong phong)
        {
            if (ModelState.IsValid)
            {
                db.Phongs.Add(phong);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(phong);
        }

        // GET: Admin/Phong/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Phong phong = db.Phongs.Find(id);
            if (phong == null) return HttpNotFound();

            return View(phong);
        }

        // POST: Admin/Phong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Phong phong)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phong).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(phong);
        }

        // GET: Admin/Phong/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Phong phong = db.Phongs.Find(id);
            if (phong == null) return HttpNotFound();

            return View(phong);
        }

        // POST: Admin/Phong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Phong phong = db.Phongs.Find(id);
            db.Phongs.Remove(phong);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
