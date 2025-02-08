using System.Collections.Generic;

namespace PlayMusic.Models
{
    public class FavoriteCategori
    {
        public int IDCategory { get; set; } // ID của danh mục yêu thích
        public string CategoryName { get; set; } // Tên danh mục
        public string IDacount { get; set; } // Người dùng tạo danh mục

        // Danh sách bài hát thuộc danh mục
        public virtual ICollection<FavoriteAlbum> FavoriteAlbums { get; set; }
    }
}
