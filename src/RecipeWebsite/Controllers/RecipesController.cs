using RecipeWebsite.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RecipeWebsite.Controllers
{
    public class RecipesController : Controller
    {
        private RecipeDbContext db = new RecipeDbContext();

        // GET: Recipes
        public ActionResult Index()
        {
            var recipes = db.Recipes.Include(r => r.Category).Include(r => r.User);
            return View(recipes.ToList());
        }

        // GET: Recipes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // GET: Recipes/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "Title,Description,PrepTime,CookTime,CategoryId")] Recipe recipe, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                // 1. Lấy UserId từ Session của người đang đăng nhập
                if (Session["UserId"] != null)
                {
                    recipe.UserId = (int)Session["UserId"];
                }
                else
                {
                    return RedirectToAction("Login", "Account");
                }

                // 2. Xử lý Upload hình ảnh
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Lấy tên file gốc
                    string fileName = Path.GetFileName(ImageFile.FileName);

                    // Đường dẫn tới thư mục lưu file trên server
                    string uploadPath = Server.MapPath("~/Content/Images/");

                    // KIỂM TRA VÀ TỰ ĐỘNG TẠO THƯ MỤC NẾU CHƯA CÓ
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Ghép đường dẫn thư mục và tên file
                    string path = Path.Combine(uploadPath, fileName);

                    // Lưu file
                    ImageFile.SaveAs(path);

                    // Lưu đường dẫn vào database
                    recipe.ImageUrl = "/Content/Images/" + fileName;
                }

                // 3. Gán thời gian tạo
                recipe.CreatedAt = DateTime.Now;

                db.Recipes.Add(recipe);
                db.SaveChanges();
                // Lưu xong công thức thì chuyển thẳng vào trang chi tiết của công thức đó
                return RedirectToAction("Details", new { id = recipe.RecipeId });
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", recipe.CategoryId);
            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", recipe.CategoryId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", recipe.UserId);
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RecipeId,Title,Description,ImageUrl,PrepTime,CookTime,CreatedAt,UserId,CategoryId")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", recipe.CategoryId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "Username", recipe.UserId);
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = db.Recipes.Find(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            db.Recipes.Remove(recipe);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
