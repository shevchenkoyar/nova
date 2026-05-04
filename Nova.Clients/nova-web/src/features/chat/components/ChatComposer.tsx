import { useState } from "react";

type Props = {
    disabled?: boolean;
    onSend: (text: string) => void;
};

export function ChatComposer({ disabled, onSend }: Props) {
    const [text, setText] = useState("");

    const submit = () => {
        const value = text.trim();

        if (!value || disabled) {
            return;
        }

        onSend(value);
        setText("");
    };

    return (
        <div className="flex gap-3">
      <textarea
          value={text}
          disabled={disabled}
          placeholder="Напиши сообщение Nova..."
          rows={3}
          className="min-h-24 flex-1 resize-none rounded-2xl border border-slate-700 bg-slate-950 px-4 py-3 text-sm text-slate-100 outline-none transition placeholder:text-slate-500 focus:border-blue-400 disabled:cursor-not-allowed disabled:opacity-60"
          onChange={(e) => setText(e.target.value)}
          onKeyDown={(e) => {
              if (e.key === "Enter" && !e.shiftKey) {
                  e.preventDefault();
                  submit();
              }
          }}
      />

            <button
                disabled={disabled || !text.trim()}
                onClick={submit}
                className="rounded-2xl bg-emerald-500 px-5 font-bold text-emerald-950 transition hover:bg-emerald-400 disabled:cursor-not-allowed disabled:opacity-60"
            >
                {disabled ? "..." : "Отправить"}
            </button>
        </div>
    );
}