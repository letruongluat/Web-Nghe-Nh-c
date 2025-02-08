using PlayMusic.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PlayMusic.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private MusicWebEntities1 db = new MusicWebEntities1();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Songs()
        {
            var musicList = db.tblMusics.ToList();
            return View(musicList);
        }

        [HttpGet]
        public ActionResult EditMusic(int id)
        {
            var music = db.tblMusics.Find(id); // Tìm bài hát bằng ID
            if (music == null)
            {
                return HttpNotFound(); // Nếu không tìm thấy, trả về lỗi 
            }
            return View(music); // Trả dữ liệu bài hát cho View
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMusic(tblMusic updatedMusic)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingMusic = db.tblMusics.Find(updatedMusic.iDmusic); // Tìm bài hát gốc trong DB

                    if (existingMusic != null)
                    {
                        // Cập nhật thông tin bài hát
                        existingMusic.DisplayName = updatedMusic.DisplayName;
                        existingMusic.casi = updatedMusic.casi;
                        existingMusic.Cotenttype = updatedMusic.Cotenttype;

                        db.Entry(existingMusic).State = EntityState.Modified; // Đánh dấu trạng thái đã sửa đổi
                        db.SaveChanges(); // Lưu thay đổi vào DB
                        TempData["SuccessMessage"] = "Cập nhật bài nhạc thành công!";
                        return RedirectToAction("Songs"); // Trở về danh sách bài nhạc
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy bài nhạc để chỉnh sửa.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi chỉnh sửa: " + ex.Message);
            }

            // Nếu có lỗi, trả lại view với thông báo lỗi
            return View(updatedMusic);
        }

        [HttpPost]
        public ActionResult DeleteMusic(int id) // Đảm bảo `id` là kiểu `int`
        {
            var music = db.tblMusics.Find(id);
            if (music == null)
            {
                return Json(new { success = false, message = "Không tìm thấy bài nhạc." });
            }

            db.tblMusics.Remove(music);
            db.SaveChanges();

            return Json(new { success = true, message = "Xóa bài nhạc thành công." });
        }
    
public ActionResult Users()
        {
            var users = db.tblAccounts.ToList();
            return View(users);
        }

        // Xem chi tiết người dùng
        public ActionResult ViewUserDetails(string id)
        {
            var user = db.tblAccounts.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }


        // Tạo người dùng mới
        [HttpGet]
        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(tblAccount account)
        {
            if (ModelState.IsValid)
            {
                db.tblAccounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Users");
            }
            return View(account);
        }

        // Sửa người dùng
        [HttpGet]
        public ActionResult EditUser(string id)
        {
            var user = db.tblAccounts.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(tblAccount account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Users");
            }
            return View(account);
        }

        // Xóa người dùng bằng Ajax
        [HttpPost]
        public JsonResult DeleteUserAjax(string id)
        {
            var user = db.tblAccounts.Find(id);
            if (user == null)
            {
                return Json(new { success = false, message = "Người dùng không tồn tại" });
            }

            db.tblAccounts.Remove(user);
            db.SaveChanges();

            return Json(new { success = true, message = "Đã xóa thành công" });
        }
    


    public ActionResult ManageViews()
        {
            var dailyViews = db.ViewLogs
     .GroupBy(v => DbFunctions.TruncateTime(v.ViewDate))
     .Select(g => new
     {
         Date = g.Key,
         Count = g.Sum(v => v.Count)
     })
     .ToList();

            // Thống kê theo tháng
            var monthlyViews = db.ViewLogs
                .GroupBy(v => new { v.ViewDate.Year, v.ViewDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Count = g.Sum(v => v.Count)
                })
                .ToList();

            // Thống kê theo quý
            var quarterlyViews = db.ViewLogs
                .GroupBy(v => new { v.ViewDate.Year, Quarter = (v.ViewDate.Month - 1) / 3 + 1 })
                .Select(g => new
                {
                    Quarter = g.Key.Quarter,
                    Year = g.Key.Year,
                    Count = g.Sum(v => v.Count)
                })
                .ToList();

            // Truyền dữ liệu vào View
            ViewBag.DailyViews = dailyViews;
            ViewBag.MonthlyViews = monthlyViews;
            ViewBag.QuarterlyViews = quarterlyViews;

            return View();
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
        public ActionResult ContributeList()
        {
            // Lấy danh sách bài hát không phải của admin
            var contributeList = db.tblMusics
                .Where(m => m.iDacc != "admin") // Lọc các bài hát không phải của admin
                .ToList();

            // Chuẩn bị dữ liệu để vẽ biểu đồ
            var chartData = contributeList
                .GroupBy(m => m.iDacc) // Nhóm theo tài khoản
                .Select(g => new
                {
                    User = g.Key,
                    SongCount = g.Count() // Đếm số bài hát cho mỗi tài khoản
        })
                .ToList();

            // Truyền dữ liệu qua ViewBag
            ViewBag.ContributeList = contributeList;
            ViewBag.ChartData = chartData;

            return View();
        }

        public ActionResult ManageDisplayedSongs()
        {
            return View();
        }
       
        public ActionResult ManageGenre()
        {
            var genres = db.chudes.ToList();
            return View(genres);

        }
        public ActionResult CreateGenre()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGenre(chude genre)
        {
            if (ModelState.IsValid)
            {
                db.chudes.Add(genre);
                db.SaveChanges();
                return RedirectToAction("ManageGenre");
            }
            return View(genre);
        }
        public ActionResult EditGenre(int id)
        {
            var genre = db.chudes.Find(id);
            if (genre == null)
            {
                return HttpNotFound();
            }
            return View(genre);
        }

        // POST: Chỉnh sửa chủ đề
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditGenre(chude genre)
        {
            if (ModelState.IsValid)
            {
                db.Entry(genre).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ManageGenre");
            }
            return View(genre);
        }

        [HttpPost]
        public JsonResult DeleteGenre(int id)
        {
            var genre = db.chudes.Find(id);
            if (genre == null)
            {
                return Json(new { success = false });
            }

            db.chudes.Remove(genre);
            db.SaveChanges();

            return Json(new { success = true });
        }

        public ActionResult ManageSongs()
        {
            int totalSongs = db.tblMusics.Count();

            // Số lượng bài hát theo từng chủ đề
            var songsByTopic = db.tblMusics
                .GroupBy(m => m.idchude)
                .Select(g => new
                {
                    TopicId = g.Key,
                    TopicName = g.FirstOrDefault().chude.theloai1, // Hoặc tên chủ đề khác nếu cần
                SongCount = g.Count()
                })
                .ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.TotalSongs = totalSongs;
            ViewBag.SongsByTopic = songsByTopic;

            return View();
        }
    }

      
    }
