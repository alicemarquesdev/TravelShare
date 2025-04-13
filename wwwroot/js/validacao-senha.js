// VALIDAÇÃO DA SENHA - REGULAR EXPRESSION
document.addEventListener("DOMContentLoaded", function () {
    const senhaInput = document.getElementById("Senha");
    const senhaErro = document.getElementById("senhaErro");
    const regexSenha = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$/;

    senhaInput.addEventListener("blur", function () { // Valida ao perder o foco
        const senha = senhaInput.value.trim();

        if (senha === "") {
            senhaErro.textContent = ""; // Remove erro se o campo estiver vazio
        } else if (!regexSenha.test(senha)) {
            senhaErro.textContent = "A senha deve ter entre 8 e 20 caracteres, incluindo pelo menos uma letra maiúscula, uma minúscula, um número e um caractere especial.";
        } else {
            senhaErro.textContent = ""; // Remove o erro se a senha for válida
        }
    });
});