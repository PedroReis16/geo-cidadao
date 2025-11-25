import React from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { Home, User, MessageCircle } from "lucide-react";
import "../styles/components/NavigationBar.css";

const NavigationBar: React.FC = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const isActive = (path: string) => {
    return location.pathname === path || location.pathname.startsWith(path);
  };

  return (
    <nav className="navigation-bar">
      <div className="navigation-container">
        <button
          className={`nav-btn ${isActive("/feed") ? "nav-btn--active" : ""}`}
          onClick={() => navigate("/feed")}
          aria-label="Feed"
        >
          <Home size={24} />
          <span className="nav-label">Feed</span>
        </button>

        <button
          className={`nav-btn ${isActive("/profile") && !location.pathname.includes("/profile/") ? "nav-btn--active" : ""}`}
          onClick={() => navigate("/profile")}
          aria-label="Perfil"
        >
          <User size={24} />
          <span className="nav-label">Perfil</span>
        </button>
      </div>
    </nav>
  );
};

export default NavigationBar;
