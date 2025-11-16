namespace GeoCidadao.AMQP.Messages
{
    public class PostInteractionMessage
    {
        public Guid PostId { get; set; }
        public string InteractionType { get; set; } = string.Empty; // "like", "unlike", "comment", "comment_edit", "comment_delete"
        public Guid UserId { get; set; }
    }
}
