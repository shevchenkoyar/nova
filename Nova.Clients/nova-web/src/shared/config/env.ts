export const env = {
    novaApiUrl: import.meta.env.VITE_NOVA_API_URL as string,
    userId: import.meta.env.VITE_NOVA_USER_ID as string,
};

if (!env.novaApiUrl) {
    throw new Error("VITE_NOVA_API_URL is not configured");
}

if (!env.userId) {
    throw new Error("VITE_NOVA_USER_ID is not configured");
}