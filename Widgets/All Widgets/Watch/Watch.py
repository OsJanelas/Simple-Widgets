import tkinter as tk
import time

class CronometroWidget:
    def __init__(self):
        self.root = tk.Tk()
        self.root.title("Watch")
        
        # SETTINGS (made with Gemini)
        self.root.overrideredirect(True)
        self.root.attributes("-topmost", True)
        self.root.geometry("250x120+100+100")
        self.root.configure(bg="#1a1a1a")

        self.tempo_inicial = 0
        self.tempo_decorrido = 0
        self.rodando = False
        self.texto_tempo = tk.StringVar(value="00:00:00.0")

        self.criar_interface()

        # Eventos para arrastar
        self.root.bind("<Button-1>", self.iniciar_arrasto)
        self.root.bind("<B1-Motion>", self.movimentar_janela)
        self.root.bind("<Button-3>", lambda e: self.root.destroy())

        self.root.mainloop()

    def criar_interface(self):
        # LABEL
        label_tempo = tk.Label(
            self.root, textvariable=self.texto_tempo,
            font=("Arial Black", 28, "bold"),
            bg="#1a1a1a", fg="#00ffcc"
        )
        label_tempo.pack(pady=10)

        frame_botoes = tk.Frame(self.root, bg="#1a1a1a")
        frame_botoes.pack(pady=5)

        self.btn_start = tk.Button(frame_botoes, text="Start", command=self.alternar,
                                  bg="#333", fg="white", font=("Segoe UI", 8, "bold"),
                                  relief="flat", width=8)
        self.btn_start.pack(side="left", padx=5)

        btn_reset = tk.Button(frame_botoes, text="Reset", command=self.resetar,
                             bg="#333", fg="white", font=("Segoe UI", 8, "bold"),
                             relief="flat", width=8)
        btn_reset.pack(side="left", padx=5)

    def alternar(self):
        if not self.rodando:
            self.rodando = True
            self.tempo_inicial = time.time() - self.tempo_decorrido
            self.btn_start.config(text="Pause", fg="#ff6666")
            self.atualizar()
        else:
            self.rodando = False
            self.btn_start.config(text="Return", fg="#66ff66")

    def resetar(self):
        self.rodando = False
        self.tempo_decorrido = 0
        self.texto_tempo.set("00:00:00.0")
        self.btn_start.config(text="Start", fg="white")

    def atualizar(self):
        if self.rodando:
            self.tempo_decorrido = time.time() - self.tempo_inicial
            
            # Cálculo de H:M:S.ms
            m, s = divmod(self.tempo_decorrido, 60)
            h, m = divmod(m, 60)
            ms = int((self.tempo_decorrido % 1) * 10)
            
            self.texto_tempo.set(f"{int(h):02}:{int(m):02}:{int(s):02}.{ms}")
            
            # Agenda a próxima atualização em 100ms
            self.root.after(100, self.atualizar)

    def iniciar_arrasto(self, event):
        self.x = event.x
        self.y = event.y

    def movimentar_janela(self, event):
        deltax = event.x - self.x
        deltay = event.y - self.y
        x = self.root.winfo_x() + deltax
        y = self.root.winfo_y() + deltay
        self.root.geometry(f"+{x}+{y}")

if __name__ == "__main__":
    CronometroWidget()
