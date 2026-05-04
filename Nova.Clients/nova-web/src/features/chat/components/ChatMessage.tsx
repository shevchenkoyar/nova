import type { ChatMessage as ChatMessageType } from "../types";

type Props = {
    message: ChatMessageType;
};

export function ChatMessage({ message }: Props) {
    const isUser = message.role === "user";

    return (
        <article
            className={[
                "max-w-[78%] rounded-2xl px-4 py-3 text-sm leading-6 shadow-sm",
                "whitespace-pre-wrap",
                isUser
                    ? "ml-auto bg-blue-600 text-white"
                    : "mr-auto bg-slate-800 text-slate-100",
            ].join(" ")}
        >
            <div className="mb-1 text-xs font-bold opacity-70">
                {isUser ? "Ты" : "Nova"}
            </div>

            <div>{message.text}</div>

            {message.data !== undefined && (
                <details className="mt-3 border-t border-white/15 pt-2">
                    <summary className="cursor-pointer text-xs opacity-75">
                        Debug data
                    </summary>

                    <pre className="mt-2 max-h-80 overflow-auto rounded-lg bg-black/30 p-3 text-xs">
            {JSON.stringify(message.data, null, 2)}
          </pre>
                </details>
            )}
        </article>
    );
}