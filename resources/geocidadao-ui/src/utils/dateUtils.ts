/**
 * Formata timestamp para exibição amigável
 * Exemplos: "agora", "5min", "2h", "3d", "15 de nov"
 */
export const formatTimestamp = (timestamp: string): string => {
  const date = new Date(timestamp);
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffMins = Math.floor(diffMs / 60000);
  const diffHours = Math.floor(diffMs / 3600000);
  const diffDays = Math.floor(diffMs / 86400000);

  if (diffMins < 1) return "agora";
  if (diffMins < 60) return `${diffMins}min`;
  if (diffHours < 24) return `${diffHours}h`;
  if (diffDays < 7) return `${diffDays}d`;

  return date.toLocaleDateString("pt-BR", {
    day: "2-digit",
    month: "short",
  });
};

/**
 * Formata data completa
 */
export const formatFullDate = (timestamp: string): string => {
  const date = new Date(timestamp);
  return date.toLocaleString("pt-BR", {
    day: "2-digit",
    month: "long",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
};

/**
 * Verifica se uma data é hoje
 */
export const isToday = (timestamp: string): boolean => {
  const date = new Date(timestamp);
  const today = new Date();
  return (
    date.getDate() === today.getDate() &&
    date.getMonth() === today.getMonth() &&
    date.getFullYear() === today.getFullYear()
  );
};

/**
 * Verifica se uma data é desta semana
 */
export const isThisWeek = (timestamp: string): boolean => {
  const date = new Date(timestamp);
  const now = new Date();
  const diffMs = now.getTime() - date.getTime();
  const diffDays = Math.floor(diffMs / 86400000);
  return diffDays < 7;
};

/**
 * Formata data para exibição em formato "mês de ano"
 * Exemplo: "novembro de 2025"
 */
export const formatDate = (timestamp: string): string => {
  const date = new Date(timestamp);
  return date.toLocaleDateString("pt-BR", {
    month: "long",
    year: "numeric",
  });
};
