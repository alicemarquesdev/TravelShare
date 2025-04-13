//  responsável por manipular as curtidas em post


document.addEventListener("DOMContentLoaded", function () {
    // Seleciona todos os botões de curtida na página
    const botoesCurtida = document.querySelectorAll(".curtida-btn");
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    // Adiciona o evento de clique a cada botão de curtida
    botoesCurtida.forEach((button) => {
        button.addEventListener("click", async function () {
            // Obtém o postId do atributo data-post-id do botão
            const postId = button.dataset.postId;

            try {
                // Envia uma requisição AJAX (POST) para a rota /Curtida/CurtirOuDescurtir
                const response = await fetch('/Curtida/CurtirOuDescurtir', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    // Envia o postId no corpo da requisição como JSON
                    body: JSON.stringify({ postId: postId })
                });

                // Converte a resposta da requisição para JSON
                const result = await response.json();

                // Verifica se a ação foi bem-sucedida
                if (result.success) {
                    const icon = button.querySelector("i");  // Seleciona o ícone dentro do botão

                    // Se a curtida foi ativada, muda o estilo do botão
                    if (result.curtidaAtiva) {
                        button.classList.add("text-danger");  // Altera a cor do botão para vermelha
                        button.classList.remove("text-secondary");  // Remove a cor secundária
                        icon.classList.add("bi-heart-fill");  // Muda o ícone para o coração preenchido
                        icon.classList.remove("bi-heart");  // Remove o ícone de coração vazio
                    } else {
                        // Se a curtida foi desativada, altera o estilo do botão para o estado original
                        button.classList.add("text-secondary");  // Muda a cor para a secundária
                        button.classList.remove("text-danger");  // Remove a cor vermelha
                        icon.classList.add("bi-heart");  // Muda o ícone para o coração vazio
                        icon.classList.remove("bi-heart-fill");  // Remove o ícone de coração preenchido
                    }
                } else {
                    // Se a requisição falhou, exibe um alerta
                    alert('Não foi possível realizar a ação.');
                }
            } catch (error) {
                // Se ocorrer algum erro na requisição, exibe no console e alerta o usuário
                console.error('Erro ao tentar curtir/descurtir:', error);
            }
        });
    });
});
