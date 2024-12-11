using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PlayMusic.Models
{
    public class SpotifyTopTracksResponse
    {
        public List<SpotifyTrackItem> items { get; set; }
    }
}