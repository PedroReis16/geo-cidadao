FROM quay.io/keycloak/keycloak:26.4

# (opcional) defaults – podem ser sobrescritos no compose
ENV KEYCLOAK_ADMIN=admin \
    KEYCLOAK_ADMIN_PASSWORD=admin \
    KC_DB=postgres \
    KC_DB_URL=jdbc:postgresql://db:5432/geodb \
    KC_DB_USERNAME=geo \
    KC_DB_PASSWORD=geo \
    KC_HOSTNAME=localhost \
    KC_HTTP_PORT=8180 \
    RABBITMQ_HOST=rabbitmq \
    RABBITMQ_USER=admin \
    RABBITMQ_PASSWORD=admin 

# Build necessário após adicionar providers/temas
RUN /opt/keycloak/bin/kc.sh build

EXPOSE 8180

# Start padrão; você pode adicionar flags extras aqui se quiser
ENTRYPOINT ["/opt/keycloak/bin/kc.sh"]
CMD ["start-dev"]