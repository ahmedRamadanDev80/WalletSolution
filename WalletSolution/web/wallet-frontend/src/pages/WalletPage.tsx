/* eslint-disable @typescript-eslint/no-explicit-any */
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
    MenuItem,
    Select,
    InputLabel,
    FormControl,
    IconButton,
    Tooltip,
} from "@mui/material";
import RefreshIcon from "@mui/icons-material/Refresh";
import AutoAwesomeIcon from "@mui/icons-material/AutoAwesome";
import api from "../api/axios";
import { getBalance, earn, burn, generateExternalRef } from "../api/walletApi";

type WalletDto = { userId: string; balance: number };
type Service = { id: string; name: string; description?: string };

export default function WalletPage() {
    // Try to read userId from localStorage (set by login). still keep a state field for display.
    const storedUserId = localStorage.getItem("userId") ?? "";
    const [userId, setUserId] = useState<string>(storedUserId);
    const [wallet, setWallet] = useState<WalletDto | null>(null);

    const [services, setServices] = useState<Service[]>([]);
    const [selectedServiceId, setSelectedServiceId] = useState<string | "">("");

    const [amount, setAmount] = useState<number | "">("");
    const [externalRef, setExternalRef] = useState<string>("");
    const [description, setDescription] = useState<string>("");
    const [loading, setLoading] = useState<boolean>(false);
    const [loadingServices, setLoadingServices] = useState<boolean>(false);

    // Load services on mount
    useEffect(() => {
        let mounted = true;
        const loadServices = async () => {
            setLoadingServices(true);
            try {
                const res = await api.get<Service[]>("/services");
                if (!mounted) return;
                // adapt to shape if backend returns ServiceDto names differently
                setServices(res.data ?? []);
            } catch (err) {
                console.error("Failed to load services", err);
            } finally {
                if (mounted) setLoadingServices(false);
            }
        };
        loadServices();
        return () => {
            mounted = false;
        };
    }, []);

    // Load wallet on mount if we have token / userId
    useEffect(() => {
        if (!localStorage.getItem("jwt")) return;
        loadWallet();
    }, []);

    async function loadWallet() {
        setLoading(true);
        try {
            const res = await getBalance(); // new API does not require userId param
            setWallet({ userId: res.userId, balance: Number(res.balance ?? 0) });
            // keep userId in sync with what backend says (useful if login stored different id)
            if (res.userId) setUserId(res.userId);
            if (res.userId) localStorage.setItem("userId", res.userId);
        } catch (err: any) {
            console.error("loadWallet error", err);
            setWallet(null);
        } finally {
            setLoading(false);
        }
    }

    async function handleEarn() {
        if (!amount || Number(amount) <= 0) return alert("Enter amount > 0");
        setLoading(true);
        try {
            const res = await earn(
                Number(amount),
                selectedServiceId || undefined,
                externalRef || undefined,
                description || undefined
            );
            setWallet({ userId: res.userId, balance: Number(res.balance ?? 0) });
            setAmount("");
            // clear externalRef/description optionally
            setExternalRef("");
            setDescription("");
        } catch (err: any) {
            console.error("earn error", err);
            alert(err?.response?.data?.message ?? "Earn failed");
        } finally {
            setLoading(false);
        }
    }

    async function handleBurn() {
        if (!amount || Number(amount) <= 0) return alert("Enter amount > 0");
        setLoading(true);
        try {
            const res = await burn(
                Number(amount),
                externalRef || undefined,
                description || undefined,
                // use selectedServiceId (not a non-existing serviceId variable)
                (selectedServiceId && selectedServiceId !== "") ? selectedServiceId : undefined
            );

            setWallet({ userId: res.userId, balance: Number(res.balance ?? 0) });
            setAmount("");
            setExternalRef("");
            setDescription("");
        } catch (err: any) {
            console.error("burn error", err);
            alert(err?.response?.data?.message ?? "Burn failed");
        } finally {
            setLoading(false);
        }
    }

    function handleGenerateExternalRef() {
        const r = generateExternalRef();
        setExternalRef(r);
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
                            helperText={!userId ? "No userId found in localStorage. Login first or paste demo GUID." : undefined}
                        />
                        <Button variant="contained" onClick={loadWallet} disabled={loading}>
                            {loading ? <CircularProgress size={18} /> : "Load"}
                        </Button>
                        <Tooltip title="Refresh balance">
                            <IconButton onClick={loadWallet} disabled={loading}>
                                <RefreshIcon />
                            </IconButton>
                        </Tooltip>
                    </Stack>

                    <Box mt={3}>
                        <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
                            <TextField
                                label="External reference"
                                fullWidth
                                value={externalRef}
                                onChange={(e) => setExternalRef(e.target.value)}
                            />
                            <Tooltip title="Generate external reference">
                                <IconButton onClick={handleGenerateExternalRef} aria-label="gen-ext">
                                    <AutoAwesomeIcon />
                                </IconButton>
                            </Tooltip>

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
                                label="Amount (SAR)"
                                type="number"
                                sx={{ width: { xs: "100%", sm: 180 } }}
                                value={amount}
                                onChange={(e) => setAmount(e.target.value === "" ? "" : Number(e.target.value))}
                            />

                            <FormControl sx={{ minWidth: 220 }}>
                                <InputLabel id="service-select-label">Service</InputLabel>
                                <Select
                                    labelId="service-select-label"
                                    value={selectedServiceId}
                                    label="Service"
                                    onChange={(e) => setSelectedServiceId(e.target.value as string)}
                                    disabled={loadingServices}
                                >
                                    <MenuItem value="">
                                        <em>None / Default</em>
                                    </MenuItem>
                                    {services.map((s) => (
                                        <MenuItem key={s.id} value={s.id}>
                                            {s.name}
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>

                            <Button variant="contained" color="success" onClick={handleEarn} disabled={loading}>
                                Earn
                            </Button>

                            <Button variant="outlined" color="error" onClick={handleBurn} disabled={loading}>
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
