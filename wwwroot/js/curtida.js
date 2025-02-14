document.addEventListener("DOMContentLoaded", function () {
    // Seleciona todos os botões de curtida
    const botoesCurtida = document.querySelectorAll(".curtida-btn");

    botoesCurtida.forEach((button) => {
        button.addEventListener("click", async function () {
            const postId = button.dataset.postId; // Obtém o postId do atributo data-post-id

            try {
                const response = await fetch('/Curtida/CurtirOuDescurtir', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ postId: postId })
                });

                const result = await response.json();

                if (result.success) {
                    const icon = button.querySelector("i");

                    if (result.curtidaAtiva) {
                        button.classList.add("text-danger");
                        button.classList.remove("text-secondary");
                        icon.classList.add("bi-heart-fill");
                        icon.classList.remove("bi-heart");
                    } else {
                        button.classList.add("text-secondary");
                        button.classList.remove("text-danger");
                        icon.classList.add("bi-heart");
                        icon.classList.remove("bi-heart-fill");
                    }
                } else {
                    alert('Não foi possível realizar a ação.');
                }
            } catch (error) {
                console.error('Erro ao tentar curtir/descurtir:', error);
            }
        });
    });
});
