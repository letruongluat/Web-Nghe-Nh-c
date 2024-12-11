using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlayMusic.Controllers
{
    public class LoginAdminController : Controller
    {
        // GET: LoginAdmin
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Kiểm tra tài khoản và mật khẩu
            if (username == "admin" && password == "123456")
            {
                // Nếu đúng, chuyển hướng đến trang Admin
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                // Nếu sai, hiển thị thông báo lỗi
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }
        }
    }
}