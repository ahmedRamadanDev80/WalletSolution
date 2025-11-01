import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
    Box,
    Button,
    Container,
    TextField,
    Typography,
    Paper,
    CircularProgress,
} from "@mui/material";
import api from "../api/axios";

export default function LoginPage() {
    const [email, setEmail] = useState<string>("");
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!email) {
            alert("Please enter an email or demo GUID.");
            return;
        }

        setLoading(true);
        try {

            const res = await api.post("/auth/login", { userId: email });
            const token = res.data?.token;
            const userId = res.data?.userId;

            if (!token) throw new Error("No token returned from server");

            localStorage.setItem("jwt", token);
            if (userId) localStorage.setItem("userId", userId);


            window.location.replace("/wallet");

        } catch (err) {
            console.error(err);
            alert("Login failed. Make sure backend is running and /auth/login is reachable.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Container
            maxWidth="xs"
            sx={{
                height: "100vh",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
            }}
        >
            <Paper elevation={6} sx={{ p: 4, borderRadius: 2, width: "100%" }}>
                <Typography variant="h5" align="center" fontWeight="bold" mb={2}>
                    Wallet Demo — Login
                </Typography>

                <Box component="form" onSubmit={handleLogin}>
                    <TextField
                        label="Email or Demo GUID"
                        variant="outlined"
                        fullWidth
                        required
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        sx={{ mb: 2 }}
                        placeholder="you@example.com or AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE"
                    />

                    <Button
                        type="submit"
                        variant="contained"
                        fullWidth
                        disabled={loading}
                        sx={{ py: 1.2 }}
                    >
                        {loading ? <CircularProgress size={22} color="inherit" /> : "Login"}
                    </Button>
                </Box>
            </Paper>
        </Container>
    );
}
