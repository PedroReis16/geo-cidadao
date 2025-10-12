# Geo Cidadão UI - Configuração Docker

Este documento descreve a configuração Docker para a aplicação de interface de usuário (UI) do Geo Cidadão, construída com React.js e TypeScript.

## Estrutura de Arquivos

```
docker-compose/
├── dockerfiles/
│   └── geo-cidadao-ui.dockerfile      # Dockerfile para a aplicação UI
├── nginx/
│   ├── nginx.conf                     # Configuração principal do Nginx
│   └── default.conf                   # Configuração do servidor virtual
├── geo-cidadao-ui.env                 # Variáveis de ambiente para a UI
└── ...

scripts/
├── ui-docker.sh                       # Script de gerenciamento (Linux/macOS)
└── ui-docker.ps1                      # Script de gerenciamento (Windows)

resources/geo-cidadao-ui/
├── .dockerignore                      # Arquivos ignorados no build Docker
├── src/                               # Código fonte da aplicação
├── package.json                       # Dependências e scripts npm
└── ...
```

## Configuração

### Variáveis de Ambiente

As variáveis de ambiente da UI são definidas no arquivo `docker-compose/geo-cidadao-ui.env`:

```env
NODE_ENV=production
VITE_API_BASE_URL=http://localhost:8081
VITE_KEYCLOAK_URL=http://localhost:8081/keycloak
VITE_KEYCLOAK_REALM=geo-cidadao
VITE_KEYCLOAK_CLIENT_ID=geo-cidadao-ui
```

### Nginx

A aplicação é servida através do Nginx, configurado para:

- **Single Page Application (SPA)**: Suporte ao React Router
- **Compressão Gzip**: Otimização de performance
- **Cache de Assets**: Configuração de cache para arquivos estáticos
- **Proxy da API**: Redirecionamento de requisições `/api/` para o Traefik
- **Headers de Segurança**: Configurações básicas de segurança
- **Health Check**: Endpoint `/health` para monitoramento

### Integração com Traefik

A UI está configurada no Traefik com:

- **Rota Principal**: `PathPrefix(/)`  com prioridade 1
- **CORS**: Headers configurados para requisições cross-origin
- **Load Balancer**: Direcionamento para a porta 80 do container

## Como Usar

### Opção 1: Docker Compose (Recomendado)

```bash
# Build e start da aplicação UI
docker-compose up -d geo-cidadao-ui

# Apenas build
docker-compose build geo-cidadao-ui

# Ver logs
docker-compose logs -f geo-cidadao-ui

# Parar o serviço
docker-compose stop geo-cidadao-ui
```

### Opção 2: Scripts de Gerenciamento

**Linux/macOS:**
```bash
# Dar permissão de execução
chmod +x scripts/ui-docker.sh

# Build da aplicação
./scripts/ui-docker.sh build

# Iniciar serviço
./scripts/ui-docker.sh start

# Ver logs
./scripts/ui-docker.sh logs

# Parar serviço
./scripts/ui-docker.sh stop

# Limpar containers e imagens
./scripts/ui-docker.sh clean
```

**Windows PowerShell:**
```powershell
# Build da aplicação
.\scripts\ui-docker.ps1 build

# Iniciar serviço
.\scripts\ui-docker.ps1 start

# Ver logs
.\scripts\ui-docker.ps1 logs

# Parar serviço
.\scripts\ui-docker.ps1 stop

# Limpar containers e imagens
.\scripts\ui-docker.ps1 clean
```

## Acesso à Aplicação

Após iniciar o serviço, a aplicação estará disponível em:

- **Direto**: http://localhost:3000
- **Via Traefik**: http://localhost:81 (usando a porta configurada em `TRAEFIK_SERVICES_PORT`)
- **Dashboard Traefik**: http://localhost:8081

## Desenvolvimento

### Build Local (sem Docker)

```bash
cd resources/geo-cidadao-ui
npm install
npm run build
npm run preview
```

### Modificar Configurações

1. **Variáveis de Ambiente**: Edite `docker-compose/geo-cidadao-ui.env`
2. **Configuração Nginx**: Edite `docker-compose/nginx/default.conf`
3. **Build Docker**: Edite `docker-compose/dockerfiles/geo-cidadao-ui.dockerfile`

### Rebuild Após Mudanças

```bash
# Rebuild completo
docker-compose build --no-cache geo-cidadao-ui

# Restart com rebuild
docker-compose up -d --build geo-cidadao-ui
```

## Troubleshooting

### Problemas Comuns

1. **Erro de Build**: Verifique se todas as dependências estão no `package.json`
2. **Erro de Proxy API**: Verifique se o Traefik está rodando
3. **Erro 404**: Verifique a configuração do Nginx para SPA routing
4. **Erro de CORS**: Verifique as configurações de CORS no Traefik

### Logs Úteis

```bash
# Logs da aplicação UI
docker-compose logs geo-cidadao-ui

# Logs do Nginx
docker exec -it geo-cidadao-ui tail -f /var/log/nginx/access.log
docker exec -it geo-cidadao-ui tail -f /var/log/nginx/error.log

# Logs do Traefik
docker-compose logs traefik
```

### Health Check

Verifique o status da aplicação:

```bash
curl http://localhost:3000/health
# Deve retornar: healthy
```

## Integração com CI/CD

A estrutura Docker criada é compatível com GitHub Actions e outras plataformas de CI/CD:

```yaml
# Exemplo para GitHub Actions
- name: Build UI Docker Image
  run: docker build -f docker-compose/dockerfiles/geo-cidadao-ui.dockerfile -t geo-cidadao-ui .

- name: Run UI Container
  run: docker run -d -p 3000:80 geo-cidadao-ui
```

## Otimizações de Performance

- **Multi-stage Build**: Separação das etapas de build e produção
- **Nginx Gzip**: Compressão de assets
- **Cache Headers**: Cache adequado para diferentes tipos de arquivo
- **Asset Optimization**: Build otimizado do Vite
- **Health Checks**: Monitoramento automático da saúde do container