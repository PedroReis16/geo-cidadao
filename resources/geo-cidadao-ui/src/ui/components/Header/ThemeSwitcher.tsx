import React from "react";
import { useTheme } from "../../../data/contexts/ThemeContext";
import "../../../ui/styles/components/Header/ThemeSwitcher.css";

const ThemeSwitcher: React.FC = () => {
  const { theme, toggleTheme } = useTheme();
  return (
    <button onClick={toggleTheme}>
      {theme === "light" ? "🌙 Escuro" : "☀️ Claro"}
    </button>
  );
};

export default ThemeSwitcher;
