// src/components/WalletView.tsx
import { useEffect, useState } from "react";
import { getBalance, earn, burn } from "../api/walletApi";

type WalletDto = { userId: string; balance: number };

export default function WalletView() {
    const [userId, setUserId] = useState("");
    const [wallet, setWallet] = useState<WalletDto | null>(null);
    const [amount, setAmount] = useState<number | "">("");
    const [externalRef, setExternalRef] = useState<string>("");
    const [description, setDescription] = useState<string>("");
    const [loading, setLoading] = useState(false);

    // load wallet when userId changes (or on mount if userId already set)
    useEffect(() => {
        if (!userId) {
            setWallet(null);
            return;
        }
        let mounted = true;
        const load = async () => {
            setLoading(true);
            try {
                const data = await getBalance(userId);
                if (mounted) setWallet(data);
            } catch (err) {
                console.error(err);
                if (mounted) setWallet(null);
            } finally {
                if (mounted) setLoading(false);
            }
        };
        load();
        return () => {
            mounted = false;
        };
    }, [userId]);

    const refresh = async () => {
        if (!userId) return;
        try {
            const d = await getBalance(userId);
            setWallet(d);
        } catch (err) {
            console.error(err);
        }
    };

    const generateExternalRefIfEmpty = () => {
        if (externalRef && externalRef.trim().length > 0) return externalRef.trim();
        // use browser crypto if available
        if (typeof crypto !== "undefined" && typeof (crypto as any).randomUUID === "function") {
            return (crypto as any).randomUUID();
        }
        // fallback
        return `client-${Date.now()}-${Math.floor(Math.random() * 10000)}`;
    };

    const handleEarn = async () => {
        if (!userId) return alert("Enter user GUID");
        if (!amount || Number(amount) <= 0) return alert("Enter amount > 0");

        const ext = generateExternalRefIfEmpty();

        setLoading(true);
        try {
            const res = await earn(userId, Number(amount), ext, description || undefined);
            setWallet({ userId: res.userId, balance: res.balance });
            // optionally clear description or amount
            setAmount("");
            // refresh transactions page if needed via callback or event
        } catch (err: any) {
            console.error(err);
            alert(err?.response?.data?.message ?? err?.message ?? "Earn failed");
        } finally {
            setLoading(false);
        }
    };

    const handleBurn = async () => {
        if (!userId) return alert("Enter user GUID");
        if (!amount || Number(amount) <= 0) return alert("Enter amount > 0");

        const ext = generateExternalRefIfEmpty();

        setLoading(true);
        try {
            const res = await burn(userId, Number(amount), ext, description || undefined);
            setWallet({ userId: res.userId, balance: res.balance });
            setAmount("");
        } catch (err: any) {
            console.error(err);
            alert(err?.response?.data?.message ?? err?.message ?? "Burn failed");
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
        <div style={{ maxWidth: 760, margin: "0 auto", padding: 20 }}>
            <h2>Wallet</h2>

            <div style={{ display: "flex", gap: 8, marginBottom: 12 }}>
                <input
                    placeholder="User GUID"
                    value={userId}
                    onChange={(e) => setUserId(e.target.value)}
                    style={{ flex: 1, padding: 8 }}
                />
                <button onClick={refresh} disabled={!userId || loading}>
                    Refresh
                </button>
            </div>

            <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 8, marginBottom: 12 }}>
                <input
                    placeholder="External reference (idempotency key)"
                    value={externalRef}
                    onChange={(e) => setExternalRef(e.target.value)}
                />
                <input placeholder="Description" value={description} onChange={(e) => setDescription(e.target.value)} />
            </div>

            {loading && <div>Loading...</div>}

            {wallet ? (
                <div style={{ border: "1px solid #ddd", padding: 12, borderRadius: 6 }}>
                    <div style={{ marginBottom: 8 }}>
                        <strong>User:</strong> {wallet.userId}
                    </div>
                    <div style={{ marginBottom: 12 }}>
                        <strong>Balance:</strong> {wallet.balance} pts
                    </div>

                    <div style={{ display: "flex", gap: 8, alignItems: "center" }}>
                        <input
                            type="number"
                            placeholder="Amount"
                            value={amount}
                            onChange={(e) => setAmount(e.target.value === "" ? "" : Number(e.target.value))}
                            style={{ width: 140, padding: 8 }}
                        />
                        <button onClick={handleEarn} disabled={loading}>
                            Earn
                        </button>
                        <button onClick={handleBurn} disabled={loading}>
                            Burn
                        </button>
                    </div>
                </div>
            ) : (
                <div style={{ color: "#666" }}>No wallet loaded</div>
            )}
        </div>
        </>
    );
}
