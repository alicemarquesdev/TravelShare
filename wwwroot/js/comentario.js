// Classe responsável por manipular os comentários, incluindo envio e remoção
class ComentarioHandler {
    constructor(formSelector, comentariosListaSelector) {
        // Recebe os seletores para o formulário de envio de comentário e a lista de comentários
        this.formSelector = formSelector;
        this.comentariosListaSelector = comentariosListaSelector;
        // Inicializa o manipulador, configurando os eventos
        this.initialize();
    }

    // Inicializa o manipulador de comentários
    initialize() {
        this.adicionarEventoComentario();  // Adiciona o evento de submit aos formulários de comentário
    }

    // Adiciona o evento de submit nos formulários de comentários
    adicionarEventoComentario() {
        // Seleciona todos os formulários de comentários com o seletor fornecido
        document.querySelectorAll(this.formSelector).forEach(form => {
            // Para cada formulário, adiciona o evento de submit
            form.addEventListener('submit', (event) => this.enviarComentario(event, form));
        });
    }

    // Envia o comentário via AJAX para o servidor
    async enviarComentario(event, form) {
        event.preventDefault();  // Impede o envio padrão do formulário

        // Obtém os dados do postId e do comentário a partir do formulário
        const postId = form.querySelector('input[name="postId"]').value;
        const comentario = form.querySelector('input[name="comentario"]').value;

        // Valida os dados antes de enviar
        if (!comentario || comentario.length > 100) {
            alert('Comentário inválido! O comentário deve ter no máximo 100 caracteres.');
            return;
        }

        // Envia os dados via fetch (AJAX)
        try {
            const response = await fetch('/Comentario/AddComentario', {
                method: 'POST',  // Método POST para enviar os dados
                headers: {
                    'Content-Type': 'application/json',  // Cabeçalho indicando que o corpo da requisição está em JSON
                },
                body: JSON.stringify({ postId: postId, comentario: comentario })  // Envia os dados como um JSON
            });

            // Converte a resposta para JSON
            const data = await response.json();

            // Se a resposta for bem-sucedida
            if (data.success) {
                // Atualiza a lista de comentários na página
                this.atualizarComentarios(data.comentariosHTML, postId);
                // Limpa o campo de comentário após envio
                form.querySelector('input[name="comentario"]').value = '';
            } else {
                // Caso contrário, exibe uma mensagem de erro
                alert('Erro ao adicionar comentário: ' + data.message);
            }
        } catch (error) {
            // Se ocorrer um erro de conexão, exibe a mensagem no console e um alerta
            console.error('Erro ao adicionar comentário', error);
            alert('Erro de conexão. Tente novamente mais tarde.');
        }
    }

    // Atualiza a lista de comentários na página após adicionar um novo comentário
    atualizarComentarios(comentariosHTML, postId) {
        // Encontra a lista de comentários específica para o postId
        const comentariosLista = document.querySelector(`.comentariosLista[data-post-id="${postId}"]`);
        // Se a lista de comentários for encontrada, atualiza o HTML dela
        if (comentariosLista) {
            comentariosLista.innerHTML = comentariosHTML;
        }
    }
}

// Inicializa o manipulador de comentários para todos os formulários de comentário e listas de comentários
document.addEventListener('DOMContentLoaded', () => {
    new ComentarioHandler('.addComentarioForm', '.comentariosLista');
});

// Seleciona todos os formulários para deletar comentários
document.querySelectorAll('.deleteComentarioForm').forEach(form => {
    // Adiciona o evento de submit para deletar o comentário
    form.addEventListener('submit', async (event) => {
        event.preventDefault();  // Impede o envio padrão do formulário

        // Obtém o ID do comentário a ser deletado
        const comentarioId = form.querySelector('input[name="id"]').value;

        // Verifica se o ID do comentário foi encontrado
        if (!comentarioId) {
            alert("Erro: ID do comentário não encontrado!");
            return;
        }

        console.log(`Enviando ID: ${comentarioId}`);  // Exibe o ID no console para depuração

        try {
            // Envia o ID do comentário para o servidor para excluí-lo
            const response = await fetch(window.location.origin + "/Comentario/DeletarComentario", {
                method: 'POST',  // Método POST para deletar o comentário
                headers: {
                    'Content-Type': 'application/json',  // Cabeçalho indicando que o corpo da requisição está em JSON
                },
                body: JSON.stringify({ id: comentarioId })  // Envia o ID do comentário como um JSON
            });

            // Converte a resposta para JSON
            const data = await response.json();

            // Se a exclusão for bem-sucedida, remove o comentário da página
            if (data.success) {
                document.getElementById(`comentario-${comentarioId}`).remove();
            } else {
                // Caso contrário, exibe uma mensagem de erro
                alert('Erro ao excluir comentário');
            }
        } catch (error) {
            // Se ocorrer um erro de conexão, exibe a mensagem no console e um alerta
            console.error('Erro ao excluir comentário', error);
            alert('Erro de conexão');
        }
    });
});