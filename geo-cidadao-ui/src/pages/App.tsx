import { BrowserRouter, Routes, Route } from "react-router-dom";

//Components
import Header from "../ui/components/Header";

//Pages
import HomePage from "./HomePage";

// Styles
import "../ui/styles/App.css";

function App() {
  return (
    <BrowserRouter>
      <Header />
      <main>
        <Routes>
          <Route path="/" element={<HomePage />} />
        </Routes>
      </main>
    </BrowserRouter>
  );
}

export default App;
