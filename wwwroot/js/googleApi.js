let map;
let geocoder;

function initGoogleMaps() {
    if (document.querySelector('.cities-autocomplete')) {
        initAutocomplete();
    }

    if (document.getElementById('map')) {
        initMap();
    }
}

function initAutocomplete() {
    const inputElements = document.querySelectorAll('.cities-autocomplete');

    inputElements.forEach(inputElement => {
        const autocomplete = new google.maps.places.Autocomplete(inputElement, {
            types: ['(cities)'],
            fields: ['geometry', 'name', 'address_components']
        });

        autocomplete.addListener('place_changed', function () {
            const place = autocomplete.getPlace();

            if (!place.geometry || !place.address_components) {
                alert('Por favor, selecione uma cidade da lista.');
                inputElement.value = '';
                return;
            }

            let city = '';
            let country = '';
            place.address_components.forEach(component => {
                if (component.types.includes('locality')) {
                    city = component.long_name;
                }
                if (component.types.includes('country')) {
                    country = component.long_name;
                }
            });

            if (city && country) {
                inputElement.value = `${city}, ${country}`;
            } else {
                alert('Por favor, selecione uma cidade válida.');
                inputElement.value = '';
            }
        });

        inputElement.addEventListener('blur', function () {
            const place = autocomplete.getPlace();
            if (!place || !place.geometry) {
                inputElement.value = '';
            }
        });

        inputElement.addEventListener('keydown', function (event) {
            if (event.key === 'Enter') {
                event.preventDefault();
            }
        });
    });
}

// Inicializa o mapa
function initMap() {
    const mapOptions = {
        center: { lat: 0, lng: 15 }, // Coordenadas do Oceano Atlântico
        zoom: 3,
        minZoom: 2,
        maxZoom: 10,
        mapTypeId: 'roadmap',
    };

    map = new google.maps.Map(document.getElementById('map'), mapOptions);
    geocoder = new google.maps.Geocoder();

    // Lista de cidades corrigida
    const dadosCidades = document.getElementById("dadosCidades").dataset.cidades;
    const cidades = JSON.parse(dadosCidades);
    console.log(cidades);

    console.log("Cidades carregadas:", cidades);

    // Adiciona os marcadores e ajusta o zoom
    addMarkers(cidades);

    // Adiciona o marcador da cidade de origem (caso tenha)
    const cidadeOrigem = document.getElementById("cidadeNascimento").dataset.cidadenascimento;
    if (cidadeOrigem) {
        addMarkerCidadeOrigem(cidadeOrigem);
    }

    console.log(cidadeOrigem)
}

// Adiciona marcadores no mapa
function addMarkers(cidades) {
    if (!cidades || cidades.length === 0) {
        console.log("Lista de cidades está vazia!");
        return;
    }
    cidades.forEach(city => {
        geocoder.geocode({ 'address': city }, function (results, status) {
            if (status === 'OK') {
                const location = results[0].geometry.location;

                const marker = new google.maps.Marker({
                    map: map,
                    position: location,
                    title: city
                });

                const infowindow = new google.maps.InfoWindow({
                    content: `<strong>${city}</strong>`
                });

                marker.addListener('click', function () {
                    infowindow.open(map, marker);
                });


            } else {
                console.log('Erro ao geocodificar a cidade:', city);
            }
        });
    });
    });
}

// Função para adicionar o marcador da cidade de origem
function addMarkerCidadeOrigem(cidadeOrigem) {
    geocoder.geocode({ 'address': cidadeOrigem }, function (results, status) {
        if (status === 'OK') {
            const location = results[0].geometry.location;

            const marker = new google.maps.Marker({
                map: map,
                position: location,
                title: cidadeOrigem,
                icon: 'https://maps.google.com/mapfiles/ms/icons/blue-dot.png'  // Usando o pino azul padrão

            });

            const infowindow = new google.maps.InfoWindow({
                content: `<strong>${cidadeOrigem}</strong>`
            });

            marker.addListener('click', function () {
                infowindow.open(map, marker);
            });

        } else {
            console.log('Erro ao geocodificar a cidade:', cidadeOrigem);
        }
    });
}

// Garante que a API do Google Maps carregue corretamente
window.initGoogleMaps = initGoogleMaps;
