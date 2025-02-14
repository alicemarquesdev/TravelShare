function buscarUsuarios() {
    var termo = document.getElementById('pesquisaInput').value.trim();
    let suggestions = document.getElementById('suggestions');

    if (termo === "") {
        suggestions.style.display = 'none';
        return;
    }

    fetch('/Home/PesquisarUsuarios?termo=' + encodeURIComponent(termo))
        .then(response => {
            if (!response.ok) {
                throw new Error("Erro na requisição: " + response.status);
            }
            return response.json();
        })
        .then(data => {
            console.log(data); // Para depuração e verificação dos dados

            suggestions.innerHTML = '';

            if (!data || data.length === 0) {
                suggestions.style.display = 'none';
                return;
            }

            data.forEach(usuario => {
                let nome = usuario.nome;
                let username = usuario.username;
                let cidade = usuario.cidadeDeNascimento;
                let usuarioId = usuario.id;
                let imagemPerfil = usuario.imagemPerfil;

                // Criar o item de sugestão como um link
                let li = document.createElement('a');
                li.href = `/Usuario/Perfil/${usuarioId}`;
                li.classList.add('list-group-item', 'd-flex', 'align-items-center', 'text-decoration-none', 'text-secondary');
                li.style.display = 'flex';
                li.style.alignItems = 'center';
                li.style.gap = '10px';
                li.style.cursor = 'pointer';

                li.innerHTML = `
                    <img src="${imagemPerfil}" alt="Foto de perfil" class="rounded-circle" style="width: 40px; height: 40px;">
                    <div>
                        <div><strong>${nome}</strong> ${username}</div>
                        <div><small>${cidade}</small></div>
                    </div>
                `;

                // Adicionar o item ao container de sugestões
                suggestions.appendChild(li);
            });

            suggestions.style.display = 'block';
        })
        .catch(error => console.error('Erro ao buscar usuários:', error));
}
