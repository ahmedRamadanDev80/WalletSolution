import api from "./axios";

export type ConfigurationRuleDto = {
    id: string;
    serviceId: string;
    ruleType: string;
    pointsPerBaseAmount: number;
    baseAmount: number;
    isDefault: boolean;
    createdAt?: string;
};

export async function getRules() {
    const res = await api.get<ConfigurationRuleDto[]>("/rules");
    return res.data;
}

export async function createRule(payload: Partial<ConfigurationRuleDto>) {
    const res = await api.post<ConfigurationRuleDto>("/rules", payload);
    return res.data;
}

export async function updateRule(id: string, payload: Partial<ConfigurationRuleDto>) {
    await api.put(`/rules/${id}`, payload);
}

export async function deleteRule(id: string) {
    await api.delete(`/rules/${id}`);
}

export async function getKpis() {
    const res = await api.get<{ totalEarned: number; totalBurned: number; activeWallets: number }>("/admin/kpis");
    return res.data;
}
