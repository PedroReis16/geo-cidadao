import ThemeSwitcher from "./ThemeSwitcher";
import "../../ui/styles/components/Header.css";

function Header() {
  return (
    <header className="content" id="page-header">
      <a href="/" id="header-logo" className="headerLogo" />
      <h1>Cabeçalho</h1>
      <ThemeSwitcher />
    </header>
  );
}

export default Header;
