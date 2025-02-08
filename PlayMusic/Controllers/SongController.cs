using System;
using System.Linq;
using System.Web.Mvc;
using PlayMusic.Models;
using System.Threading.Tasks; 
using System.IO;

namespace PlayMusic.Controllers
{
    public class SongController : Controller
    {
        private MusicWebEntities1 db = new MusicWebEntities1(); 

        // Lấy danh sách tất cả bài hát
        public ActionResult Index()
        {
            var songs = db.tblMusics.ToList(); // Truy xuất danh sách bài hát từ database

            // Ghi log số lượng bài hát để debug
            System.Diagnostics.Debug.WriteLine("Number of songs: " + songs.Count);

            return View(songs); // Truyền danh sách bài hát vào view để hiển thị
        }

        // Hiển thị chi tiết bài hát và liên kết bài hát trước/sau
        public ActionResult Details(int id)
        {
            var song = db.tblMusics.Find(id); // Tìm bài hát theo ID
            if (song == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy bài hát
            }

            // Tìm bài hát trước đó
            var previousSong = db.tblMusics
                .Where(s => s.iDmusic < id)
                .OrderByDescending(s => s.iDmusic)
                .FirstOrDefault();

            // Tìm bài hát tiếp theo
            var nextSong = db.tblMusics
                .Where(s => s.iDmusic > id)
                .OrderBy(s => s.iDmusic)
                .FirstOrDefault();

            // Truyền thông tin bài hát trước/sau vào ViewBag
            ViewBag.PreviousSong = previousSong;
            ViewBag.NextSong = nextSong;

            return View(song); // Truyền bài hát hiện tại vào view
        }

        // Thêm bài hát vào danh sách yêu thích
        public ActionResult AddToFavorite(int id)
        {
            if (Session["UserID"] != null) // Kiểm tra nếu người dùng đã đăng nhập
            {
                string userId = Session["UserID"].ToString();

                // Ghi log thông tin người dùng và bài hát để debug
                System.Diagnostics.Debug.WriteLine("Adding to favorite: UserID = " + userId + ", Music ID = " + id);

                // Kiểm tra xem bài hát đã có trong danh sách yêu thích chưa
                var existingFavorite = db.FavoriteAlbums.FirstOrDefault(f => f.IDacount == userId && f.iDMusic == id);
                if (existingFavorite == null)
                {
                    // Nếu chưa có, thêm bài hát vào danh sách yêu thích
                    var favorite = new FavoriteAlbum
                    {
                        IDacount = userId,
                        iDMusic = id
                    };

                    db.FavoriteAlbums.Add(favorite);

                    try
                    {
                        db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                        System.Diagnostics.Debug.WriteLine("Added to favorites successfully.");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error saving to database: " + ex.Message);
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Song already exists in favorites.");
                    ViewBag.Message = "Bài hát đã có trong danh sách yêu thích.";
                }
            }
            else
            {
                // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                System.Diagnostics.Debug.WriteLine("Session UserID not found, redirecting to login.");
                return RedirectToAction("Index", "Login");
            }

            return RedirectToAction("Details", new { id = id }); // Quay lại trang chi tiết bài hát
        }

        // Quản lý danh sách bài hát yêu thích của người dùng
        public ActionResult ManageFavoriteAlbums()
        {
            string userId = User.Identity.Name; // Lấy ID của người dùng đăng nhập

            // Lấy danh sách bài hát yêu thích của người dùng
            var favoriteSongs = db.FavoriteAlbums
                .Where(f => f.IDacount == userId) // Lọc theo ID người dùng
                .Select(f => f.tblMusic) // Lấy thông tin bài hát từ FavoriteAlbums
                .ToList();

            return View(favoriteSongs); // Truyền danh sách bài hát yêu thích vào view
        }

        // Hiển thị giao diện xác nhận xóa bài hát yêu thích
        [HttpGet]
        public ActionResult RemoveFromFavorite(int id)
        {
            // Ghi log ID của bài hát cần xóa để debug
            System.Diagnostics.Debug.WriteLine("RemoveFromFavorite action triggered with id: " + id);

            var favoriteMusic = db.FavoriteAlbums.Find(id); // Tìm bài hát yêu thích theo ID
            if (favoriteMusic == null)
            {
                System.Diagnostics.Debug.WriteLine("Favorite album with ID " + id + " not found.");
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy
            }
            return View(favoriteMusic); // Truyền bài hát yêu thích vào view
        }

        // Xóa bài hát khỏi danh sách yêu thích
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromFavoriteConfirmed(int id)
        {
            var favoriteMusic = db.FavoriteAlbums.Find(id); // Tìm bài hát yêu thích theo ID
            if (favoriteMusic != null)
            {
                db.FavoriteAlbums.Remove(favoriteMusic); // Xóa bài hát khỏi cơ sở dữ liệu
                db.SaveChanges(); // Lưu thay đổi
                TempData["SuccessMessage"] = "Đã xóa bài hát yêu thích thành công!";
                return RedirectToAction("ManageFavoriteAlbums"); // Chuyển hướng đến trang quản lý danh sách yêu thích
            }

            TempData["ErrorMessage"] = "Không tìm thấy bài hát yêu thích để xóa.";
            return RedirectToAction("ManageFavoriteAlbums");
        }

        // Upload bài hát lên Dropbox
        public async Task<ActionResult> UploadToDropbox(int id)
        {
            var song = db.tblMusics.Find(id); // Tìm bài hát theo ID
            if (song == null)
            {
                return HttpNotFound();
            }

            string localPath = Server.MapPath("~/music/" + song.Data); // Đường dẫn file trên máy chủ
            string dropboxFolder = "/Apps/MusicAPPListen/music"; // Thư mục trên Dropbox
            string fileName = song.Data; // Tên file

            // Upload file lên Dropbox và lấy URL
            string dropboxUrl = await DropboxHelper.UploadFileAsync(localPath, dropboxFolder, fileName);

            // Cập nhật cột Data của bài hát với URL từ Dropbox
            song.Data = dropboxUrl;
            db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            TempData["SuccessMessage"] = "File uploaded to Dropbox successfully!";
            return RedirectToAction("Details", new { id = id });
        }

        // Cập nhật URL Dropbox cho tất cả bài hát
        public async Task<ActionResult> UpdateSongsWithDropboxUrls()
        {
            string dropboxFolderPath = "/Apps/MusicAPPListen/music"; // Thư mục chứa bài hát trên Dropbox
            var dropboxService = new DropboxService("your-dropbox-token"); // Khởi tạo dịch vụ Dropbox

            // Lấy danh sách URL file từ Dropbox
            var songUrls = await dropboxService.GetFileUrls(dropboxFolderPath);

            foreach (var url in songUrls)
            {
                string fileName = Path.GetFileNameWithoutExtension(url); // Lấy tên file từ URL

                // Tìm bài hát trong cơ sở dữ liệu theo tên
                var song = db.tblMusics.FirstOrDefault(s => s.DisplayName == fileName);
                if (song != null)
                {
                    song.Data = url; // Cập nhật cột Data với URL từ Dropbox
                }
            }

            db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
            return Content("Đã cập nhật thành công các URL Dropbox vào database.");
        }
    }
}