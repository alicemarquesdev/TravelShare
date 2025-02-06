using System.ComponentModel.DataAnnotations;

namespace TravelShare.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "O email/username é obrigatório")]
        public string EmailOuUsername { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Senha { get; set; }
    }
}