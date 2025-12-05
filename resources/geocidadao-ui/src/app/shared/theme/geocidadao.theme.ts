import { definePreset } from '@primeng/themes';
import Aura from '@primeng/themes/aura';

export const primaryPalette = {
  light: {
    'background':'#f9fafb',
    'text': '#1c1c1c',
    'card': '#ffffff',
    'primary': '#0a4d68',
    'secondary': '#08bdbd',
    'alert': '#ff6b35',
    'border': '#e5e5e5',
    'hover': 'rgba(10, 77, 104, 0.1)',
    'shadow': '0 2px 8px rgba(0, 0, 0, 0.1)',
  },
  dark: {
    'background':'#242424',
    'text': '#e5e5e5',
    'card': '#242424',
    'primary': '#0a4d68',
    'secondary': '#08bdbd',
    'alert': '#ff6b35',
    'border': '#333',
    'hover': 'rgba(255, 255, 255, 0.05)',
    'shadow': '0 2px 8px rgba(0, 0, 0, 0.3)',
  },
};


export function injectPaletteVariables(){
  const root = document.documentElement;
  const isDark = localStorage.getItem('theme') === 'dark';
  const palette = isDark ? primaryPalette.dark : primaryPalette.light;

  Object.entries(palette).forEach(([key, value]) => {
    root.style.setProperty(`--${key}`, value);
  });
}