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
    /// Routing key para notificação de interação em posts (curtidas e comentários)
    /// </summary>
    public const string POST_INTERACTION_ROUTING_KEY = "post.interaction";

    /// <summary>
    /// Dead Letter Routing key para notificação de interação em posts
    /// </summary>
    public const string DLQ_POST_INTERACTION_ROUTING_KEY = "post.interaction.dlq";

    /// <summary>
    /// Routing key para notificação de eventos de analytics de posts com localização
    /// </summary>
    public const string POST_ANALYTICS_ROUTING_KEY = "post.analytics";

    /// <summary>
    /// Dead Letter Routing key para notificação de eventos de analytics de posts
    /// </summary>
    public const string DLQ_POST_ANALYTICS_ROUTING_KEY = "post.analytics.dlq";
}
