using System;
using System.Linq;
using System.Web.Mvc;
using PlayMusic.Models;

namespace PlayMusic.Controllers
{
    public class SongController : Controller
    {
        private MusicWebEntities1 db = new MusicWebEntities1();

        // GET: Songs
        public ActionResult Index()
        {
            var songs = db.tblMusics.ToList(); 

         
            System.Diagnostics.Debug.WriteLine("Number of songs: " + songs.Count);

            return View(songs); 
        }




        public ActionResult Details(int id)
        {
            var song = db.tblMusics.Find(id);
            if (song == null)
            {
                return HttpNotFound();
            }
            return View(song);
        }

        public ActionResult AddToFavorite(int id)
        {
            if (Session["UserID"] != null)
            {
                string userId = Session["UserID"].ToString();
                System.Diagnostics.Debug.WriteLine("Adding to favorite: UserID = " + userId + ", Music ID = " + id);

                // Check for existing favorite
                var existingFavorite = db.FavoriteAlbums.FirstOrDefault(f => f.IDacount == userId && f.iDMusic == id);
                if (existingFavorite == null)
                {
                    // Add new favorite
                    var favorite = new FavoriteAlbum
                    {
                        IDacount = userId,
                        iDMusic = id
                    };

                    db.FavoriteAlbums.Add(favorite);

                    try
                    {
                        db.SaveChanges();
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
                System.Diagnostics.Debug.WriteLine("Session UserID not found, redirecting to login.");
                return RedirectToAction("Index", "Login");
            }

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult ManageFavoriteAlbums()
        {
            string userId = User.Identity.Name; // Lấy thông tin người dùng đăng nhập

            // Lấy danh sách các bài hát yêu thích của người dùng
            var favoriteSongs = db.FavoriteAlbums
                .Where(f => f.IDacount == userId) // Lọc theo ID người dùng
                .Select(f => f.tblMusic) // Lấy thông tin bài hát từ FavoriteAlbums
                .ToList();

            return View(favoriteSongs); // Truyền danh sách bài hát (tblMusic) vào view
        }


        [HttpGet]
        public ActionResult RemoveFromFavorite(int id)
        {
            System.Diagnostics.Debug.WriteLine("RemoveFromFavorite action triggered with id: " + id);

            var favoriteMusic = db.FavoriteAlbums.Find(id); // Tìm bài hát yêu thích theo ID
            if (favoriteMusic == null)
            {
                System.Diagnostics.Debug.WriteLine("Favorite album with ID " + id + " not found.");
                return HttpNotFound();
            }
            return View(favoriteMusic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RemoveFromFavoriteConfirmed(int id)
        {
            var favoriteMusic = db.FavoriteAlbums.Find(id);
            if (favoriteMusic != null)
            {
                db.FavoriteAlbums.Remove(favoriteMusic);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Đã xóa bài hát yêu thích thành công!";
                return RedirectToAction("ManageFavoriteAlbums");
            }

            TempData["ErrorMessage"] = "Không tìm thấy bài hát yêu thích để xóa.";
            return RedirectToAction("ManageFavoriteAlbums");
        }
    }
}