import {
  createBrowserRouter,
  RouterProvider,
  Outlet,
  Navigate,
} from "react-router-dom";

//Components
import ProtectedRoute from "../ui/components/ProtectedRoute";

//Pages
import FeedPage from "./FeedPage";
import PostPage from "./PostPage";
import NotFoundPage from "./NotFoundPage";
import KeycloakCallback from "./KeycloakCallback";

// Styles
import { ThemeProvider } from "../data/contexts/ThemeProvider";

// Layout protegido que envolve todas as rotas
const ProtectedLayout = () => (
  <ProtectedRoute>
    <Outlet />
  </ProtectedRoute>
);

const router = createBrowserRouter([
  {
    path: "/",
    element: <ProtectedLayout />,
    children: [
      {
        index: true,
        element: <Navigate to="/feed" replace />, // Redirect root to /feed
      },
      {
        path: "feed",
        element: <FeedPage />,
      },
      {
        path: "posts/:id",
        element: <PostPage />,
      },
      {
        path: "*",
        element: <NotFoundPage />,
      },
    ],
  },
  {
    // Rota de callback do Keycloak (sem proteção)
    path: "/auth/callback",
    element: <KeycloakCallback />,
  },
]);

function App() {
  return (
    <>
      <ThemeProvider>
        {/* <Header /> */}
        <main className="content">
          <RouterProvider router={router} />
        </main>
      </ThemeProvider>
    </>
  );
}

export default App;
