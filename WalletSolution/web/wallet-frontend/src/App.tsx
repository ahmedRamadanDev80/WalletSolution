import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import WalletPage from "./pages/WalletPage";
import TransactionsPage from "./pages/TransactionsPage";
import './App.css'

function App() {

  return (
    <>
    <BrowserRouter>
      <div style={{ padding: 12, background: "#f5f7fb" }}>
        <nav style={{ maxWidth: 900, margin: "0 auto", display: "flex", gap: 8 }}>
          <Link to="/">Wallet</Link>
          <Link to="/transactions">Transactions</Link>
        </nav>
      </div>

      <Routes>
        <Route path="/" element={<WalletPage />} />
        <Route path="/transactions" element={<TransactionsPage />} />
      </Routes>
    </BrowserRouter>
    </>
  );
}

export default App
