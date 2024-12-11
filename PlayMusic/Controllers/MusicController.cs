using PlayMusic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Net.Http;
using Newtonsoft.Json;

namespace PlayMusic.Controllers
{
    public class MusicController : Controller
    {
        private MusicWebEntities1 db = new MusicWebEntities1();

        // Gọi API tìm kiếm
        private async Task<List<MusicViewModel>> SearchExternalMusic(string searchTerm)
        {
            List<MusicViewModel> results = new List<MusicViewModel>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                string apiUrl = $"https://example-music-api.com/search?query={searchTerm}"; // Thay bằng URL API thực tế

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync(apiUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            var jsonString = await response.Content.ReadAsStringAsync();
                            var apiResults = JsonConvert.DeserializeObject<List<MusicViewModel>>(jsonString);

                            if (apiResults != null)
                            {
                                results.AddRange(apiResults);
                            }
                        }
                    }
                    catch
                    {
                        // Xử lý lỗi API nếu cần
                    }
                }
            }

            return results;
        }

        // Hành động tìm kiếm
        public async Task<ActionResult> Search(string searchTerm)
        {
            // Tìm kiếm trong CSDL
            var dbResults = db.tblMusics
                              .Where(m => m.DisplayName.Contains(searchTerm) || m.casi.Contains(searchTerm))
                              .Select(m => new MusicViewModel
                              {
                                  DisplayName = m.DisplayName,
                                  Artist = m.casi,
                                  ImageUrl = m.Image,
                                  AudioUrl = "~/music/" + m.Data
                              })
                              .ToList();

            // Gọi API tìm kiếm
            var apiResults = await SearchExternalMusic(searchTerm);

            // Gộp kết quả
            var combinedResults = dbResults.Concat(apiResults).ToList();

            return View(combinedResults);
        }
    }
}
