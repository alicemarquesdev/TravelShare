using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    public class UsuarioSemSenhaModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Username é obrigatório.")]
        [StringLength(20, ErrorMessage = "O Username deve ter no máximo 20 caracteres.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [StringLength(150, ErrorMessage = "O Email deve ter no máximo 150 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Data de nascimento é obrigatória.")]
        [Range(typeof(DateTime), "01/01/1900", "01/01/2025", ErrorMessage = "Data de nascimento inválida.")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "Lugar de Nascimento é obrigátorio.")]
        [StringLength(200, ErrorMessage = "O país de nascimento deve ter no máximo 200 caracteres.")]
        public string CidadeDeNascimento { get; set; }

        public string? FotoPerfil { get; set; } = "~/assets/img/profile-img.jpg";

        [StringLength(255, ErrorMessage = "A Bio deve ter no máximo 255 caracteres.")]
        public string? Bio { get; set; }

        public List<string> Seguindo { get; set; } = new List<string>();

        public List<string> Seguidores { get; set; } = new List<string>();

        public List<string> CidadesVisitadas { get; set; } = new List<string>();
    }
}