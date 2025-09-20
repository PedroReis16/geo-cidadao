import React from "react";
import { useTheme } from "../../data/contexts/ThemeContext";


const ThemeSwitcher: React.FC = () => {
  const { theme, toggleTheme } = useTheme();
  return (
    <button onClick={toggleTheme}>
      {theme === "light" ? "🌙 Escuro" : "☀️ Claro"}
    </button>
  );
};

function Header() {

  return (
    <header style={{ background: "var(--color-card)", padding: "1rem" }}>
      <h1>Cabeçalho</h1>
      <ThemeSwitcher />
    </header>
  );
}

export default Header;
