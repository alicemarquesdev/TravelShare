// Responsável pela exibição do mapa em perfil
// Contém o AutoComplete para adicionar cidades existentes usando api do google maps

let map;
let geocoder;

// Função principal que inicializa o Google Maps e os componentes relevantes
function initGoogleMaps() {
    // Se o campo de autocompletar de cidade estiver presente, inicializa o autocomplete
    if (document.querySelector('.cities-autocomplete')) {
        initAutocomplete();
    }

    // Se o mapa estiver presente na página, inicializa o mapa
    if (document.getElementById('map')) {
        initMap();
    }
}

// Função que inicializa o recurso de autocompletar para cidades
function initAutocomplete() {
    // Seleciona todos os campos de entrada com a classe 'cities-autocomplete'
    const inputElements = document.querySelectorAll('.cities-autocomplete');

    // Itera por cada campo de entrada de cidade
    inputElements.forEach(inputElement => {
        // Inicializa o Autocomplete do Google Maps para esse campo, restrito a cidades
        const autocomplete = new google.maps.places.Autocomplete(inputElement, {
            types: ['(cities)'], // Limita a busca a cidades
            fields: ['geometry', 'name', 'address_components'] // Campos que serão retornados
        });

        // Evento acionado quando o usuário seleciona uma cidade
        autocomplete.addListener('place_changed', function () {
            const place = autocomplete.getPlace();

            // Verifica se a cidade foi selecionada corretamente
            if (!place.geometry || !place.address_components) {
                alert('Por favor, selecione uma cidade da lista.');
                inputElement.value = ''; // Limpa o campo caso não tenha sido selecionado corretamente
                return;
            }

            // Variáveis para armazenar o nome da cidade e do país
            let city = '';
            let country = '';

            // Itera pelos componentes de endereço e extrai cidade e país
            place.address_components.forEach(component => {
                if (component.types.includes('locality')) {
                    city = component.long_name;
                }
                if (component.types.includes('country')) {
                    country = component.long_name;
                }
            });

            // Preenche o campo de entrada com a cidade e país
            if (city && country) {
                inputElement.value = `${city}, ${country}`;
            } else {
                alert('Por favor, selecione uma cidade válida.');
                inputElement.value = ''; // Limpa o campo se a cidade ou país não forem encontrados
            }
        });

        // Evento para limpar o campo de entrada se o foco for perdido e nenhuma cidade válida for selecionada
        inputElement.addEventListener('blur', function () {
            const place = autocomplete.getPlace();
            if (!place || !place.geometry) {
                inputElement.value = ''; // Limpa o campo se o local não for encontrado
            }
        });

        // Previne o comportamento padrão de enviar o formulário ao pressionar "Enter"
        inputElement.addEventListener('keydown', function (event) {
            if (event.key === 'Enter') {
                event.preventDefault();
            }
        });
    });
}

// Função para inicializar o mapa
function initMap() {
    const mapOptions = {
        center: { lat: 0, lng: 15 }, // Definindo as coordenadas iniciais do mapa para o Oceano Atlântico
        zoom: 1, // Nível de zoom inicial
        minZoom: 2, // Zoom mínimo permitido
        maxZoom: 10, // Zoom máximo permitido
        mapTypeId: 'roadmap', // Tipo de mapa a ser exibido
    };

    // Cria o mapa e armazena no objeto 'map'
    map = new google.maps.Map(document.getElementById('map'), mapOptions);
    geocoder = new google.maps.Geocoder(); // Inicializa o geocodificador para transformar endereços em coordenadas

    // Obtém a lista de cidades a partir de um elemento de dados (dataset)
    const dadosCidades = document.getElementById("dadosCidades").dataset.cidades;
    const cidades = JSON.parse(dadosCidades); // Converte os dados em JSON para um array de cidades
    console.log(cidades); // Exibe as cidades no console

    // Adiciona os marcadores para as cidades no mapa
    addMarkers(cidades);
}

// Função para adicionar marcadores no mapa com base em uma lista de cidades
function addMarkers(cidades) {
    if (!cidades || cidades.length === 0) {
        console.log("Lista de cidades está vazia!"); // Verifica se a lista de cidades está vazia
        return;
    }

    // Itera pelas cidades e geocodifica cada uma delas para obter as coordenadas
    cidades.forEach(city => {
        geocoder.geocode({ 'address': city }, function (results, status) {
            if (status === 'OK') {
                const location = results[0].geometry.location;

                // Cria um marcador para a cidade no mapa
                const marker = new google.maps.Marker({
                    map: map,
                    position: location,
                    title: city
                });

                // Cria uma janela de informações que será aberta ao clicar no marcador
                const infowindow = new google.maps.InfoWindow({
                    content: `<strong><a href="https://www.google.com/maps?q=${encodeURIComponent(city)}" target="_blank">${city}</a></strong>`
                });

                // Adiciona um evento de clique no marcador para abrir a janela de informações
                marker.addListener('click', function () {
                    infowindow.open(map, marker);
                });
            } else {
                console.log('Erro ao geocodificar a cidade:', city); // Exibe um erro caso o geocoding falhe
            }
        });
    });
}


// Garante que a função 'initGoogleMaps' seja executada corretamente após o carregamento da API do Google Maps
window.initGoogleMaps = initGoogleMaps;
