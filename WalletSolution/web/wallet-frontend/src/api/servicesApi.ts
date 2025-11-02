import api from "./axios";

export type ServiceDto = {
    id: string;
    name: string;
    description?: string | null;
    createdAt?: string;
};

export async function getServices() {
    const res = await api.get<ServiceDto[]>("/services");
    return res.data;
}