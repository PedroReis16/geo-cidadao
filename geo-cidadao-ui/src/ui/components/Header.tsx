import ThemeSwitcher from "./ThemeSwitcher";
import "../../ui/styles/components/Header.css";

function Header() {
  return (
    <header className="content" id="page-header">
      <a href="/" id="header-logo" className="headerLogo" />
      <div id="searchField">
        <i id="searchIcon" />
        <input id="search-input" type="text" placeholder="Pesquisar" />
      </div>
      <ThemeSwitcher />
    </header>
  );
}

export default Header;
