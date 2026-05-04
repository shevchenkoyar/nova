import { env } from "../config/env";

export async function novaFetch<TResponse>(
    path: string,
    options: RequestInit
): Promise<TResponse> {
    const response = await fetch(`${env.novaApiUrl}${path}`, {
        ...options,
        headers: {
            "Content-Type": "application/json",
            ...(options.headers ?? {}),
        },
    });

    if (!response.ok) {
        const text = await response.text();

        throw new Error(
            text || `Nova API request failed with status ${response.status}`
        );
    }

    return await response.json() as Promise<TResponse>;
}