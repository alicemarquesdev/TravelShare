let autocomplete;
function initAutocomplete() {
    const inputElements = document.querySelectorAll('.cities-autocomplete');

    inputElements.forEach(inputElement => {
        const autocomplete = new google.maps.places.Autocomplete(inputElement, {
            types: ['(cities)'], // Apenas cidades
            fields: ['place_id', 'geometry', 'name']
        });

        autocomplete.addListener('place_changed', function () {
            const place = autocomplete.getPlace();

            if (!place.geometry) {
                inputElement.value = ''; // Se não for válido, apaga o campo
                alert('Por favor, selecione uma cidade da lista.');
            }
        });

        // Bloqueia entrada manual inválida
        inputElement.addEventListener('blur', function () {
            const place = autocomplete.getPlace();
            if (!place || !place.geometry) {
                inputElement.value = ''; // Se não for uma cidade válida, limpa o campo
            }
        });
    });
}

window.onload = function () {
    initAutocomplete();
};