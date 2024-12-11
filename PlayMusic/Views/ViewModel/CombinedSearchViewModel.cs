namespace PlayMusic.ViewModels
{
    using PlayMusic.Models; // Để dùng Model tblMusic hoặc ExternalMusic
    using System.Collections.Generic;

    public class CombinedSearchViewModel
    {
        public List<tblMusic> LocalResults { get; set; }  // Kết quả từ Database
        public List<ExternalMusic> ExternalResults { get; set; } // Kết quả từ API
        public string SearchTerm { get; set; } // Term mà người dùng nhập để tìm kiếm
    }
}
