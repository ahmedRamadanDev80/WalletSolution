import { useEffect, useState } from "react";
import { Box, Card, CardContent, Typography, CircularProgress, Stack } from "@mui/material";
import { getKpis } from "../api/adminApi";
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer } from "recharts";

export default function AdminDashboard() {
    const [kpis, setKpis] = useState<{ totalEarned: number; totalBurned: number; activeWallets: number } | null>(null);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        load();
        // eslint-disable-next-line react-hooks/exhaustive-deps
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

    const chartData = kpis
        ? [
            { name: "Earned", value: kpis.totalEarned },
            { name: "Burned", value: kpis.totalBurned },
            { name: "Active Wallets", value: kpis.activeWallets },
        ]
        : [];

    return (
        <Box sx={{ maxWidth: 1100, margin: "24px auto", padding: 2 }}>
            <Typography variant="h5" mb={2}>
                Admin Dashboard
            </Typography>

            {/* KPI cards row */}
            <Stack direction={{ xs: "column", md: "row" }} spacing={2} mb={2}>
                <Card sx={{ flex: 1 }}>
                    <CardContent>
                        <Typography variant="subtitle2">Total Earned</Typography>
                        <Typography variant="h6">{kpis ? kpis.totalEarned : <CircularProgress size={18} />}</Typography>
                    </CardContent>
                </Card>

                <Card sx={{ flex: 1 }}>
                    <CardContent>
                        <Typography variant="subtitle2">Total Burned</Typography>
                        <Typography variant="h6">{kpis ? kpis.totalBurned : <CircularProgress size={18} />}</Typography>
                    </CardContent>
                </Card>

                <Card sx={{ flex: 1 }}>
                    <CardContent>
                        <Typography variant="subtitle2">Active Wallets</Typography>
                        <Typography variant="h6">{kpis ? kpis.activeWallets : <CircularProgress size={18} />}</Typography>
                    </CardContent>
                </Card>
            </Stack>

            {/* Chart */}
            <Card>
                <CardContent sx={{ height: { xs: 260, md: 360 } }}>
                    {loading ? (
                        <Box display="flex" alignItems="center" justifyContent="center" height="100%">
                            <CircularProgress />
                        </Box>
                    ) : (
                        <ResponsiveContainer width="100%" height="100%">
                            <BarChart data={chartData}>
                                <XAxis dataKey="name" />
                                <YAxis />
                                <Tooltip />
                                <Bar dataKey="value" />
                            </BarChart>
                        </ResponsiveContainer>
                    )}
                </CardContent>
            </Card>
        </Box>
    );
}
