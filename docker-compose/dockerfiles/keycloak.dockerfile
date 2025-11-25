FROM quay.io/keycloak/keycloak:26.4

ENV KEYCLOAK_ADMIN=admin \
    KEYCLOAK_ADMIN_PASSWORD=admin \
    KC_DB=postgres \
    KC_DB_URL=jdbc:postgresql://db:5432/keycloakdb \
    KC_DB_USERNAME=keycloak \
    KC_DB_PASSWORD=keycloak \
    KC_DB_SCHEMA=keycloak

RUN /opt/keycloak/bin/kc.sh build

EXPOSE 8080

ENTRYPOINT ["/opt/keycloak/bin/kc.sh"]