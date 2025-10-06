// O papel dessa pasta é centralizar a lógica de estado global ou de compartilhamento de dados entre múltiplos componentes, sem a necessidade de passar props manualmente por vários níveis (o famoso prop drilling).

// Por exemplo, dentro da pasta contexts, você poderia ter arquivos como:

// AuthContext.tsx → Armazena informações do usuário logado e permissões.

// ThemeContext.tsx → Gerencia tema claro/escuro da aplicação.

// LanguageContext.tsx → Gerencia o idioma da interface.