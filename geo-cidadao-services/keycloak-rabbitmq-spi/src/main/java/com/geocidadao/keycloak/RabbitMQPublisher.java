package com.geocidadao.keycloak;

import com.rabbitmq.client.Channel;
import com.rabbitmq.client.Connection;
import com.rabbitmq.client.ConnectionFactory;
import org.keycloak.events.Event;
import org.keycloak.models.KeycloakSession;
import org.keycloak.models.UserModel;

import java.nio.charset.StandardCharsets;
import org.json.JSONObject;

public class RabbitMQPublisher {

    private static final String EXCHANGE_NAME = "keycloak.events";
    private static final String ROUTING_KEY = "user.created";

    private final ConnectionFactory factory;

    public RabbitMQPublisher() {
        factory = new ConnectionFactory();
        factory.setHost("rabbitmq");
        factory.setUsername("admin");
        factory.setPassword("admin");
        factory.setPort(5672);
    }

    public void publishUserCreatedEvent(Event event, KeycloakSession session) {
        String userId = event.getUserId();
        UserModel user = session.users().getUserById(session.getContext().getRealm(), userId);

        JSONObject json = new JSONObject();
        json.put("id", user.getId());
        json.put("username", user.getUsername());
        json.put("email", user.getEmail());
        json.put("firstName", user.getFirstName());
        json.put("lastName", user.getLastName());

        publishMessage(EXCHANGE_NAME, ROUTING_KEY, json.toString());
    }

    public void publishUserUpdatedEvent(Event event, KeycloakSession session) {
        // semelhante ao created, mas outra routing key
        publishMessage(EXCHANGE_NAME, "user.updated", "...");
    }

    private void publishMessage(String exchange, String routingKey, String message) {
        try (Connection connection = factory.newConnection();
             Channel channel = connection.createChannel()) {
            channel.exchangeDeclare(exchange, "topic", true);
            channel.basicPublish(exchange, routingKey, null, message.getBytes(StandardCharsets.UTF_8));
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}
