# Sistema de Criação de Posts - GeoCidadão

## Funcionalidades Implementadas

### PostCreator Component

O componente `PostCreator` agora possui as seguintes funcionalidades:

1. **Criação de Posts com Texto**
   - Campo de texto expansível automaticamente
   - Validação de conteúdo mínimo

2. **Upload de Mídias**
   - Suporte para até 10 arquivos
   - Formatos aceitos:
     - Imagens: JPEG, PNG
     - Vídeos: MP4
   - Drag & drop de arquivos
   - Preview das mídias selecionadas
   - Remoção individual de arquivos

3. **Seleção de Localização**
   - **Botão "Localização" ao lado dos botões de mídia**
   - Ao clicar, o **mapa expande em tela cheia** sobre o conteúdo
   - **Banner azul no topo** com instruções e botões de ação
   - Usuário clica diretamente no mapa para selecionar posição
   - **Marcador verde** indica a posição selecionada
   - Ao confirmar:
     - Mapa fecha automaticamente
     - **Card azul exibe informações da localização:**
       - Nome da rua/endereço (via geocodificação reversa Nominatim)
       - Coordenadas completas
       - Botão para **editar** localização (reabre o mapa)
       - Botão para **remover** localização
   - Geocodificação reversa para obter endereço legível

4. **Integração com API**
   - Requisição POST para `/gerenciamento-posts/Posts`
   - Envio de FormData com:
     - `Content`: texto do post
     - `Latitude` e `Longitude`: coordenadas (opcional)
     - `MediaFiles`: arquivos de mídia (opcional)
   - Autenticação via Keycloak (Bearer token)

## Configuração

### Variáveis de Ambiente

Crie um arquivo `.env` na raiz do projeto `geocidadao-ui` baseado no `.env.example`:

```env
VITE_API_URL=http://localhost:8081
VITE_KEYCLOAK_URL=http://localhost:8082/
VITE_KEYCLOAK_REALM=geocidadao
VITE_KEYCLOAK_CLIENT_ID=geocidadao-ui
```

## Uso

### Criar um Post com Localização

1. Digite o texto no campo de publicação
2. (Opcional) Adicione fotos/vídeos
3. **Clique no botão "Localização"** (ao lado de Foto/Vídeo)
4. O mapa expande em tela cheia com banner de instruções
5. Navegue e **clique no mapa** na posição desejada
6. Um **marcador verde** aparece na posição
7. Clique em **"Confirmar"** no banner
8. O mapa fecha e exibe um **card azul** com:
   - 📍 Nome da rua/endereço
   - Coordenadas (lat, lng)
   - Botão ✏️ para editar
   - Botão ❌ para remover
9. Clique em "Publicar"

### Editar Localização

1. No card de localização, clique no ícone de **edição** (✏️)
2. O mapa expande novamente
3. Selecione nova posição
4. Confirme para atualizar

### Remover Localização

1. No card de localização, clique no **X**
2. A localização é removida do post

## Fluxo Visual

```
┌─────────────────────────────────────────────┐
│ [Foto] [Vídeo] [📍 Localização]             │
└─────────────────────────────────────────────┘
         ↓ (clique)
┌─────────────────────────────────────────────┐
│ 📍 Clique no mapa...  [Cancelar] [Confirmar]│ ← Banner fixo
├─────────────────────────────────────────────┤
│                                             │
│         MAPA EM TELA CHEIA                  │
│              (clique aqui)                  │
│                  ↓                          │
│               🟢 Marcador                    │
│                                             │
└─────────────────────────────────────────────┘
         ↓ (confirmar)
┌─────────────────────────────────────────────┐
│ 📍  Rua Exemplo, 123 - São Paulo           │
│    -23.550520, -46.633308        [✏️] [❌]  │ ← Card azul
└─────────────────────────────────────────────┘
```

## Arquivos Criados/Modificados

### Criados
- `src/config/api.ts` - Configuração de endpoints da API
- `src/data/services/postService.ts` - Serviço de criação de posts
- `src/data/services/geocodingService.ts` - Serviço de geocodificação reversa (Nominatim)
- `.env.example` - Exemplo de variáveis de ambiente

### Modificados
- `src/ui/components/PostCreator.tsx` - Implementação completa do fluxo de seleção
- `src/ui/components/MapLayout.tsx` - Handler para clique no mapa
- `src/ui/styles/components/PostCreator.css` - Estilos para card e banner

## Observações Técnicas

- **Integração com MapContext** para controle do mapa global
- **Nominatim** para geocodificação reversa (endereço a partir de coordenadas)
- **Mapa expande em tela cheia** quando em modo de seleção
- **Banner fixo no topo** com animação de slide down
- **Card de localização** com gradiente azul e animação de entrada
- **Botões de edição e remoção** no próprio card
- **Fallback** para coordenadas se geocodificação falhar
- **Responsivo** e funciona em dispositivos móveis
- **Loading state** enquanto busca endereço
- **Autenticação automática** via Keycloak

