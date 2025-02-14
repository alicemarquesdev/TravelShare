document.addEventListener("DOMContentLoaded", function () {
    const cidadeInput = document.getElementById("cidadeInput");
    const adicionarCidadeBtn = document.getElementById("adicionarCidadeBtn");
    const cidadesLista = document.getElementById("cidadesLista");
    const contadorCidades = document.getElementById("contadorCidades");

    // Adicionar cidade via AJAX
    adicionarCidadeBtn.addEventListener("click", async function () {
        const cidade = cidadeInput.value.trim();

        if (!cidade) {
            alert("Digite uma cidade válida.");
            return;
        }

        try {
            const response = await fetch("/CidadesVisitadas/AddCidade", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ cidade })
            });

            const data = await response.json();

            if (data.success) {
                const novaCidade = document.createElement("div");
                novaCidade.id = `cidade-${cidade}`;
                novaCidade.classList.add("cidade-item", "d-flex", "align-items-center");
                novaCidade.innerHTML = `
                     <p class="me-2">${cidade}</p>
                            <button type="submit" class="border-0 bg-transparent ms-auto">
                                <i class="bi bi-trash text-dark"></i>
                            </button>
                `;

                cidadesLista.appendChild(novaCidade);
                cidadeInput.value = "";

                atualizarContador(1);
            } else {
                alert(data.message || "Erro ao adicionar cidade.");
            }
        } catch (error) {
            console.error("Erro ao adicionar cidade:", error);
        }
    });

    // Remover cidade usando event delegation
    document.addEventListener("click", async function (event) {
        if (event.target.closest(".removerCidade")) {
            const botao = event.target.closest(".removerCidade");
            const cidade = botao.dataset.cidade;

            if (!cidade) return;

            try {
                const response = await fetch("/CidadesVisitadas/RemoverCidade", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ cidade })
                });

                const data = await response.json();

                if (data.success) {
                    document.getElementById(`cidade-${cidade}`).remove();
                    atualizarContador(-1);
                } else {
                    alert(data.message || "Erro ao remover cidade.");
                }
            } catch (error) {
                console.error("Erro ao remover cidade:", error);
            }
        }
    });

    // Função para atualizar contador de cidades
    function atualizarContador(ajuste) {
        let total = parseInt(contadorCidades.innerText) || 0;
        total += ajuste;
        contadorCidades.innerText = total;

        // Se não houver cidades, exibir mensagem "Nenhuma cidade encontrada"
        if (total === 0) {
            cidadesLista.innerHTML = '<p id="nenhumaCidade">Nenhuma cidade encontrada</p>';
        }
    }
});
