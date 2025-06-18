// Usado na view Explorar para buscar usuários

// Função que busca usuários com base no termo de pesquisa fornecido pelo usuário
function buscarUsuarios() {
    // Obtém o valor do campo de input e remove espaços extras
    var termo = document.getElementById('pesquisaInput').value.trim();

    // Obtém o elemento onde as sugestões serão exibidas
    let suggestions = document.getElementById('suggestions');

    // Se o termo de pesquisa estiver vazio, oculta as sugestões e encerra a função
    if (termo === "") {
        suggestions.style.display = 'none';
        return;
    }

    // Faz uma requisição GET para o servidor para buscar usuários com o termo fornecido
    fetch('/Home/PesquisarUsuarios?termo=' + encodeURIComponent(termo))
        .then(response => {
            // Se a resposta não for bem-sucedida, lança um erro
            if (!response.ok) {
                throw new Error("Erro na requisição: " + response.status);
            }
            // Retorna a resposta como um JSON
            return response.json();
        })
        .then(data => {
            // Exibe os dados retornados para depuração
            console.log(data); // Para depuração e verificação dos dados

            // Limpa as sugestões anteriores, caso haja
            suggestions.innerHTML = '';

            // Se não houver dados ou a lista estiver vazia, oculta as sugestões
            if (!data || data.length === 0) {
                suggestions.style.display = 'none';
                return;
            }

            // Itera sobre os dados dos usuários retornados
            data.forEach(usuario => {
                // Extrai informações do usuário
                let nome = usuario.nome;
                let username = usuario.username;
                let cidade = usuario.cidadeDeNascimento;
                let usuarioId = usuario.id;
                let imagemPerfil = usuario.imagemPerfil;

                // Cria um novo item de sugestão como um link
                let li = document.createElement('a');
                li.href = `/Usuario/Perfil/${usuarioId}`; // Link para o perfil do usuário
                li.classList.add('list-group-item', 'd-flex', 'align-items-center', 'text-decoration-none', 'text-secondary');
                li.style.display = 'flex';
                li.style.alignItems = 'center';
                li.style.gap = '10px';
                li.style.cursor = 'pointer';

                // Define o conteúdo do item de sugestão com as informações do usuário
                li.innerHTML = `
                    <img src="${imagemPerfil}" alt="Foto de perfil" class="rounded-circle" style="width: 40px; height: 40px; object-fit:cover;">
                    <div>
                        <div><strong>${nome}</strong> ${username}</div>
                        <div><small>${cidade}</small></div>
                    </div>
                `;

                // Adiciona o item de sugestão ao container de sugestões
                suggestions.appendChild(li);
            });

            // Exibe as sugestões após adicionar os itens
            suggestions.style.display = 'block';
        })
        .catch(error => console.error('Erro ao buscar usuários:', error)); // Exibe erros no console, caso ocorram
}
// Função para esconder as sugestões quando o clique for fora do dropdown
document.addEventListener('click', function (event) {
    let suggestions = document.getElementById('suggestions');
    let inputField = document.getElementById('pesquisaInput');

    // Se o clique foi fora do campo de input e das sugestões, esconda as sugestões
    if (!inputField.contains(event.target) && !suggestions.contains(event.target)) {
        suggestions.style.display = 'none';
    }
});