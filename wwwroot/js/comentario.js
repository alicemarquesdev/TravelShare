class ComentarioHandler {
    constructor(formSelector, comentariosListaSelector) {
        this.formSelector = formSelector;
        this.comentariosListaSelector = comentariosListaSelector;
        this.initialize();
    }

    // Inicializa o manipulador de comentários
    initialize() {
        this.adicionarEventoComentario();
    }

    // Adiciona o evento de submit nos formulários de comentários
    adicionarEventoComentario() {
        document.querySelectorAll(this.formSelector).forEach(form => {
            form.addEventListener('submit', (event) => this.enviarComentario(event, form));
        });
    }

    // Envia o comentário via AJAX
    async enviarComentario(event, form) {
        event.preventDefault();  // Impede o envio padrão do formulário

        const postId = form.querySelector('input[name="postId"]').value;
        const comentario = form.querySelector('input[name="comentario"]').value;

        // Validar os dados antes de enviar
        if (!comentario || comentario.length > 100) {
            alert('Comentário inválido! O comentário deve ter no máximo 100 caracteres.');
            return;
        }

        // Enviar os dados via fetch (AJAX)
        try {
            const response = await fetch('/Comentario/AddComentario', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ postId: postId, comentario: comentario })
            });

            const data = await response.json();

            if (data.success) {
                // Atualizar a lista de comentários na página
                this.atualizarComentarios(data.comentariosHTML, postId);
                // Limpar o campo de comentário após envio
                form.querySelector('input[name="comentario"]').value = '';
            } else {
                alert('Erro ao adicionar comentário: ' + data.message);
            }
        } catch (error) {
            console.error('Erro ao adicionar comentário', error);
            alert('Erro de conexão. Tente novamente mais tarde.');
        }
    }

    // Atualiza a lista de comentários na página
    atualizarComentarios(comentariosHTML, postId) {
        const comentariosLista = document.querySelector(`.comentariosLista[data-post-id="${postId}"]`);
        if (comentariosLista) {
            comentariosLista.innerHTML = comentariosHTML;
        }
    }

}

// Inicializar o manipulador para todos os formulários de comentário e listas de comentários
document.addEventListener('DOMContentLoaded', () => {
    new ComentarioHandler('.addComentarioForm', '.comentariosLista');
});



document.querySelectorAll('.deleteComentarioForm').forEach(form => {
    form.addEventListener('submit', async (event) => {
        event.preventDefault();  // Impede o envio padrão do formulário

        const comentarioId = form.querySelector('input[name="id"]').value;

        if (!comentarioId) {
            alert("Erro: ID do comentário não encontrado!");
            return;
        }

        console.log(`Enviando ID: ${comentarioId}`);

        try {
            const response = await fetch('/Comentario/DeletarComentario', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ id: comentarioId })
            });

            const data = await response.json();

            if (data.success) {
                document.getElementById(`comentario-${comentarioId}`).remove();
            } else {
                alert('Erro ao excluir comentário');
            }
        } catch (error) {
            console.error('Erro ao excluir comentário', error);
            alert('Erro de conexão');
        }
    });
});
