namespace GeoCidadao.AMQP.Configuration
{
    // Padrão de declaração de queue -> Nome do serviço que consome a mensagem + objetivo da fila + "QUEUE_NAME"

    public static class QueueNames
    {
        /// <summary>
        /// Fila para recebimento de eventos vindos da criação de novos usuários do Keycloak
        /// </summary>
        public const string USER_MANAGEMENT_NEW_USER_QUEUE_NAME = "user_management_new_user_queue_name";

        /// <summary>
        /// Dead Letter Queue para recebimento de eventos vindos da criação de novos usuários do Keycloak
        /// </summary>
        public const string DLQ_USER_MANAGEMENT_NEW_USER_QUEUE_NAME = "user_management_new_user_dlq";

        /// <summary>
        /// Fila para recebimento de eventos de analytics de posts
        /// </summary>
        public const string ANALYTICS_SERVICE_POST_ANALYTICS_QUEUE_NAME = "analytics_service_post_analytics_queue";

        /// <summary>
        /// Dead Letter Queue para eventos de analytics de posts
        /// </summary>
        public const string DLQ_ANALYTICS_SERVICE_POST_ANALYTICS_QUEUE_NAME = "analytics_service_post_analytics_dlq";
    }

}