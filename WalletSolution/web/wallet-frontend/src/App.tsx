import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import { AppBar, Toolbar, Typography, Container, Button } from "@mui/material";
import WalletPage from "./pages/WalletPage";
import TransactionsPage from "./pages/TransactionsPage";
import './App.css'

function App() {

  return (
    <>
      <BrowserRouter>
        <AppBar position="static">
          <Toolbar>
            <Typography variant="h6" sx={{ flexGrow: 1 }}>Wallet Demo</Typography>
            <Button color="inherit" component={Link} to="/">Wallet</Button>
            <Button color="inherit" component={Link} to="/transactions">Transactions</Button>
          </Toolbar>
        </AppBar>

        <Container sx={{ mt: 4 }}>
          <Routes>
            <Route path="/" element={<WalletPage />} />
            <Route path="/transactions" element={<TransactionsPage />} />
          </Routes>
        </Container>
      </BrowserRouter>
    </>
  );
}

export default App
