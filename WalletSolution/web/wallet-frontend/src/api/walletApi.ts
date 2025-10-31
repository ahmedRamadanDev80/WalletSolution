// src/api/walletApi.ts
import axios from "axios";

const api = axios.create({
    baseURL: "https://localhost:7040/api",
    timeout: 15000,
});

export type WalletDto = { userId: string; balance: number; };

export async function getBalance(userId: string) {
    const res = await api.get<WalletDto>(`/wallets/${userId}/balance`);
    return res.data;
}

export async function earn(userId: string, amount: number, externalReference?: string, description?: string) {
    const res = await api.post<{ userId: string; balance: number }>(`/wallets/${userId}/earn`, {
        amount,
        externalReference,
        description,
    });
    return res.data;
}

export async function burn(userId: string, amount: number, externalReference?: string, description?: string) {
    const res = await api.post<{ userId: string; balance: number }>(`/wallets/${userId}/burn`, {
        amount,
        externalReference,
        description,
    });
    return res.data;
}

export type WalletTransactionDto = {
    id: string;
    type: string;
    amount: number;
    balanceAfter: number;
    description?: string | null;
    externalReference?: string | null;
    createdAt: string;
};

export async function getTransactions(userId: string, skip = 0, take = 20) {
    const res = await api.get<{ items: WalletTransactionDto[]; total: number }>(
        `/wallets/${userId}/transactions?skip=${skip}&take=${take}`
    );
    return res.data;
}
