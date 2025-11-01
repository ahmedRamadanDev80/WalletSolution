import { Link } from "react-router-dom";
import { AppBar, Toolbar, Typography, Button, Box } from "@mui/material";

type TopBarProps = {
    authenticated: boolean;
    onLogout: () => void;
};

export default function TopBar({ authenticated, onLogout }: TopBarProps) {
    return (
        <AppBar position="static">
            <Toolbar>
                <Typography variant="h6" sx={{ flexGrow: 1 }}>
                    Wallet Demo
                </Typography>

                {/* navigation links */}
                <Box sx={{ display: "flex", gap: 1, alignItems: "center" }}>
                    <Button color="inherit" component={Link} to="/">
                        Wallet
                    </Button>
                    <Button color="inherit" component={Link} to="/transactions">
                        Transactions
                    </Button>
                </Box>

                {/* spacer */}
                <Box sx={{ flexGrow: 1 }} />

                {/* auth button */}
                {authenticated ? (
                    <Button color="inherit" onClick={onLogout}>
                        Logout
                    </Button>
                ) : (
                    <Button color="inherit" component={Link} to="/login">
                        Login
                    </Button>
                )}
            </Toolbar>
        </AppBar>
    );
}
