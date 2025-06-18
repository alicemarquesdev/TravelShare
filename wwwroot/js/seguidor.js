// Função para gerenciar o envio do formulário de seguir/desseguir
document.querySelectorAll('.seguirOuDeseguirForm').forEach(form => {
    // Escuta o evento de envio do formulário
    form.addEventListener('submit', async (event) => {
        event.preventDefault();  // Impede o envio do formulário padrão

        const seguindoId = form.dataset.seguindoId; // Pega o ID do usuário que está sendo seguido
        const usuarioId = form.dataset.usuarioId;   // Pega o ID do usuário que está realizando a ação
        const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

        // Enviar os dados via fetch (AJAX)
        try {
            const response = await fetch('/Seguidor/SeguirOuDeseguir', {
                method: 'POST',  // Usando o método POST para enviar os dados
                headers: {
                    'Content-Type': 'application/json',  // Definindo o tipo de conteúdo como JSON
                },
                body: JSON.stringify({ seguindoId: seguindoId, usuarioId: usuarioId })  // Convertendo os dados para JSON
            });

            const data = await response.json();  // Espera pela resposta da API e converte para JSON

            if (data.success) {  // Verifica se a resposta foi bem-sucedida
                const button = form.querySelector('button');  // Pega o botão do formulário

                // Verifica a classe atual do botão e a altera
                if (button.classList.contains('btn-dark')) {
                    button.textContent = 'Seguindo';  // Muda o texto do botão
                    button.classList.remove('btn-dark');  // Remove a classe btn-dark
                    button.classList.add('btn-outline-dark');  // Adiciona a classe btn-outline-dark
                }
                else if (button.classList.contains('btn-outline-dark')) {
                    button.textContent = 'Seguir';  // Muda o texto do botão
                    button.classList.remove('btn-outline-dark');  // Remove a classe btn-outline-dark
                    button.classList.add('btn-dark');  // Adiciona a classe btn-dark
                }
            } else {
                alert('Erro ao executar ação');  // Exibe um erro caso a operação não tenha sido bem-sucedida
            }
        } catch (error) {
            console.error('Erro ao executar ação de seguir/desseguir', error);  // Exibe erro no console
            alert('Erro de conexão');  // Exibe uma mensagem de erro ao usuário
        }
    });
});

async function removerSeguidor(button) {
    const seguidorId = button.getAttribute('data-seguidor-id');

    // Certificar-se de que o ID do seguidor está presente
    if (!seguidorId) {
        alert("ID do seguidor não encontrado!");
        return;
    }

    try {
        // Fazer a requisição usando fetch
        const response = await fetch('/SeuController/RemoverSeguidor', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() // Adicionando o token de segurança
            },
            body: JSON.stringify({ seguidorId: seguidorId })
        });

        const data = await response.json();

        if (data.success) {
            // Se o seguidor for removido com sucesso, ocultar o botão
            button.style.display = 'none'; // Ocultar o botão
        } else {
            alert(data.message || "Erro ao remover seguidor.");
        }
    } catch (error) {
        console.error("Erro ao remover seguidor:", error);
        alert("Ocorreu um erro ao remover o seguidor.");
    }
}
