using ProyectoFinal_G1_Autenticado.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoFinal_G1_Autenticado.Controllers
{
    public class ProductController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products
        [AllowAnonymous]
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Images).AsQueryable();

            if (!User.IsInRole("Administrador"))
            {
                products = products.Where(p => p.Status == ProductStatus.Activo);
            }

            return View(products.ToList());
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult ToggleStatus(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            product.Status = product.Status == ProductStatus.Activo
                ? ProductStatus.Inactivo
                : ProductStatus.Activo;

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        // GET: Products/Details/5
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var product = db.Products
                            .Include(p => p.Images)
                            .Include(p => p.Reviews)
                            .FirstOrDefault(p => p.Id == id);
            if (product == null) return HttpNotFound();

            product.Reviews = product.Reviews?.Where(r => r.Status == ReviewStatus.Aprobada).ToList();

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create(Product model, IEnumerable<HttpPostedFileBase> images)
        {
            if (!ModelState.IsValid) return View(model);

            var uploaded = images?.Where(i => i != null && i.ContentLength > 0).ToList() ?? new List<HttpPostedFileBase>();
            

            db.Products.Add(model);
            db.SaveChanges();

            int order = 1;
            foreach (var file in uploaded)
            {
                using (var ms = new System.IO.MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    var img = new ProductImage
                    {
                        ProductId = model.Id,
                        ImageData = ms.ToArray(),
                        FileName = System.IO.Path.GetFileName(file.FileName),
                        ContentType = file.ContentType,
                        SortOrder = order++
                    };
                    db.ProductImages.Add(img);
                }
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            var product = db.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(Product model, IEnumerable<HttpPostedFileBase> newImages)
        {
            if (!ModelState.IsValid) return View(model);

            var product = db.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == model.Id);
            if (product == null) return HttpNotFound();

            product.Name = model.Name;
            product.Code = model.Code;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.Status = model.Status;

            db.Entry(product).State = EntityState.Modified;
            db.SaveChanges();

            var uploaded = newImages?.Where(i => i != null && i.ContentLength > 0).ToList() ?? new List<HttpPostedFileBase>();
            if (uploaded.Any())
            {
                int maxOrder = product.Images.Any() ? product.Images.Max(i => i.SortOrder) : 0;
                foreach (var file in uploaded)
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        var img = new ProductImage
                        {
                            ProductId = product.Id,
                            ImageData = ms.ToArray(),
                            FileName = System.IO.Path.GetFileName(file.FileName),
                            ContentType = file.ContentType,
                            SortOrder = ++maxOrder
                        };
                        db.ProductImages.Add(img);
                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            var product = db.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult DeleteConfirmed(int id)
        {
            var product = db.Products.Include(p => p.Images).FirstOrDefault(p => p.Id == id);
            if (product == null) return HttpNotFound();

            var imgs = product.Images.ToList();
            foreach (var img in imgs)
            {
                db.ProductImages.Remove(img);
            }

            db.Products.Remove(product);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Products/GetImage/5
        [AllowAnonymous]
        public ActionResult GetImage(int id)
        {
            var img = db.ProductImages.Find(id);
            if (img == null) return HttpNotFound();

            return File(img.ImageData, img.ContentType);
        }

        // POST: Products/RemoveImage/5
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveImage(int id)
        {
            var img = db.ProductImages.Find(id);
            if (img == null) return HttpNotFound();

            db.ProductImages.Remove(img);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = img.ProductId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
