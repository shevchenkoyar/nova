import { useEffect, useRef } from "react";
import { ChatComposer } from "./ChatComposer";
import { ChatMessage } from "./ChatMessage";
import { useChat } from "../hooks/useChat";

export function ChatWindow() {
    const {
        messages,
        isSending,
        isLoadingHistory,
        error,
        send,
        clear,
    } = useChat();

    const bottomRef = useRef<HTMLDivElement | null>(null);

    useEffect(() => {
        bottomRef.current?.scrollIntoView({
            behavior: "smooth",
            block: "end",
        });
    }, [messages]);

    return (
        <main className="mx-auto flex h-screen max-w-5xl flex-col gap-4 bg-slate-900 p-6 text-slate-100">
            <header className="flex items-center justify-between">
                <div>
                    <h1 className="text-3xl font-bold">Nova</h1>
                    <p className="text-sm text-slate-400">
                        Personal AI assistant
                    </p>
                </div>

                <button
                    onClick={clear}
                    className="rounded-xl border border-slate-700 bg-slate-800 px-4 py-2 text-sm"
                >
                    Очистить
                </button>
            </header>

            {error && (
                <div className="rounded-xl bg-red-900 p-3 text-sm">
                    {error}
                </div>
            )}

            <section className="flex-1 overflow-y-auto rounded-3xl border border-slate-800 bg-slate-950 p-5">
                {isLoadingHistory ? (
                    <div className="grid h-full place-items-center text-slate-500">
                        Загрузка истории...
                    </div>
                ) : messages.length === 0 ? (
                    <div className="grid h-full place-items-center text-slate-500">
                        Напиши первое сообщение
                    </div>
                ) : (
                    <div className="flex flex-col gap-4">
                        {messages.map((m) => (
                            <ChatMessage key={m.id} message={m} />
                        ))}
                        <div ref={bottomRef} />
                    </div>
                )}
            </section>

            <ChatComposer disabled={isSending} onSend={send} />
        </main>
    );
}