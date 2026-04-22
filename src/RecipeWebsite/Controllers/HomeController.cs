using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using RecipeWebsite.Models;

namespace RecipeWebsite.Controllers
{
    public class HomeController : Controller
    {
        private RecipeDbContext db = new RecipeDbContext();
        public ActionResult Index(string searchString)
        {
            // 1. Tạo câu truy vấn cơ bản (chưa thực thi xuống DB)
            var recipes = db.Recipes
                            .Include(r => r.User)
                            .Include(r => r.Category)
                            .AsQueryable();

            // 2. Kiểm tra nếu có từ khóa tìm kiếm thì thêm điều kiện Where
            if (!string.IsNullOrEmpty(searchString))
            {
                recipes = recipes.Where(r => r.Title.Contains(searchString)
                                          || r.Category.CategoryName.Contains(searchString));
            }

            // 3. Sắp xếp mới nhất lên đầu
            recipes = recipes.OrderByDescending(r => r.CreatedAt);

            // 4. Lưu lại từ khóa vào ViewBag để hiển thị lại trên ô input (giữ trạng thái)
            ViewBag.CurrentFilter = searchString;

            // Thực thi truy vấn và trả về View
            return View(recipes.ToList());
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