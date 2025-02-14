/*!
    * Start Bootstrap - SB Admin v7.0.7 (https://startbootstrap.com/template/sb-admin)
    * Copyright 2013-2023 Start Bootstrap
    * Licensed under MIT (https://github.com/StartBootstrap/startbootstrap-sb-admin/blob/master/LICENSE)
    */
//
// Scripts
//

window.addEventListener('DOMContentLoaded', event => {
    // Toggle the side navigation
    const sidebarToggle = document.body.querySelector('#sidebarToggle');
    if (sidebarToggle) {
        // Uncomment Below to persist sidebar toggle between refreshes
        // if (localStorage.getItem('sb|sidebar-toggle') === 'true') {
        //     document.body.classList.toggle('sb-sidenav-toggled');
        // }
        sidebarToggle.addEventListener('click', event => {
            event.preventDefault();
            document.body.classList.toggle('sb-sidenav-toggled');
            localStorage.setItem('sb|sidebar-toggle', document.body.classList.contains('sb-sidenav-toggled'));
        });
    }

    $('.close-alert').click(function () {
        $(".alert").hide();
    }); 
});
function initAutocomplete() {
    var input = document.getElementById('cities-autocomplete');
    if (input) {
        var autocomplete = new google.maps.places.Autocomplete(input, {
            types: ['(cities)']
        });

        autocomplete.addListener('place_changed', function () {
            var place = autocomplete.getPlace();
            if (!place.geometry || !place.address_components) {
                alert('Por favor, selecione uma cidade da lista.');
                input.value = '';
                return;
            }

            var city = '';
            var country = '';
            place.address_components.forEach(component => {
                if (component.types.includes('locality')) {
                    city = component.long_name;
                }
                if (component.types.includes('country')) {
                    country = component.long_name;
                }
            });

            if (city && country) {
                input.value = `${city}, ${country}`;
            } else {
                alert('Por favor, selecione uma cidade válida.');
                input.value = '';
            }
        });
    }
}


// VALIDAÇÃO DA SENHA - REGULAR EXPRESSION
document.addEventListener("DOMContentLoaded", function () {
    const senhaInput = document.getElementById("Senha");
    const senhaErro = document.getElementById("senhaErro");

    senhaInput.addEventListener("input", function () {
        const senha = senhaInput.value;
        const regexSenha = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$/;

        if (!regexSenha.test(senha)) {
            senhaErro.textContent = "A senha deve ter no mínimo 8 caracteres e no máximo 20, incluindo pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial.";
        } else {
            senhaErro.textContent = ""; // Remove o erro quando estiver correta
        }
    });
});