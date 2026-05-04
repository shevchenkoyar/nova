import { useCallback, useEffect, useState } from "react";
import { env } from "../../../shared/config/env";
import { sendAssistantMessage, loadHistory } from "../api/chatApi";
import type { ChatMessage } from "../types";

function createMessage(
    role: ChatMessage["role"],
    text: string,
    data?: unknown,
    createdAt?: string
): ChatMessage {
    return {
        id: crypto.randomUUID(),
        role,
        text,
        data,
        createdAt: createdAt ?? new Date().toISOString(),
    };
}

function mapHistory(items: any[]): ChatMessage[] {
    return items
        .filter((x) => x.role === "User" || x.role === "Assistant")
        .map((x) =>
            createMessage(
                x.role === "User" ? "user" : "assistant",
                x.content,
                parseMetadata(x.metadataJson),
                x.createdAt
            )
        );
}

function parseMetadata(json?: string | null) {
    if (!json) return null;

    try {
        return JSON.parse(json);
    } catch {
        return null;
    }
}

export function useChat() {
    const [messages, setMessages] = useState<ChatMessage[]>([]);
    const [isSending, setIsSending] = useState(false);
    const [isLoadingHistory, setIsLoadingHistory] = useState(true);
    const [error, setError] = useState<string | null>(null);

    // 🔥 загрузка истории
    useEffect(() => {
        const load = async () => {
            try {
                const history = await loadHistory();

                const mapped = mapHistory(history);

                setMessages(mapped);
            } catch (e) {
                console.warn("Failed to load history", e);
            } finally {
                setIsLoadingHistory(false);
            }
        };

        load();
    }, []);

    const send = useCallback(
        async (text: string) => {
            const normalizedText = text.trim();

            if (!normalizedText || isSending) {
                return;
            }

            setError(null);

            const userMessage = createMessage("user", normalizedText);

            setMessages((prev) => [...prev, userMessage]);
            setIsSending(true);

            try {
                const response = await sendAssistantMessage({
                    userId: env.userId,
                    text: normalizedText,
                });

                const assistantMessage = createMessage(
                    "assistant",
                    response.message,
                    response.data
                );

                setMessages((prev) => [...prev, assistantMessage]);
            } catch (e) {
                const message =
                    e instanceof Error ? e.message : "Unknown Nova API error";

                setError(message);

                setMessages((prev) => [
                    ...prev,
                    createMessage("assistant", "Ошибка запроса к Nova", {
                        error: message,
                    }),
                ]);
            } finally {
                setIsSending(false);
            }
        },
        [isSending]
    );

    const clear = useCallback(() => {
        setMessages([]);
        setError(null);
    }, []);

    return {
        messages,
        isSending,
        isLoadingHistory,
        error,
        send,
        clear,
    };
}