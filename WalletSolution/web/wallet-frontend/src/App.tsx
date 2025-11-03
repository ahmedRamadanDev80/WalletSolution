import { useCallback, useEffect, useState } from "react";
import { BrowserRouter, Routes, Route, Navigate, useNavigate } from "react-router-dom";
import { Container } from "@mui/material";
import WalletPage from "./pages/WalletPage";
import TransactionsPage from "./pages/TransactionsPage";
import LoginPage from "./pages/LoginPage";
import ProtectedRoute from "./routes/ProtectedRoute";
import TopBar from "./components/TopBar";
import './App.css';
import AdminRulesPage from "./pages/AdminRulesPage";
import AdminDashboard from "./pages/AdminDashboard";

function AppRoutesWrapper() {
  // we keep routing logic inside a nested component to use hooks (useNavigate)
  const navigate = useNavigate();
  const [authenticated, setAuthenticated] = useState<boolean>(!!localStorage.getItem("jwt"));

  // expose logout to TopBar via callback
  const handleLogout = useCallback(() => {
    localStorage.removeItem("jwt");
    localStorage.removeItem("userId");
    setAuthenticated(false);
    navigate("/login");
  }, [navigate]);

  // keep `authenticated` in sync when jwt value changes in other tabs (storage event)
  useEffect(() => {
    const onStorage = (e: StorageEvent) => {
      if (e.key === "jwt") {
        setAuthenticated(!!e.newValue);
      }
    };
    window.addEventListener("storage", onStorage);
    return () => window.removeEventListener("storage", onStorage);
  }, []);

  // When component mounts, ensure we read current token state
  useEffect(() => {
    setAuthenticated(!!localStorage.getItem("jwt"));
  }, []);

  return (
    <>
      <TopBar authenticated={authenticated} onLogout={handleLogout} />

      <Container sx={{ mt: 10 }} maxWidth="lg">
        <Routes>
          {/* Public route */}
          <Route path="/login" element={<LoginPage />} />

          {/* Protected routes */}
          <Route
            path="/"
            element={
              <ProtectedRoute>
                <WalletPage />
              </ProtectedRoute>
            }
          />
          <Route
            path="/transactions"
            element={
              <ProtectedRoute>
                <TransactionsPage />
              </ProtectedRoute>
            }
          />
          <Route path="/admin/rules" element={<ProtectedRoute><AdminRulesPage /></ProtectedRoute>} />
          <Route path="/admin/dashboard" element={<ProtectedRoute><AdminDashboard /></ProtectedRoute>} />

          {/* fallback: redirect to wallet */}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </Container>
    </>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AppRoutesWrapper />
    </BrowserRouter>
  );
}
