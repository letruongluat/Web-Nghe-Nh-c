using PlayMusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PlayMusic.Controllers
{
    public class AlbumController : Controller
    {
        // GET: Album
        public ActionResult Index()
        {
            return View();
        }

        private MusicWebEntities1 db = new MusicWebEntities1();

        // Thêm bài hát vào yêu thích
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ToggleFavorite(int musicId)
        {
            var userId = User.Identity.Name;
            var favorite = db.FavoriteAlbums.FirstOrDefault(f => f.IDacount == userId && f.iDMusic == musicId);

            if (favorite == null)
            {
                // Thêm vào yêu thích
                db.FavoriteAlbums.Add(new FavoriteAlbum { IDacount = userId, iDMusic = musicId });
                db.SaveChanges();
                return Json(new { isFavorite = true });
            }
            else
            {
                // Xóa khỏi yêu thích
                db.FavoriteAlbums.Remove(favorite);
                db.SaveChanges();
                return Json(new { isFavorite = false });
            }
        }

        // Hiển thị danh sách bài hát yêu thích
        public ActionResult Favorites()
        {
            var userId = User.Identity.Name;
            var favorites = db.FavoriteAlbums
                .Where(f => f.IDacount == userId)
                .Select(f => f.tblMusic)
                .ToList();
            return View(favorites);
        }
    }
}
