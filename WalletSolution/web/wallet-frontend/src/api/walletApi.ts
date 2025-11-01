import api from "../api/axios";

export type WalletDto = { userId: string; balance: number };

export type WalletTransactionDto = {
    id: string;
    type: string;
    amount: number;
    balanceAfter: number;
    description?: string | null;
    externalReference?: string | null;
    createdAt: string;
};

export type TransactionsPaged = { items: WalletTransactionDto[]; total: number };

/**
 * Helper to generate an external reference for idempotency.
 * Uses crypto.randomUUID() when available, otherwise falls back to a timestamp-based id.
 */
export function generateExternalRef(): string {
    // modern browsers support crypto.randomUUID()
    // fallback to timestamp + random number
    try {
        // @ts-ignore - crypto may be available in the environment
        if (typeof crypto !== "undefined" && typeof (crypto as any).randomUUID === "function") {
            // eslint-disable-next-line @typescript-eslint/no-unsafe-return
            return `EXT-${(crypto as any).randomUUID()}`;
        }
    } catch {
        // ignore and fallback
    }
    return `EXT-${Date.now()}-${Math.floor(Math.random() * 100000)}`;
}

/**
 * Get the authenticated user's wallet balance.
 * (Backend: GET /api/wallets/balance)
 */
export async function getBalance(): Promise<WalletDto> {
    const res = await api.get<WalletDto>("/wallets/balance");
    return res.data;
}

/**
 * Earn points for the authenticated user.
 * - amount: money amount (backend will convert to points using rules)
 * - serviceId: optional GUID of the service used
 * - externalReference / description: optional
 *
 * (Backend: POST /api/wallets/earn)
 */
export async function earn(
    amount: number,
    serviceId?: string | null,
    externalReference?: string | null,
    description?: string | null
): Promise<WalletDto> {
    const payload: Record<string, unknown> = {
        amount,
    };

    if (serviceId) payload.serviceId = serviceId;
    if (externalReference) payload.externalReference = externalReference;
    if (description) payload.description = description;

    const res = await api.post<WalletDto>("/wallets/earn", payload);
    return res.data;
}

/**
 * Burn points for the authenticated user.
 * - amount: points (or money if backend treats 1 point = 1 SAR)
 * - externalReference / description: optional
 *
 * (Backend: POST /api/wallets/burn)
 */
export async function burn(
    amount: number,
    externalReference?: string | null,
    description?: string | null
): Promise<WalletDto> {
    const payload: Record<string, unknown> = { amount };
    if (externalReference) payload.externalReference = externalReference;
    if (description) payload.description = description;

    const res = await api.post<WalletDto>("/wallets/burn", payload);
    return res.data;
}

/**
 * Get transactions for the authenticated user (paged).
 * (Backend: GET /api/wallets/transactions?skip=0&take=20)
 */
export async function getTransactions(skip = 0, take = 20): Promise<TransactionsPaged> {
    const res = await api.get<TransactionsPaged>(`/wallets/transactions?skip=${skip}&take=${take}`);
    return res.data;
}
