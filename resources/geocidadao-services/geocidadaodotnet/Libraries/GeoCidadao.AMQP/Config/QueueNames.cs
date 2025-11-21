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
        /// Fila para recebimento de eventos vindos da criação de novos posts para o indexador de posts
        /// </summary>
        public const string POST_INDEXER_WORKER_NEW_POST_QUEUE_NAME = "post_indexer_worker_new_post_queue_name";

        /// <summary>
        /// Dead Letter Queue para recebimento de eventos vindos da criação de novos posts para o indexador de posts
        /// </summary>
        public const string DLQ_POST_INDEXER_WORKER_NEW_POST_QUEUE_NAME = "post_indexer_worker_new_post_dlq";

        /// <summary>
        /// Fila para o recebimento de eventos relacionados a alterações na relevância dos posts para o indexador de posts
        /// </summary>
        public const string POST_INDEXER_WORKER_RELEVANCE_POST_QUEUE_NAME = "post_indexer_worker_relevance_post_queue_name";

        /// <summary>
        /// Dead Letter Queue para o recebimento de eventos relacionados a alterações na relevância dos posts para o indexador de posts
        /// </summary>
        public const string DLQ_POST_INDEXER_WORKER_RELEVANCE_POST_QUEUE_NAME = "post_indexer_worker_relevance_post_dlq";

        /// <summary>
        /// Fila para o recebimento de eventos relacionados a exclusão de postagens de uma usuário para o indexador de posts
        /// </summary>
        public const string POST_INDEXER_WORKER_DELETED_POST_QUEUE_NAME = "post_indexer_worker_deleted_post_queue_name";

        /// <summary>
        /// Dead Letter Queue para o recebimento de eventos relacionados a exclusão de postagens de uma usuário para o indexador de posts
        /// </summary>
        public const string DLQ_POST_INDEXER_WORKER_DELETED_POST_QUEUE_NAME = "post_indexer_worker_deleted_post_dlq";

        /// <summary>
        /// Fila para o recebimento de eventos relacionadas a exclusão de usuários para o serviço de gerenciamento de posts
        /// </summary>
        public const string POST_MANAGEMENT_USER_DELETED_QUEUE_NAME = "post_management_user_deleted_queue_name";

        /// <summary>
        /// Dead Letter Queue para o recebimento de eventos relacionadas a exclusão de usuários para o serviço de gerenciamento de posts
        /// </summary>
        public const string DLQ_POST_MANAGEMENT_USER_DELETED_QUEUE_NAME = "post_management_user_deleted_dlq";

        /// <summary>
        /// Fila para o recebimento de eventos vindos da criação de novos posts para o worker de relevância
        /// </summary>
        public const string RELEVANCE_WORKER_NEW_POST_QUEUE_NAME = "relevance_worker_new_post_queue_name";

        /// <summary>
        /// Dead Letter Queue para o recebimento de eventos vindos da criação de novos posts para o worker de relevância
        /// </summary>
        public const string DLQ_RELEVANCE_WORKER_NEW_POST_QUEUE_NAME = "relevance_worker_new_post_dlq";
    
    
        /// <summary>
        /// Fila para o recebimento de eventos relacionados a exclusão de postagens de uma usuário para o worker de relevância
        /// </summary>
        public const string RELEVANCE_WORKER_DELETED_POST_QUEUE_NAME = "relevance_worker_deleted_post_queue_name";

        /// <summary>
        /// Dead Letter Queue para o recebimento de eventos relacionados a exclusão de postagens de
        /// uma usuário para o worker de relevância
        /// </summary>
        public const string DLQ_RELEVANCE_WORKER_DELETED_POST_QUEUE_NAME = "relevance_worker_deleted_post_dlq";
    }

}