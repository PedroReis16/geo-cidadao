// src/data/contexts/RouteProvider.tsx
import { createBrowserRouter, Outlet, Navigate } from "react-router-dom";
import Feed from "../../ui/pages/Feed";
import KeycloakCallback from "../../ui/pages/KeycloakCallback";
import NotFoundPage from "../../ui/pages/NotFound";
import ProtectedRouter from "../../ui/components/ProtectedRouter";

const ProtectedLayout = () => (
  <ProtectedRouter>
    <Outlet />
  </ProtectedRouter>
);

const Router = createBrowserRouter([
  {
    path: "/",
    element: <ProtectedLayout />,
    children: [
      {
        index: true,
        element: <Navigate to="/feed" replace />,
      },
      {
        path: "feed",
        element: <Feed />,
      },
      {
        path: "*",
        element: <NotFoundPage />,
      },
    ],
  },
  {
    path: "/auth/callback",
    element: <KeycloakCallback />,
  },
]);

export default Router;
