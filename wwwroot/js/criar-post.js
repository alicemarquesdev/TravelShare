// Responsável por analisar file inserido na view CRIAR POST e exibir pré-visualização

document.addEventListener("DOMContentLoaded", function () {
    const imagemPostInput = document.getElementById("ImagemPost");
    const warning = document.getElementById("fileWarning");
    const imagePreviewContainer = document.getElementById("currentImagePreview");

    if (imagemPostInput && warning && imagePreviewContainer) {
        imagemPostInput.addEventListener("change", function () {
            const files = this.files;
            const maxFiles = 10;
            warning.textContent = ""; // Limpa mensagens de erro anteriores
            imagePreviewContainer.innerHTML = ""; // Limpa pré-visualizações anteriores

            // Verifica se o número de arquivos não excede o máximo permitido
            if (files.length > maxFiles) {
                warning.textContent = `Você pode adicionar no máximo ${maxFiles} arquivos.`;
                this.value = ""; // Limpa os arquivos selecionados
                return;
            }

            const extensoesPermitidas = ["jpg", "jpeg", "png"];

            // Verifica se os arquivos selecionados são válidos
            if (files.length > 0) {
                Array.from(files).forEach((file) => {
                    const extensaoArquivo = file.name.split(".").pop().toLowerCase();

                    // Valida a extensão do arquivo
                    if (!extensoesPermitidas.includes(extensaoArquivo)) {
                        warning.textContent = "Formato de imagem não permitido. Apenas .jpg, .jpeg, .png.";
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

                            // Redimensiona a largura e altura se necessário
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

                            // Criar pré-visualização da imagem
                            const previewImage = document.createElement("img");
                            previewImage.src = e.target.result;
                            previewImage.classList.add("img-fluid", "m-1");
                            previewImage.style.maxWidth = `200px`;
                            previewImage.style.maxHeight = `200px`;

                            imagePreviewContainer.appendChild(previewImage);
                        };
                    };

                    reader.readAsDataURL(file); // Lê o arquivo como URL de dados
                });
            } else {
                warning.textContent = "Por favor, selecione ao menos uma imagem.";
            }
        });
    }
});
