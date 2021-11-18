using System.Text.Json.Serialization;

namespace InfiniteGallery.Models.Data
{
    public class PhotoDTO
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public long Width { get; set; }
        public long Height { get; set; }
        public string Url { get; set; }

        [JsonPropertyName("download_url")]
        public string DownloadUrl { get; set; }
    }
}