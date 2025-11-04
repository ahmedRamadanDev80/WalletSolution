// src/pages/AdminDashboard.tsx
import { useEffect, useState } from "react";
import {
    Box,
    Card,
    CardContent,
    Typography,
    CircularProgress,
    Stack,
} from "@mui/material";
import { getKpis } from "../api/adminApi";
import {
    BarChart,
    Bar,
    XAxis,
    YAxis,
    Tooltip,
    ResponsiveContainer,
    Cell,
    CartesianGrid,
    LabelList,
} from "recharts";

type Kpis = { totalEarned: number; totalBurned: number; activeWallets: number };

export default function AdminDashboard() {
    const [kpis, setKpis] = useState<Kpis | null>(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        load();
    }, []);

    async function load() {
        setLoading(true);
        try {
            const res = await getKpis();
            setKpis(res);
        } catch (err) {
            console.error(err);
            alert("Failed to load KPIs");
        } finally {
            setLoading(false);
        }
    }

    const earnedBurnData = kpis
        ? [
            { name: "Earned", value: kpis.totalEarned, color: "#2E7D32" }, // green
            { name: "Burned", value: kpis.totalBurned, color: "#C62828" }, // red
        ]
        : [];

    const activeWalletsData = kpis
        ? [{ name: "Active Wallets", value: kpis.activeWallets, color: "#1565C0" }]
        : [];

    // safer tooltip formatter type (no `any`)
    const tooltipFormatter = (value: number | string) => {
        if (typeof value === "number") return value.toLocaleString();
        return String(value);
    };

    return (
        <Box sx={{ maxWidth: 1100, margin: "24px auto", padding: 2 }}>
            <Typography variant="h5" mb={2} textAlign="center">
                Admin Dashboard
            </Typography>

            {/* KPI cards row */}
            <Stack direction={{ xs: "column", md: "row" }} spacing={2} mb={2}>
                <Card sx={{ flex: 1 }}>
                    <CardContent>
                        <Typography variant="subtitle2">Total Earned</Typography>
                        <Typography variant="h6">
                            {kpis ? kpis.totalEarned : <CircularProgress size={18} />}
                        </Typography>
                    </CardContent>
                </Card>

                <Card sx={{ flex: 1 }}>
                    <CardContent>
                        <Typography variant="subtitle2">Total Burned</Typography>
                        <Typography variant="h6">
                            {kpis ? kpis.totalBurned : <CircularProgress size={18} />}
                        </Typography>
                    </CardContent>
                </Card>

                <Card sx={{ flex: 1 }}>
                    <CardContent>
                        <Typography variant="subtitle2">Active Wallets</Typography>
                        <Typography variant="h6">
                            {kpis ? kpis.activeWallets : <CircularProgress size={18} />}
                        </Typography>
                    </CardContent>
                </Card>
            </Stack>

            {/* Charts area: left = Earn/Burn, right = Active Wallets */}
            <Stack direction={{ xs: "column", md: "row" }} spacing={2}>
                {/* Earned & Burned - larger (takes 2/3 width on md+) */}
                <Box sx={{ flex: { xs: "1 1 100%", md: "2 1 66%" } }}>
                    <Card sx={{ height: { xs: 260, md: 360 } }}>
                        <CardContent sx={{ height: "100%" }}>
                            {loading ? (
                                <Box
                                    display="flex"
                                    alignItems="center"
                                    justifyContent="center"
                                    height="100%"
                                >
                                    <CircularProgress />
                                </Box>
                            ) : (
                                <ResponsiveContainer width="100%" height="100%">
                                    <BarChart
                                        data={earnedBurnData}
                                        margin={{ top: 20, right: 24, left: 12, bottom: 20 }}
                                    >
                                        <CartesianGrid strokeDasharray="3 3" />
                                        <XAxis dataKey="name" />
                                        <YAxis />
                                        <Tooltip formatter={tooltipFormatter} />
                                        <Bar dataKey="value" barSize={60}>
                                            {earnedBurnData.map((entry) => (
                                                <Cell key={entry.name} fill={entry.color} />
                                            ))}
                                            <LabelList
                                                dataKey="value"
                                                position="top"
                                                formatter={(v) => String(v)}
                                            />
                                        </Bar>
                                    </BarChart>
                                </ResponsiveContainer>
                            )}
                        </CardContent>
                    </Card>
                </Box>

                {/* Active Wallets - separate smaller chart (takes 1/3 width on md+) */}
                <Box sx={{ flex: { xs: "1 1 100%", md: "1 1 34%" } }}>
                    <Card
                        sx={{
                            height: { xs: 260, md: 360 },
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                        }}
                    >
                        <CardContent sx={{ height: "100%", width: "100%", padding: 2 }}>
                            {loading ? (
                                <Box
                                    display="flex"
                                    alignItems="center"
                                    justifyContent="center"
                                    height="100%"
                                >
                                    <CircularProgress />
                                </Box>
                            ) : (
                                <ResponsiveContainer width="100%" height="100%">
                                    <BarChart
                                        data={activeWalletsData}
                                        layout="vertical"
                                        margin={{ top: 12, right: 12, left: 12, bottom: 12 }}
                                    >
                                        <CartesianGrid strokeDasharray="3 3" horizontal={false} />
                                        <XAxis type="number" />
                                        <YAxis dataKey="name" type="category" width={140} />
                                        <Tooltip formatter={tooltipFormatter} />
                                        <Bar dataKey="value" barSize={20}>
                                            {activeWalletsData.map((entry) => (
                                                <Cell key={entry.name} fill={entry.color} />
                                            ))}
                                            <LabelList
                                                dataKey="value"
                                                position="right"
                                                formatter={(v) => String(v)}
                                            />
                                        </Bar>
                                    </BarChart>
                                </ResponsiveContainer>
                            )}
                        </CardContent>
                    </Card>
                </Box>
            </Stack>
        </Box>
    );
}
