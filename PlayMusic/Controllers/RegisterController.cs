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
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(tblAccount model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                  
                    if (db.tblAccounts.Any(a => a.IDacount == model.IDacount || a.email == model.email))
                    {
                        ModelState.AddModelError("", "Tên đăng nhập hoặc email đã tồn tại.");
                        return View(model);
                    }

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
            catch (Exception ex)
            {
                
                ViewBag.ErrorMessage = ex.Message;
                return View(model);
            }

            
            return View(model);
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
