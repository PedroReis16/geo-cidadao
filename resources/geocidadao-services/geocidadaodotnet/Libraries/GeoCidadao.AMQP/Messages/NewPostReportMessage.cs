using GeoCidadao.Models.Enums;

namespace GeoCidadao.AMQP.Messages
{
    public class NewPostReportMessage
    {
        public Guid PostId { get; set; }
        public Guid ReporterUserId { get; set; }
        public string? Reason { get; set; }
        public ReportTypes ReportType { get; set; }
    }
}