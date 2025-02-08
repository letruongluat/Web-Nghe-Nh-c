using System;
using System.Linq;
using System.Web.Mvc;
using PlayMusic.Models;

namespace PlayMusic.Controllers
{
    public class RegisterController : Controller
    {
        private MusicWebEntities1 db = new MusicWebEntities1(); 
        // GET: Register
        [HttpGet] // Phương thức HTTP GET để hiển thị trang đăng ký
        public ActionResult Register()
        {
            return View(); // Trả về view hiển thị form đăng ký
        }

        // POST: Register
        [HttpPost] // Phương thức HTTP POST để xử lý dữ liệu từ form
        [ValidateAntiForgeryToken] // Bảo vệ chống tấn công CSRF
        public ActionResult Register(tblAccount model)
        {
            try
            {
                if (ModelState.IsValid) // Kiểm tra dữ liệu từ người dùng có hợp lệ không
                {
                    
                    if (db.tblAccounts.Any(a => a.IDacount == model.IDacount || a.email == model.email))
                    {
                        // Thêm lỗi vào ModelState nếu trùng tên đăng nhập hoặc email
                        ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại.");
                        return View(model); // Trả lại view với dữ liệu hiện tại
                    }

                    // Tạo đối tượng tài khoản mới từ dữ liệu người dùng nhập
                    var account = new tblAccount
                    {
                        IDacount = model.IDacount, 
                        Password = model.Password,
                        Name = model.Name, 
                        email = model.email, 
                        ngaysinh = model.ngaysinh, 
                        gioitinh = model.gioitinh
                    };

                    
                    db.tblAccounts.Add(account);
                    db.SaveChanges(); 

                  
                    TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";

                   
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex) // Xử lý lỗi trong quá trình lưu dữ liệu
            {
                
                ViewBag.ErrorMessage = ex.Message;
                return View(model); 
            }

            
            return View(model);
        }

        // Phương thức Dispose để giải phóng tài nguyên khi controller không còn sử dụng
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); // Giải phóng kết nối cơ sở dữ liệu
            }
            base.Dispose(disposing); // Gọi Dispose từ lớp cơ sở
        }
    }
}
