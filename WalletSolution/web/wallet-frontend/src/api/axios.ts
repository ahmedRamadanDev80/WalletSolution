import axios from "axios";

const api = axios.create({
    baseURL: "https://localhost:7040/api",
    timeout: 15000,
});

api.interceptors.request.use((config) => {
    const token = localStorage.getItem("jwt");
    if (token) {
        config.headers = config.headers ?? {};
        config.headers["Authorization"] = `Bearer ${token}`;
    }
    return config;
});

export default api;
