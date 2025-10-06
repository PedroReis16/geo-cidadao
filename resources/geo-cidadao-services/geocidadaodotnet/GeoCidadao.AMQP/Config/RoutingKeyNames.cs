namespace GeoCidadao.AMQP.Configuration;

public static class RoutingKeyNames
{
    public const string NEW_USER_ROUTING_KEY = "new.user";
    public const string DLQ_NEW_USER_ROUTING_KEY = "new.user.dlq";
}
