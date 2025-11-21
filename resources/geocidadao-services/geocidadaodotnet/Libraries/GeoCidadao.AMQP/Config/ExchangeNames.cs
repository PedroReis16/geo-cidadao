namespace GeoCidadao.AMQP.Configuration
{
    // Padrão de declaração do Exchage -> Nome do serviço que gera a mensagem + Tipo de exchange


    public static class ExchangeNames
    {
        /// <summary>
        /// Exchange para notificação de eventos do Keycloak
        /// </summary>
        public const string KEYCLOAK_EVENTS_TOPIC_EXCHANGE = "keycloak_events_topic_exchange";

        /// <summary>
        /// Dead Letter Exchange para eventos do Keycloak
        /// </summary>
        public const string DLQ_KEYCLOAK_EVENTS_TOPIC_EXCHANGE = "keycloak_events_exchange_dlq";

        /// <summary>
        /// Exchange para gerenciamento de usuários
        /// </summary>
        public const string USER_MANAGEMENT_TOPIC_EXCHANGE_NAME = "user_management_topic_exchange";

        /// <summary>
        /// Dead Letter Exchange para gerenciamento de usuários
        /// </summary>
        public const string DLQ_USER_MANAGEMENT_TOPIC_EXCHANGE_NAME = "user_management_topic_exchange_dlq";

        /// <summary>
        /// Exchange para gerenciamento de posts
        /// </summary>
        public const string POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME = "posts_management_topic_exchange";

        /// <summary>
        /// Dead Letter Exchange para gerenciamento de posts
        /// </summary>
        public const string DLQ_POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME = "posts_management_topic_exchange_dlq";


        /// <summary>
        /// Exchange para controle de relevância de posts
        /// </summary>
        public const string RELEVANCE_WORKER_DIRECT_EXCHANGE_NAME = "relevance_worker_direct_exchange";

        /// <summary>
        /// Dead Letter Exchange para controle de relevância de posts
        /// </summary>
        public const string DLQ_RELEVANCE_WORKER_DIRECT_EXCHANGE_NAME = "relevance_worker_direct_exchange_dlq";
    }

}