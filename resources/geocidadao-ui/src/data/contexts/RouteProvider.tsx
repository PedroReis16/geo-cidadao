// src/data/contexts/RouteProvider.tsx
import { createBrowserRouter, Outlet, Navigate } from "react-router-dom";
import FeedPage from "../../ui/pages/FeedPage";
import PostDetailsPage from "../../ui/pages/PostDetailsPage";
import KeycloakCallback from "../../ui/pages/KeycloakCallback";
import NotFoundPage from "../../ui/pages/NotFound";
import ProtectedRouter from "../../ui/components/ProtectedRouter";
import MapLayout from "../../ui/components/MapLayout";
import { MapProvider } from "./MapProvider";

const ProtectedLayout = () => (
  <ProtectedRouter>
    <MapProvider>
      <MapLayout>
        <Outlet />
      </MapLayout>
    </MapProvider>
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
        element: <FeedPage />,
      },
      {
        path: "post/:postId",
        element: <PostDetailsPage />,
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
