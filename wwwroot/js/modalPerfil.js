// Carregar posts e inicializar eventos ao abrir o modal
document.querySelectorAll('.post-thumbnail').forEach(img => {
    img.addEventListener('click', event => {
        const postId = event.target.getAttribute('data-post-id');
        const modal = document.getElementById(`postModal_${postId}`);

        // Verifica se o modal está carregado corretamente
        if (modal) {
            // Inicializa o carrossel dinamicamente
            const carouselElement = modal.querySelector(`#carouselPostImages_${postId}`);
            if (carouselElement) {
                const bsCarousel = bootstrap.Carousel.getInstance(carouselElement) || new bootstrap.Carousel(carouselElement);
                bsCarousel.to(0); // Garante que comece no primeiro slide
            }
        } else {
            console.error(`Modal não encontrado para o Post ID: ${postId}`);
        }
    });
});