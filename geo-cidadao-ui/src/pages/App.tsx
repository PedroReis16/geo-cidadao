import {
  createBrowserRouter,
  RouterProvider,
  Outlet,
  Navigate,
} from "react-router-dom";

//Components
import Header from "../ui/components/Header";

//Pages
import HomePage from "./HomePage";
import PostPage from "./PostPage";
import NotFoundPage from "./NotFoundPage";

// Styles
import "../ui/styles/App.css";
import { ThemeProvider } from "../data/contexts/ThemeProvider";

const router = createBrowserRouter([
  {
    path: "/",
    element: <Outlet />,
    children: [
      {
        index: true,
        path: "feed",
        element: <HomePage />,
      },
      {
        path: "posts/:id",
        element: <PostPage />,
      },
      {
        path: "*",
        element: <NotFoundPage />,
      },
      {
        index: true,
        element: <Navigate to="/feed" replace />, // Redirect root to /feed
      },
    ],
  },
]);

function App() {
  return (
    <>
      <ThemeProvider>
        <Header />
        <main className="content">
          <RouterProvider router={router} />
        </main>
      </ThemeProvider>
    </>
  );
}

export default App;
