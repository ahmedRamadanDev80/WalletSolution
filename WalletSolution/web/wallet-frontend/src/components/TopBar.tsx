import { Link } from "react-router-dom";
import {
    AppBar,
    Toolbar,
    Typography,
    Button,
    Box,
    IconButton,
    Drawer,
    List,
    ListItem,
    ListItemButton,
    Container
} from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import { useState } from "react";

type TopBarProps = { authenticated: boolean; onLogout: () => void; };

export default function TopBar({ authenticated, onLogout }: TopBarProps) {
    const [open, setOpen] = useState(false);
    const nav = [
        { to: "/", label: "WALLET" },
        { to: "/transactions", label: "TRANSACTIONS" },
        { to: "/admin/rules", label: "RULES" },
        { to: "/admin/dashboard", label: "DASHBOARD" },
    ];

    return (
        <>
            <AppBar
                position="fixed"
                elevation={4}
                sx={{
                    background: "linear-gradient(90deg,#1565c0,#1e88e5)",
                }}
            >
                <Container maxWidth="lg">
                    <Toolbar disableGutters sx={{ height: 64 }}>
                        <Typography
                            component={Link}
                            to="/"
                            sx={{ color: "inherit", textDecoration: "none", fontWeight: 600, mr: 2 }}
                        >
                            Wallet Demo
                        </Typography>

                        {/* mobile menu */}
                        <IconButton
                            edge="start"
                            color="inherit"
                            aria-label="menu"
                            sx={{ display: { xs: "inline-flex", md: "none" } }}
                            onClick={() => setOpen(true)}
                        >
                            <MenuIcon />
                        </IconButton>

                        {/* main nav (centered on md+) */}
                        <Box sx={{ display: { xs: "none", md: "flex" }, gap: 2, flexGrow: 1, justifyContent: "center" }}>
                            {nav.map(n => (
                                <Button key={n.to} color="inherit" component={Link} to={n.to}>
                                    {n.label}
                                </Button>
                            ))}
                        </Box>

                        {/* spacer */}
                        <Box sx={{ flexGrow: 1 }} />

                        {/* auth */}
                        <Box>
                            {authenticated ? (
                                <Button color="inherit" onClick={onLogout} sx={{ borderRadius: 2 }}>
                                    LOGOUT
                                </Button>
                            ) : (
                                <Button color="inherit" component={Link} to="/login">
                                    LOGIN
                                </Button>
                            )}
                        </Box>
                    </Toolbar>
                </Container>
            </AppBar>

            {/* drawer for mobile */}
            <Drawer anchor="left" open={open} onClose={() => setOpen(false)}>
                <Box sx={{ width: 240 }} role="presentation" onClick={() => setOpen(false)}>
                    <List>
                        {nav.map(n => (
                            <ListItem key={n.to} disablePadding>
                                <ListItemButton component={Link} to={n.to}>{n.label}</ListItemButton>
                            </ListItem>
                        ))}
                    </List>
                </Box>
            </Drawer>
        </>
    );
}
