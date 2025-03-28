/*
* Start Bootstrap - Resume v7.0.6 (https://startbootstrap.com/theme/resume)
* Copyright 2013-2023 Start Bootstrap
* Licensed under MIT (https://github.com/StartBootstrap/startbootstrap-resume/blob/master/LICENSE)
*/
//
// Scripts

// NAVBAR VIEWCOMPONENTS MENU

window.addEventListener('DOMContentLoaded', event => {
    // Activate Bootstrap scrollspy on the main nav element
    const sideNav = document.body.querySelector('#sideNav');
    if (sideNav) {
        new bootstrap.ScrollSpy(document.body, {
            target: '#sideNav',
            rootMargin: '0px 0px -40%',
        });
    };

    // Collapse responsive navbar when toggler is visible
    const navbarToggler = document.body.querySelector('.navbar-toggler');
    const responsiveNavItems = [].slice.call(
        document.querySelectorAll('#navbarResponsive .nav-link')
    );
    responsiveNavItems.map(function (responsiveNavItem) {
        responsiveNavItem.addEventListener('click', () => {
            if (window.getComputedStyle(navbarToggler).display !== 'none') {
                navbarToggler.click();
            }
        });
    });
});

// VIEW CRIAR POST

document.addEventListener("DOMContentLoaded", function () {
    const imagemPostInput = document.getElementById("ImagemPost");
    const warning = document.getElementById("fileWarning");
    const imagePreviewContainer = document.getElementById("currentImagePreview");

    if (imagemPostInput && warning && imagePreviewContainer) {
        imagemPostInput.addEventListener("change", function () {
            const files = this.files;
            const maxFiles = 10;
            warning.textContent = ""; // Limpa mensagens de erro anteriores
            imagePreviewContainer.innerHTML = ""; // Limpa pr�-visualiza��es anteriores

            // Verifica se o n�mero de arquivos n�o excede o m�ximo permitido
            if (files.length > maxFiles) {
                warning.textContent = `Voc� pode adicionar no m�ximo ${maxFiles} arquivos.`;
                this.value = ""; // Limpa os arquivos selecionados
                return;
            }

            const extensoesPermitidas = ["jpg", "jpeg", "png"];

            // Verifica se os arquivos selecionados s�o v�lidos
            if (files.length > 0) {
                Array.from(files).forEach((file) => {
                    const extensaoArquivo = file.name.split(".").pop().toLowerCase();

                    // Valida a extens�o do arquivo
                    if (!extensoesPermitidas.includes(extensaoArquivo)) {
                        warning.textContent = "Formato de imagem n�o permitido. Apenas .jpg, .jpeg, .png.";
                        this.value = ""; // Limpa os arquivos selecionados
                        return;
                    }

                    const reader = new FileReader();

                    reader.onload = function (e) {
                        const image = new Image();
                        image.src = e.target.result;

                        image.onload = function () {
                            const maxWidth = 1080;
                            const minWidth = 600;
                            const maxHeight = 1080;
                            const minHeight = 600;

                            let width = image.width;
                            let height = image.height;

                            // Redimensiona a largura e altura se necess�rio
                            if (width > maxWidth) {
                                const ratio = maxWidth / width;
                                width = maxWidth;
                                height = height * ratio;
                            } else if (width < minWidth) {
                                const ratio = minWidth / width;
                                width = minWidth;
                                height = height * ratio;
                            }

                            if (height > maxHeight) {
                                const ratio = maxHeight / height;
                                height = maxHeight;
                                width = width * ratio;
                            } else if (height < minHeight) {
                                const ratio = minHeight / height;
                                height = minHeight;
                                width = width * ratio;
                            }

                            // Criar pr�-visualiza��o da imagem
                            const previewImage = document.createElement("img");
                            previewImage.src = e.target.result;
                            previewImage.classList.add("img-fluid", "m-1");
                            previewImage.style.maxWidth = `200px`;
                            previewImage.style.maxHeight = `200px`;

                            imagePreviewContainer.appendChild(previewImage);
                        };
                    };

                    reader.readAsDataURL(file); // L� o arquivo como URL de dados
                });
            } else {
                warning.textContent = "Por favor, selecione ao menos uma imagem.";
            }
        });
    }
});


// EDITAR PERFIL 
document.addEventListener("DOMContentLoaded", function () {
    const profilePictureInput = document.getElementById('profilePicture');
    const profileImagePreview = document.getElementById('profileImagePreview');
    const formPerfil = document.getElementById('formEditarPerfil');

    if (profilePictureInput) {
        profilePictureInput.addEventListener('change', function (event) {
            const file = event.target.files[0];

            if (file) {
                const extensoesPermitidas = ["jpg", "jpeg", "png"];
                const extensao = file.name.split('.').pop().toLowerCase();

                if (!extensoesPermitidas.includes(extensao)) {
                    alert('Formato inv�lido. Apenas .jpg, .jpeg, .png s�o permitidos.');
                    return;
                }

                // Atualiza a pr�via da imagem
                const reader = new FileReader();
                reader.onload = function () {
                    if (profileImagePreview) {
                        profileImagePreview.src = reader.result;
                    }
                };
                reader.readAsDataURL(file);
            }
        });
    }

    // Enviar os dados do formul�rio via AJAX
    if (formPerfil) {
        formPerfil.addEventListener('submit', function (event) {
            event.preventDefault(); // Impede o envio padr�o do formul�rio

            const formData = new FormData(formPerfil);

            // Adiciona a imagem ao formData se existir
            if (profilePictureInput.files.length > 0) {
                formData.append("imagem", profilePictureInput.files[0]);
            }

            fetch(formPerfil.action, {
                method: "POST",
                body: formData
            })
                .then(response => {
                    if (!response.ok) {
                        throw new Error("Erro ao atualizar perfil.");
                    }
                    return response.text(); // Pode ser JSON dependendo do backend
                })
                .then(data => {
                    alert("Perfil atualizado com sucesso!");
                    location.reload(); // Atualiza a p�gina para exibir a nova imagem
                })
                .catch(error => console.error("Erro ao atualizar perfil:", error));
        });
    }
});


// VALIDA��O DA SENHA - REGULAR EXPRESSION
document.addEventListener("DOMContentLoaded", function () {
    const senhaInput = document.getElementById("Senha");
    const senhaErro = document.getElementById("senhaErro");

    senhaInput.addEventListener("input", function () {
        const senha = senhaInput.value;
        const regexSenha = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$/;

        if (!regexSenha.test(senha)) {
            senhaErro.textContent = "A senha deve ter no m�nimo 8 caracteres e no m�ximo 20, incluindo pelo menos uma letra mai�scula, uma min�scula, um n�mero e um caractere especial.";
        } else {
            senhaErro.textContent = ""; // Remove o erro quando estiver correta
        }
    });
});
