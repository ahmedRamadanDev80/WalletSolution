// src/components/TransactionsView.tsx
import { useState } from "react";
import { getTransactions, type WalletTransactionDto } from "../api/walletApi";

export default function TransactionsView() {
    const [userId, setUserId] = useState("");
    const [transactions, setTransactions] = useState<WalletTransactionDto[]>([]);
    const [loading, setLoading] = useState(false);

    async function load() {
        if (!userId) return alert("Enter user GUID");
        setLoading(true);
        try {
            const res = await getTransactions(userId, 0, 50);
            setTransactions(res.items);
        } catch (err: any) {
            alert(err?.response?.data?.message ?? "Failed to load transactions");
            setTransactions([]);
        } finally {
            setLoading(false);
        }
    }

    return (
        <div style={{ maxWidth: 900, margin: "0 auto", padding: 20 }}>
            <div style={{ display: "flex", gap: 8, marginBottom: 12 }}>
                <input
                    placeholder="User GUID"
                    value={userId}
                    onChange={(e) => setUserId(e.target.value)}
                    style={{ flex: 1, padding: 8 }}
                />
                <button onClick={load} disabled={loading}>
                    Load Transactions
                </button>
            </div>

            <table style={{ width: "100%", borderCollapse: "collapse" }}>
                <thead>
                    <tr style={{ textAlign: "left", borderBottom: "1px solid #ddd" }}>
                        <th style={{ padding: 8 }}>Type</th>
                        <th style={{ padding: 8 }}>Amount</th>
                        <th style={{ padding: 8 }}>Balance After</th>
                        <th style={{ padding: 8 }}>Description</th>
                        <th style={{ padding: 8 }}>When</th>
                    </tr>
                </thead>
                <tbody>
                    {transactions.map((t) => (
                        <tr key={t.id} style={{ borderBottom: "1px solid #f2f2f2" }}>
                            <td style={{ padding: 8 }}>{t.type}</td>
                            <td style={{ padding: 8 }}>{t.amount}</td>
                            <td style={{ padding: 8 }}>{t.balanceAfter}</td>
                            <td style={{ padding: 8 }}>{t.description ?? ""}</td>
                            <td style={{ padding: 8 }}>{new Date(t.createdAt).toLocaleString()}</td>
                        </tr>
                    ))}
                    {transactions.length === 0 && (
                        <tr>
                            <td colSpan={5} style={{ padding: 12, color: "#666" }}>
                                No transactions
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    );
}
