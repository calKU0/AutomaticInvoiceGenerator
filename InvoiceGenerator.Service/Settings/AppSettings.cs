namespace InvoiceGenerator.Service.Settings
{
    public class AppSettings
    {
        public int LogsExpirationDays { get; set; }
        public int RunIntervalMinutes { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
    }
}
