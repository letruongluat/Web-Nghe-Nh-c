using System.Linq;
using System.Web.Mvc;
using PlayMusic.Models;
using System;
using System.Web.Security;

namespace PlayMusic.Controllers
{
    public class LoginController : Controller
    {
        private MusicWebEntities1 db = new MusicWebEntities1(); // Replace with your DbContext

        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Username and password are required";
                return View();
            }

            try
            {
               
                var user = db.tblAccounts.FirstOrDefault(u => u.IDacount == username && u.Password == password);

                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.IDacount, false);
                    TempData["SuccessMessage"] = "Đăng nhập thành công!";

                    // Lưu ID của người dùng vào Session
                    Session["UserID"] = user.IDacount;

                    return RedirectToAction("Index", "Song");
                }

                else
                {
                    ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng";
                    return View();
                }
            }
            catch (Exception ex)
            {
                
                ViewBag.ErrorMessage = "Đã xảy ra lỗi: " + ex.Message;
                if (ex.InnerException != null)
                {
                    ViewBag.ErrorMessage += " Lỗi nội bộ: " + ex.InnerException.Message;
                }
                System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Lỗi nội bộ: {ex.InnerException.Message}");
                }
                return View();
            }
        }
        public ActionResult Logout()
        {
            
            FormsAuthentication.SignOut(); 
                                          
            return RedirectToAction("Index", "Login");
        }


    }
}
