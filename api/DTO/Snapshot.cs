using System.Net.Mime;

namespace api.DTO
{
    public class Snapshot
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}
