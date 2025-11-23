import { ThemeProvider } from "./data/contexts/ThemeProvider";
import { RouterProvider } from "react-router-dom";
import Router from "./data/contexts/RouteProvider";

function App() {

  return (
    <>
      <ThemeProvider>
        <main>
          <RouterProvider router={Router} />
        </main>
      </ThemeProvider>
    </>
  );
}

export default App;
