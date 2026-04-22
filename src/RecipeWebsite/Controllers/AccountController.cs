using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using RecipeWebsite.Models;

namespace RecipeWebsite.Controllers
{
    public class AccountController : Controller
    {
        private RecipeDbContext db = new RecipeDbContext();

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                var check = db.Users.FirstOrDefault(s => s.Username == user.Username || s.Email == user.Email);
                if (check == null)
                {
                    user.Role = "Member"; // Mặc định tài khoản mới là Member
                    // TODO: Nên thêm hàm Hash mật khẩu ở đây trước khi lưu vào DB
                    db.Users.Add(user);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.Error = "Tên đăng nhập hoặc Email đã tồn tại!";
                }
            }
            return View(user);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {
                // TODO: Cần so sánh với mật khẩu đã Hash nếu ở trên có sử dụng Hash
                var data = db.Users.FirstOrDefault(s => s.Username == username && s.Password == password);
                if (data != null)
                {
                    // Khởi tạo Cookie xác thực
                    FormsAuthentication.SetAuthCookie(data.Username, false);

                    // Lưu thêm Session để tiện lấy thông tin hiển thị
                    Session["UserId"] = data.UserId;
                    Session["Username"] = data.Username;
                    Session["Role"] = data.Role;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                }
            }
            return View();
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}