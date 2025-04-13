// Responsável por válidar o file inserido na view EditarPerfil e exibir pré-visualização

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
                    alert('Formato inválido. Apenas .jpg, .jpeg, .png são permitidos.');
                    return;
                }

                // Atualiza a prévia da imagem
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

    // Enviar os dados do formulário via AJAX
    if (formPerfil) {
        formPerfil.addEventListener('submit', function (event) {
            event.preventDefault(); // Impede o envio padrão do formulário

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
                    location.reload(); // Atualiza a página para exibir a nova imagem
                })
                .catch(error => console.error("Erro ao atualizar perfil:", error));
        });
    }
});