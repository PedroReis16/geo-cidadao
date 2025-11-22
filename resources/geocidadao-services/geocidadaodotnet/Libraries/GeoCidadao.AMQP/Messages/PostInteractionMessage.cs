using GeoCidadao.Models.Enums;

namespace GeoCidadao.AMQP.Messages
{
    public class PostInteractionMessage
    {
        public Guid PostId { get; set; }
        public InteractionType InteractionType { get; set; } 
    }
}
