document.querySelectorAll('.seguirOuDeseguirForm').forEach(form => {
    form.addEventListener('submit', async (event) => {
        event.preventDefault();  // Impede o envio do formulário

        const seguindoId = form.dataset.seguindoId;
        const usuarioId = form.dataset.usuarioId;

        // Enviar os dados via fetch (AJAX)
        try {
            const response = await fetch('/Seguidor/SeguirOuDeseguir', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ seguindoId: seguindoId, usuarioId: usuarioId })
            });

            const data = await response.json();

            if (data.success) {
                // Aqui você pode mudar o botão dinamicamente após a ação
                const button = form.querySelector('button');

                // Verifique a classe atual e atualize o botão
                if (button.classList.contains('btn-dark')) {
                    button.textContent = 'Seguindo';
                    button.classList.remove('btn-dark');
                    button.classList.add('btn-outline-dark');
                }
                else if (button.classList.contains('btn-outline-dark')) {
                    button.textContent = 'Seguir';
                    button.classList.remove('btn-outline-dark');
                    button.classList.add('btn-dark');
                }
            } else {
                alert('Erro ao executar ação');
            }
        } catch (error) {
            console.error('Erro ao executar ação de seguir/desseguir', error);
            alert('Erro de conexão');
        }
    });
});
