namespace GeoCidadao.AMQP.Configuration;

public static class RoutingKeyNames
{
    /// <summary>
    /// Routing key para notificação de novos usuários vindos do keycloak
    /// </summary>
    public const string NEW_USER_ROUTING_KEY = "new.user";
    /// <summary>
    /// Dead Letter Routing key para notificação de novos usuários vindos do keycloak
    /// </summary>
    public const string DLQ_NEW_USER_ROUTING_KEY = "new.user.dlq";

    /// <summary>
    /// Routing key para notificação de alterações nos dados do usuário
    /// </summary>
    public const string USER_CHANGED_ACTIONS_ROUTING_KEY = "user.changed";

    /// <summary>
    /// Dead Letter Routing key para notificação de alterações nos dados do usuário
    /// </summary>
    public const string DLQ_USER_CHANGED_ACTIONS_ROUTING_KEY = "user.changed.dlq";

    /// <summary>
    /// Routing key para notificação de alteração em posts
    /// </summary>
    public const string POST_CHANGED_ROUTING_KEY = "post.changed";

    /// <summary>
    /// Dead Letter Routing key para notificação de alteração em posts
    /// </summary>
    public const string DLQ_POST_CHANGED_ROUTING_KEY = "post.changed.dlq";

    /// <summary>
    /// Routing key para notificação de novos posts
    /// </summary>
    public const string NEW_POST_ROUTING_KEY = "post.new";

    /// <summary>
    /// Dead Letter Routing key para notificação de novos posts
    /// </summary>
    public const string DLQ_NEW_POST_ROUTING_KEY = "post.new.dlq";

    /// <summary>
    /// Routing key para notificação de interações em posts
    /// </summary>
    public const string POST_INTERACTED_ROUTING_KEY = "post.interacted";

    /// <summary>
    /// Dead Letter Routing key para notificação de interações em posts
    /// </summary>
    public const string DLQ_POST_INTERACTED_ROUTING_KEY = "post.interacted.dlq";

    /// <summary>
    /// Routing key para notificação de posts deletados
    /// </summary>
    public const string POST_DELETED_ROUTING_KEY = "post.deleted";

    /// <summary>
    /// Dead Letter Routing key para notificação de posts deletados
    /// </summary>
    public const string DLQ_POST_DELETED_ROUTING_KEY = "post.deleted.dlq";

    /// <summary>
    /// Routing key para notificação de usuários deletados
    /// </summary>
    public const string USER_DELETED_ROUTING_KEY = "user.deleted";

    /// <summary>
    /// Dead Letter Routing key para notificação de usuários deletados
    /// </summary>
    public const string DLQ_USER_DELETED_ROUTING_KEY = "user.deleted.dlq";

    /// <summary>
    /// Routing key para notificação de posts denunciados
    /// </summary>
    public const string POST_REPORTED_ROUTING_KEY = "post.reported";

    /// <summary>
    /// Dead Letter Routing key para notificação de posts denunciados
    /// </summary>
    public const string DLQ_POST_REPORTED_ROUTING_KEY = "post.reported.dlq";
}
