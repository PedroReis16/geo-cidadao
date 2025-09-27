# Build da aplicação (usando Maeven Wrapper)
FROM maven:3.9.6-eclipse-temurin-21 AS build
WORKDIR /app

# Copia pom.xml e baixa dependências
COPY gerenciamento-usuarios-api/pom.xml .
RUN mvn dependency:go-offline -B

# Copia o código fonte e compila
COPY src ./src
RUN mvn clean package -DskipTests

# Imagem final
FROM eclipse-temurin:21-jre-alpine
WORKDIR /app

# Copia o jar gerado na fase de build
COPY --from=build /app/target/gerenciamento-usuarios-api-*.jar app.jar

# Expõe a porta que a aplicação irá rodar
EXPOSE 7080

# Comando para rodar a aplicação
ENTRYPOINT ["java", "-jar", "app.jar"]