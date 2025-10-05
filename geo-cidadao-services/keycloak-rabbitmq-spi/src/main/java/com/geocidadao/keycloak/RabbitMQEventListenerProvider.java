package com.geocidadao.keycloak;

import org.keycloak.events.Event;
import org.keycloak.events.EventListenerProvider;
import org.keycloak.events.admin.AdminEvent;
import org.keycloak.models.KeycloakSession;

public class RabbitMQEventListenerProvider implements EventListenerProvider {

    private final RabbitMQPublisher publisher;
    private final KeycloakSession session;

    public RabbitMQEventListenerProvider(KeycloakSession session, RabbitMQPublisher publisher) {
        this.session = session;
        this.publisher = publisher;
    }

    @Override
    public void onEvent(Event event) {
        switch (event.getType()) {
            case REGISTER:
                publisher.publishUserCreatedEvent(event, session);
                break;
            case UPDATE_PROFILE:
                publisher.publishUserUpdatedEvent(event, session);
                break;
            case LOGIN:
                // opcional: enviar logs de login
                break;
            default:
                break;
        }
    }

    @Override
    public void onEvent(AdminEvent event, boolean includeRepresentation) {
        // Pode ser usado para eventos administrativos se desejar
    }

    @Override
    public void close() {
        // Liberar recursos se necessário
    }
}
