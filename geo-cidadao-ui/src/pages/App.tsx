import {
  createBrowserRouter,
  RouterProvider,
  Outlet,
  Navigate,
} from "react-router-dom";

//Components
import Header from "../ui/components/Header/Header";

//Pages
import FeedPage from "./FeedPage";
import PostPage from "./PostPage";
import NotFoundPage from "./NotFoundPage";

// Styles
import { ThemeProvider } from "../data/contexts/ThemeProvider";

const router = createBrowserRouter([
  {
    path: "/",
    element: <Outlet />,
    children: [
      {
        index: true,
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
