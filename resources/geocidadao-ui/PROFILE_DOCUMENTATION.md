# Seção de Perfil de Usuário - Documentação

## Visão Geral

Esta funcionalidade adiciona um sistema completo de gerenciamento de perfil de usuário ao GeoCidadão, permitindo visualização, navegação e edição de perfis.

## Estrutura de Arquivos

### Tipos TypeScript
- **`src/data/@types/UserProfile.d.ts`**: Define os tipos para perfil de usuário e dados de atualização

### Serviços
- **`src/data/services/userService.ts`**: Serviço responsável pela comunicação com a API de gerenciamento de usuários
  - `getCurrentUserProfile()`: Busca perfil do usuário autenticado
  - `getUserProfile(userId)`: Busca perfil de um usuário específico
  - `getUserPosts(userId)`: Busca postagens de um usuário
  - `updateUserProfile(data)`: Atualiza nome e/ou foto do perfil

### Hooks
- **`src/data/hooks/useUserProfile.ts`**: Hook customizado para gerenciar estado do perfil
  - Gerencia carregamento de perfil e posts
  - Controla estados de loading e erro
  - Determina se é o próprio perfil do usuário
  - Fornece função para atualizar perfil

### Componentes

#### EditProfileModal
**Localização**: `src/ui/components/EditProfileModal.tsx`

Modal para edição de perfil com:
- Upload de foto de perfil com preview
- Edição de nome
- Estados de loading durante atualização
- Validação de formulário

#### NavigationBar
**Localização**: `src/ui/components/NavigationBar.tsx`

Barra de navegação fixa no topo com links para:
- Feed
- Perfil do usuário logado

#### Componentes Atualizados

**Avatar** (`src/ui/components/PostCard/Avatar.tsx`)
- Adicionado suporte para diferentes tamanhos: `small`, `medium`, `large`
- Tamanhos configuráveis para diferentes contextos de uso

**PostHeader** (`src/ui/components/PostCard/PostHeader.tsx`)
- Adicionado navegação ao clicar no nome ou avatar do autor
- Efeitos hover para indicar interatividade

### Páginas

#### ProfilePage
**Localização**: `src/ui/pages/ProfilePage.tsx`

Página de perfil com:
- **Header do perfil**: 
  - Foto de capa com gradiente
  - Avatar do usuário
  - Nome, username e biografia
  - Data de membro desde
  - Estatísticas (número de publicações)
  - Botão "Editar perfil" (apenas para o próprio perfil)
  
- **Seção de publicações**:
  - Lista de todas as postagens do usuário
  - Mensagem quando não há publicações
  - Integração com navegação para detalhes do post
  - Integração com mapa ao clicar no ícone de localização

- **Modal de edição**:
  - Abre ao clicar em "Editar perfil"
  - Permite alterar foto e nome
  - Feedback de erros e loading

### Estilos CSS

- **`src/ui/styles/components/EditProfileModal.css`**: Estilização do modal de edição
- **`src/ui/styles/components/NavigationBar.css`**: Estilização da barra de navegação
- **`src/ui/styles/pages/ProfilePage.css`**: Estilização da página de perfil
- **`src/ui/styles/components/PostCard/Avatar.css`**: Atualizado com suporte a tamanhos
- **`src/ui/styles/components/PostCard/PostHeader.css`**: Atualizado com efeitos hover
- **`src/ui/styles/components/MapLayout.css`**: Ajustado para acomodar barra de navegação

### Utilitários

**dateUtils.ts** - Adicionada nova função:
- `formatDate(timestamp)`: Formata data no formato "mês de ano" (ex: "novembro de 2025")

## Rotas

Foram adicionadas as seguintes rotas ao sistema:

```typescript
{
  path: "profile",
  element: <ProfilePage />,  // Perfil do usuário autenticado
},
{
  path: "profile/:userId",
  element: <ProfilePage />,  // Perfil de outro usuário
}
```

### Exemplos de Uso

- `/profile` - Exibe o perfil do usuário logado
- `/profile/123-456-789` - Exibe o perfil do usuário com ID 123-456-789

## API Endpoints

O serviço de usuário se comunica com os seguintes endpoints (configurados em `API_ENDPOINTS.GERENCIAMENTO_USUARIOS`):

- `GET /usuarios/me` - Busca perfil do usuário autenticado
- `GET /usuarios/{id}` - Busca perfil de um usuário específico
- `GET /usuarios/{id}/posts` - Busca posts de um usuário
- `PUT /usuarios/me` - Atualiza perfil (aceita FormData com Name e ProfilePicture)

## Funcionalidades

### 1. Visualização de Perfil

- Exibir informações do perfil (nome, username, foto, bio)
- Mostrar estatísticas (número de publicações)
- Listar todas as postagens do usuário
- Diferenciar entre próprio perfil e perfil de outros usuários

### 2. Navegação

- Clicar no avatar ou nome em qualquer post navega para o perfil do autor
- Barra de navegação fixa permite acesso rápido ao próprio perfil
- Botão de voltar ao feed em caso de erro

### 3. Edição de Perfil

- Modal de edição acessível através do botão "Editar perfil"
- Upload de nova foto com preview instantâneo
- Edição do nome do usuário
- Validação de formulário
- Feedback visual durante upload/atualização
- Tratamento de erros

### 4. Integração com Posts

- Clicar em um post navega para a página de detalhes
- Clicar no ícone de mapa navega para o feed com foco no post específico
- Mesmo componente PostCard usado no feed

## Fluxo de Uso

### Acessar Próprio Perfil
1. Clicar no botão "Perfil" na barra de navegação
2. Visualizar informações e publicações
3. (Opcional) Clicar em "Editar perfil" para fazer alterações

### Acessar Perfil de Outro Usuário
1. No feed, clicar no avatar ou nome de qualquer autor
2. Visualizar perfil e publicações do usuário
3. (Não há opção de editar perfil de outros usuários)

### Editar Perfil
1. Acessar próprio perfil
2. Clicar em "Editar perfil"
3. Alterar foto (clicando em "Alterar foto" e selecionando imagem)
4. Alterar nome no campo de texto
5. Clicar em "Salvar" para confirmar ou "Cancelar" para descartar

## Responsividade

Todos os componentes são responsivos e adaptam-se a diferentes tamanhos de tela:

- **Desktop**: Layout completo com todos os elementos visíveis
- **Tablet**: Ajustes no tamanho de fontes e espaçamentos
- **Mobile**: 
  - Navegação otimizada com ícones e labels menores
  - Modal ocupa mais espaço vertical
  - Botões de ação em coluna
  - Perfil otimizado para tela pequena

## Temas

Todos os componentes suportam temas claro/escuro através das variáveis CSS:
- `--color-surface-1`, `--color-surface-2`, `--color-surface-3`
- `--color-text-primary`, `--color-text-secondary`
- `--color-primary`, `--color-primary-dark`
- `--color-border`
- `--color-background`

## Considerações de Segurança

- Todas as requisições incluem token de autenticação via Keycloak
- Apenas o próprio usuário pode editar seu perfil
- Upload de imagens validado no frontend (aceita apenas imagens)
- Limitação de caracteres no campo de nome (100 caracteres)

## Próximas Melhorias Sugeridas

1. Adicionar edição de biografia
2. Adicionar estatísticas adicionais (curtidas recebidas, comentários, etc.)
3. Implementar seguir/deixar de seguir usuários
4. Adicionar aba de curtidas na página de perfil
5. Implementar crop de imagem antes do upload
6. Adicionar validação de tamanho de arquivo
7. Adicionar paginação nas publicações do usuário
8. Implementar compartilhamento de perfil
