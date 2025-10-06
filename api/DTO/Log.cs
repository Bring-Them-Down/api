namespace api.DTO
{
    public class Log
    {
        public int LogId { get; set; }
        public DateTime Timestamp { get; set; }
        public int DeviceId { get; set; }
        public Device? Device { get; set; }
        public int KnownId { get; set; }
        public Known? Known { get; set; }
        public int? CaptureId { get; set; }
        public Capture? Capture { get; set; }
    }
}
