// Usado em editar perfil do usuário, para adicionar cidades visitadas e remover cidades visitadas do usuário


// Aguarda o carregamento completo do conteúdo da página antes de executar o script
document.addEventListener("DOMContentLoaded", function () {
    // Obtém os elementos HTML necessários
    const cidadeInput = document.getElementById("cidadeInput"); // Campo de entrada de cidade
    const adicionarCidadeBtn = document.getElementById("adicionarCidadeBtn"); // Botão para adicionar cidade
    const cidadesLista = document.getElementById("cidadesLista"); // Lista de cidades já adicionadas
    const contadorCidades = document.getElementById("contadorCidades"); // Elemento para exibir o número total de cidades
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Evento para adicionar uma cidade ao clicar no botão
    adicionarCidadeBtn.addEventListener("click", async function () {
        // Obtém o valor do campo de entrada e remove espaços extras
        const cidade = cidadeInput.value.trim();

        // Verifica se o campo de cidade está vazio
        if (!cidade) {
            alert("Digite uma cidade válida.");
            return;
        }

        try {
            // Faz uma requisição AJAX via POST para adicionar a cidade
            const response = await fetch("/CidadesVisitadas/AddCidade", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ cidade }) // Envia o nome da cidade como corpo da requisição
            });

            // Converte a resposta em JSON
            const data = await response.json();

            // Verifica se a cidade foi adicionada com sucesso
            if (data.success) {
                // Cria um novo item na lista de cidades
                const novaCidade = document.createElement("div");
                novaCidade.id = `cidade-${cidade}`; // Define um ID único para cada cidade
                novaCidade.classList.add("cidade-item", "d-flex", "align-items-center"); // Adiciona classes para estilização
                novaCidade.innerHTML = `
                     <p class="me-2">${cidade}</p>
                            <button type="submit" class="border-0 bg-transparent ms-auto">
                                <i class="bi bi-trash text-dark"></i>
                            </button>
                `;

                // Adiciona o item à lista de cidades
                cidadesLista.appendChild(novaCidade);

                // Limpa o campo de entrada de cidade
                cidadeInput.value = "";

                // Atualiza o contador de cidades, aumentando em 1
                atualizarContador(1);
            } else {
                // Se a operação falhar, exibe uma mensagem de erro
                alert(data.message || "Erro ao adicionar cidade.");
            }
        } catch (error) {
            // Se ocorrer um erro durante a requisição, exibe o erro no console
            console.error("Erro ao adicionar cidade:", error);
        }
    });

    // Evento para remover cidade utilizando event delegation
    document.addEventListener("click", async function (event) {
        // Verifica se o clique foi no botão de remoção da cidade
        if (event.target.closest(".removerCidade")) {
            // Obtém o botão clicado e a cidade associada
            const botao = event.target.closest(".removerCidade");
            const cidade = botao.dataset.cidade;

            // Verifica se a cidade foi encontrada
            if (!cidade) return;

            try {
                // Faz uma requisição AJAX via POST para remover a cidade
                const response = await fetch("/CidadesVisitadas/RemoverCidade", {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({ cidade }) // Envia o nome da cidade como corpo da requisição
                });

                // Converte a resposta em JSON
                const data = await response.json();

                // Verifica se a cidade foi removida com sucesso
                if (data.success) {
                    // Remove o item da lista de cidades
                    document.getElementById(`cidade-${cidade}`).remove();
                    // Atualiza o contador de cidades, diminuindo em 1
                    atualizarContador(-1);
                } else {
                    // Se a operação falhar, exibe uma mensagem de erro
                    alert(data.message || "Erro ao remover cidade.");
                }
            } catch (error) {
                // Se ocorrer um erro durante a requisição, exibe o erro no console
                console.error("Erro ao remover cidade:", error);
            }
        }
    });

    // Função para atualizar o contador de cidades
    function atualizarContador(ajuste) {
        // Obtém o valor atual do contador e faz o ajuste (incrementa ou decrementa)
        let total = parseInt(contadorCidades.innerText) || 0; // Se o contador estiver vazio, assume 0
        total += ajuste; // Aplica o ajuste (positivo ou negativo)
        contadorCidades.innerText = total; // Atualiza o contador na interface

        // Se não houver cidades (total = 0), exibe a mensagem "Nenhuma cidade encontrada"
        if (total === 0) {
            cidadesLista.innerHTML = '<p id="nenhumaCidade">Nenhuma cidade encontrada</p>';
        }
    }
});
