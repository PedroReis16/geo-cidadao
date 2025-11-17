package com.geocidadao.keycloak;

import org.keycloak.Config;
import org.keycloak.events.EventListenerProvider;
import org.keycloak.events.EventListenerProviderFactory;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.KeycloakSessionFactory;

public class RabbitMQEventListenerProviderFactory implements EventListenerProviderFactory {

    private RabbitMQPublisher publisher;

    @Override
    public EventListenerProvider create(KeycloakSession session) {
        return new RabbitMQEventListenerProvider(session, publisher);
    }

    @Override
    public void init(Config.Scope config) {
        publisher = new RabbitMQPublisher();
    }

    @Override
    public void postInit(KeycloakSessionFactory factory) { }

    @Override
    public void close() { }

    @Override
    public String getId() {
        return "rabbitmq-events";
    }
}
