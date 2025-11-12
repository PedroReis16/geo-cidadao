document.addEventListener("DOMContentLoaded", () => {
  const isDarkMode =
    window.matchMedia &&
    window.matchMedia("(prefers-color-scheme: dark)").matches;

  const logo = document.getElementById("logo");

  // Substitua pelos caminhos corretos do seu tema
  const logoLight = "${url.resourcesPath}/img/logo-light.svg";
  const logoDark = "${url.resourcesPath}/img/logo-dark.svg";

  logo.src = isDarkMode ? logoDark : logoLight;

  // Detecta mudanças de tema em tempo real
  if (window.matchMedia) {
    window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", (e) => {
      logo.src = e.matches ? logoDark : logoLight;
    });
  }
});
