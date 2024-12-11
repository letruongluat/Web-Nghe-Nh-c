using PlayMusic.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlayMusic.Controllers
{
    public class HomeController : Controller
    {
        private MusicWebEntities1 db = new MusicWebEntities1();

        public ActionResult Index()
        {
            return View("~/Views/Login/Index.cshtml");
        }

        public ActionResult LoggedInHome()
        {
            return View("~/Views/Login/Index.cshtml");
        }

        public ActionResult NewMusic()
        {
            var newMusic = db.tblMusics.OrderByDescending(m => m.DisplayName).Take(10).ToList();
            return View(newMusic);
        }

        public ActionResult Genres()
        {
            // Lấy tất cả chủ đề từ cơ sở dữ liệu
            var allChudes = db.chudes.ToList();

            // Lọc chủ đề duy nhất dựa trên theloai1
            var uniqueChudeList = allChudes
                .GroupBy(c => c.theloai1)
                .Select(g => g.FirstOrDefault())
                .Where(c => !string.IsNullOrEmpty(c.theloai1))
                .ToList();

            // Trả danh sách chủ đề duy nhất về view
            return View(uniqueChudeList);
        }


        public ActionResult GetSongsByChude(int id)
        {
            var songs = db.tblMusics.Where(m => m.idchude == id).ToList();

            // Truyền danh sách bài hát vào view
            return View(songs);
        }



        public ActionResult Artists()
        {
            var artists = db.tblMusics.Select(m => m.casi).Distinct().ToList();
            return View(artists);
        }


        public ActionResult ArtistSongs(string artistName)
        {
            var songs = db.tblMusics.Where(m => m.casi == artistName).ToList();
            ViewBag.ArtistName = artistName; // Optionally pass artist name via ViewBag
            return View(songs);
        }


        public ActionResult Contribute()
        {
            // Lấy danh sách chủ đề để hiển thị trong dropdown
            ViewBag.Genres = new SelectList(db.chudes, "IDchude", "theloai1");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Contribute(tblMusic music, HttpPostedFileBase AudioFile, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra và lưu tệp âm thanh
                    if (AudioFile != null && AudioFile.ContentLength > 0)
                    {
                        var audioFileName = Path.GetFileName(AudioFile.FileName);
                        var audioPath = Path.Combine(Server.MapPath("~/music/"), audioFileName);
                        AudioFile.SaveAs(audioPath);
                        music.Data = audioFileName;
                    }

                    // Kiểm tra và lưu tệp hình ảnh
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        var imageFileName = Path.GetFileName(ImageFile.FileName);
                        var imagePath = Path.Combine(Server.MapPath("~/image/"), imageFileName);
                        ImageFile.SaveAs(imagePath);
                        music.Image = "/image/" + imageFileName;
                    }

                    // Thiết lập các thuộc tính bổ sung cho bài nhạc
                    music.iDacc = User.Identity.Name;
                    music.luotnghe = 0;

                    // Thêm bản ghi vào cơ sở dữ liệu
                    db.tblMusics.Add(music);
                    db.SaveChanges();

                    ViewBag.Message = "Upload thành công!";
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi chung
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi lưu dữ liệu: " + ex.Message);
                    ViewBag.Message = "Đã xảy ra lỗi khi upload.";
                }
            }
            else
            {
                ViewBag.Message = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại.";
            }

            // Lấy lại danh sách chủ đề nếu có lỗi
            ViewBag.Genres = new SelectList(db.chudes, "IDchude", "theloai1");
            return View(music);
        }

        public ActionResult ManageDisplayedSongs()
                {
                    return View();
                }
            

    
    public ActionResult Information_User()
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Lấy UserID từ Session
            string userId = Session["UserID"].ToString();

            // Truy vấn thông tin người dùng từ cơ sở dữ liệu
            var user = db.tblAccounts.FirstOrDefault(u => u.IDacount == userId);

            if (user == null)
            {
                return HttpNotFound("Không tìm thấy người dùng.");
            }

            // Truyền dữ liệu người dùng vào View
            return View(user);
        }
        [HttpGet]
        public ActionResult EditInformation()
        {
            // Kiểm tra nếu người dùng đã đăng nhập
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            // Lấy UserID từ Session
            string userId = Session["UserID"].ToString();

            // Truy vấn thông tin người dùng từ cơ sở dữ liệu
            var user = db.tblAccounts.FirstOrDefault(u => u.IDacount == userId);
            if (user == null)
            {
                return HttpNotFound("Không tìm thấy người dùng.");
            }

            return View(user);
        }

        // POST: Account/EditInformation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditInformation(tblAccount updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedUser);
            }

            try
            {
                // Truy vấn người dùng từ cơ sở dữ liệu
                var user = db.tblAccounts.FirstOrDefault(u => u.IDacount == updatedUser.IDacount);

                if (user != null)
                {
                    // Cập nhật thông tin người dùng
                    user.Password = updatedUser.Password;
                    user.Name = updatedUser.Name;
                    user.email = updatedUser.email;
                    user.ngaysinh = updatedUser.ngaysinh;
                    user.gioitinh = updatedUser.gioitinh;

                    db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    TempData["SuccessMessage"] = "Thông tin đã được cập nhật thành công!";
                    return RedirectToAction("Information_User");
                }
                else
                {
                    return HttpNotFound("Không tìm thấy người dùng.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Có lỗi xảy ra: " + ex.Message;
                return View(updatedUser);
            }
        }
    }

}

