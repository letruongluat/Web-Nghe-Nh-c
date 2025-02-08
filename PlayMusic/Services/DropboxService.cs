using Dropbox.Api;
using Dropbox.Api.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace PlayMusic.Models
{

  

    public class DropboxService
    {
        private readonly string _accessToken;

        public DropboxService(string accessToken)
        {
            _accessToken = accessToken;
        }

        public async Task<List<string>> GetFileUrls(string folderPath)
        {
            using (var dropboxClient = new DropboxClient(_accessToken))
            {
                var fileUrls = new List<string>();

                var list = await dropboxClient.Files.ListFolderAsync(folderPath);
                foreach (var item in list.Entries.Where(i => i.IsFile))
                {
                    var sharedLink = await dropboxClient.Sharing.CreateSharedLinkWithSettingsAsync(item.PathLower);
                    var directLink = sharedLink.Url.Replace("?dl=0", "?dl=1");
                    fileUrls.Add(directLink);
                }

                return fileUrls;
            }
        }
    }

}