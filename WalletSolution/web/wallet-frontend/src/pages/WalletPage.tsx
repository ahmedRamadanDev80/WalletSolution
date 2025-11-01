import { useEffect, useState } from "react";
import {
    Box,
    Card,
    CardContent,
    TextField,
    Button,
    Typography,
    CircularProgress,
    Stack,
} from "@mui/material";
import { getBalance, earn, burn } from "../api/walletApi";

type WalletDto = { userId: string; balance: number };

export default function WalletPage() {
    const [userId, setUserId] = useState<string>("");
    const [wallet, setWallet] = useState<WalletDto | null>(null);
    const [amount, setAmount] = useState<number | "">("");
    const [externalRef, setExternalRef] = useState<string>("");
    const [description, setDescription] = useState<string>("");
    const [loading, setLoading] = useState<boolean>(false);

    useEffect(() => {
        if (!userId) {
            setWallet(null);
            return;
        }
        const t = setTimeout(() => {
            loadWallet(userId);
        }, 450);
        return () => clearTimeout(t);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [userId]);

    async function loadWallet(id?: string) {
        const uid = id ?? userId;
        if (!uid) return;
        setLoading(true);
        try {
            const res = await getBalance(uid);
            setWallet({ userId: res.userId, balance: Number(res.balance ?? 0) });
        } catch (err: any) {
            console.error("loadWallet error", err);
            setWallet(null);
        } finally {
            setLoading(false);
        }
    }

    async function handleEarn() {
        if (!userId) return alert("Enter user GUID");
        if (!amount || Number(amount) <= 0) return alert("Enter amount > 0");
        setLoading(true);
        try {
            const res = await earn(userId, Number(amount), externalRef || undefined, description || undefined);
            setWallet({ userId: res.userId, balance: Number(res.balance ?? 0) });
            setAmount("");
        } catch (err: any) {
            console.error("earn error", err);
            alert(err?.response?.data?.message ?? "Earn failed");
        } finally {
            setLoading(false);
        }
    }

    async function handleBurn() {
        if (!userId) return alert("Enter user GUID");
        if (!amount || Number(amount) <= 0) return alert("Enter amount > 0");
        setLoading(true);
        try {
            const res = await burn(userId, Number(amount), externalRef || undefined, description || undefined);
            setWallet({ userId: res.userId, balance: Number(res.balance ?? 0) });
            setAmount("");
        } catch (err: any) {
            console.error("burn error", err);
            alert(err?.response?.data?.message ?? "Burn failed");
        } finally {
            setLoading(false);
        }
    }

    return (
        <Box sx={{ maxWidth: 900, margin: "24px auto", padding: 2 }}>
            <Card>
                <CardContent>
                    <Typography variant="h5" gutterBottom>
                        Wallet
                    </Typography>

                    <Stack direction={{ xs: "column", sm: "row" }} spacing={2} alignItems="center">
                        <TextField
                            label="User GUID"
                            placeholder="aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee"
                            fullWidth
                            value={userId}
                            onChange={(e) => setUserId(e.target.value)}
                        />
                        <Button variant="contained" onClick={() => loadWallet()} disabled={!userId || loading}>
                            {loading ? <CircularProgress size={18} /> : "Load"}
                        </Button>
                    </Stack>

                    <Box mt={3}>
                        <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
                            <TextField
                                label="External reference"
                                fullWidth
                                value={externalRef}
                                onChange={(e) => setExternalRef(e.target.value)}
                            />
                            <TextField
                                label="Description"
                                fullWidth
                                value={description}
                                onChange={(e) => setDescription(e.target.value)}
                            />
                        </Stack>
                    </Box>

                    <Box mt={3}>
                        <Typography variant="subtitle2">Balance</Typography>
                        <Typography variant="h4" gutterBottom>
                            {wallet ? wallet.balance : "---"}
                        </Typography>

                        <Stack direction={{ xs: "column", sm: "row" }} spacing={2} alignItems="center">
                            <TextField
                                label="Amount"
                                type="number"
                                sx={{ width: { xs: "100%", sm: 180 } }}
                                value={amount}
                                onChange={(e) => setAmount(e.target.value === "" ? "" : Number(e.target.value))}
                            />

                            <Button variant="contained" color="success" onClick={handleEarn} disabled={loading || !userId}>
                                Earn
                            </Button>

                            <Button variant="outlined" color="error" onClick={handleBurn} disabled={loading || !userId}>
                                Burn
                            </Button>

                            <Box flex={1} />
                        </Stack>
                    </Box>
                </CardContent>
            </Card>
        </Box>
    );
}
