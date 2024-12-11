using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlayMusic.Models
{
    public class SpotifyTrackItem
    {
        public string name { get; set; }
        public List<SpotifyArtist> artists { get; set; }
        public SpotifyAlbum album { get; set; }
        public string preview_url { get; set; }
    }
}