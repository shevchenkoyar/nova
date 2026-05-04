import { novaFetch } from "../../../shared/api/novaApi";
import { env } from "../../../shared/config/env";
import type {
    AssistantMessageRequest,
    AssistantMessageResponse,
} from "../types";

export function sendAssistantMessage(
    request: AssistantMessageRequest
): Promise<AssistantMessageResponse> {
    return novaFetch<AssistantMessageResponse>("/api/messages", {
        method: "POST",
        body: JSON.stringify(request),
    });
}

export type HistoryItemDto = {
    role: "User" | "Assistant";
    message: string;
    data?: unknown;
    createdAt: string;
};

export function loadHistory(): Promise<HistoryItemDto[]> {
    return novaFetch<HistoryItemDto[]>(
        `/api/messages/history/${env.userId}`,
        {
            method: "GET",
        }
    );
}