using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PlayMusic.Models
{


    public class DropboxHelper
    {
        private static string AccessToken = "sl.CCwPof66c3NdjHEa5Uxpk-FwtjZlCJ0Q_ebzKUbLgM-SPysMbMVei9Ad3f5_PPFAjGv6oMvT9_-OFvLCP9swUHfGwGMGfWawDPlw1gGPNOfzbTtwpZdmGHfyK05EceMwGtPSgznCws4VFXBxahEy"; // Thay bằng access token của bạn

        public static async Task<string> UploadFileAsync(string localFilePath, string dropboxFolder, string fileName)
        {
            using (var dbx = new DropboxClient(AccessToken))
            {
                using (var fileStream = File.OpenRead(localFilePath))
                {
                    var dropboxPath = $"{dropboxFolder}/{fileName}";

                    // Tải file lên Dropbox
                    var uploadResult = await dbx.Files.UploadAsync(
                        dropboxPath,
                        WriteMode.Overwrite.Instance,
                        body: fileStream);

                    // Lấy liên kết tải file
                    var sharedLink = await dbx.Sharing.CreateSharedLinkWithSettingsAsync(dropboxPath);
                    return sharedLink.Url.Replace("?dl=0", "?raw=1"); // Chuyển link thành direct URL
                }
            }
        }
    }
}