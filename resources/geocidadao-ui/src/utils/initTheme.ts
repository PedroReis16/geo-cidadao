/**
 * Inicializa o tema da aplicação antes do React renderizar
 * Isso previne flash de conteúdo com tema incorreto (FOUC)
 */
export const initTheme = () => {
  const savedTheme = localStorage.getItem('theme') as 'light' | 'dark' | null;
  
  // Se não houver tema salvo, verifica preferência do sistema
  const theme = savedTheme || 
    (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');
  
  document.documentElement.setAttribute('data-theme', theme);
  
  // Salva a preferência se não existir
  if (!savedTheme) {
    localStorage.setItem('theme', theme);
  }
};
