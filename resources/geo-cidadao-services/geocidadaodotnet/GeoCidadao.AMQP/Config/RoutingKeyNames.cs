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
    /// Routing key para notificação de alteração na foto de usuário
    /// </summary>
    public const string USER_PHOTO_CHANGED_ROUTING_KEY = "user.photo.changed";

    /// <summary>
    ///    Dead Letter Routing key para notificação de alteração na foto de usuário
    /// </summary>
    public const string DLQ_USER_PHOTO_CHANGED_ROUTING_KEY = "user.photo.changed.dlq";

    /// <summary>
    /// Routing key para notificação de alterações nos dados do usuário
    /// </summary>
    public const string USER_CHANGED_ACTIONS_ROUTING_KEY = "user.changed";

    /// <summary>
    /// Dead Letter Routing key para notificação de alterações nos dados do usuário
    /// </summary>
    public const string DLQ_USER_CHANGED_ACTIONS_ROUTING_KEY = "user.changed.dlq";
}
