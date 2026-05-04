export type ChatMessageRole = "user" | "assistant";

export type ChatMessage = {
    id: string;
    role: ChatMessageRole;
    text: string;
    createdAt: string;
    data?: unknown;
};

export type AssistantMessageRequest = {
    userId: string;
    text: string;
};

export type AssistantMessageResponse = {
    message: string;
    data?: unknown;
};