using System.Security.Cryptography;
using System.Text;

namespace TravelShare.Helper
{
    public static class Criptografia
    {
        public static string GerarHash(this string valor)
        {
            var hash = SHA1.Create();  // Converte uma entrada de dados (neste caso, uma string) em um valor de tamanho fixo de 160 bits.
            var encoding = new ASCIIEncoding(); // Será usada para converter a string para uma sequência de bytes, pois o algoritmo SHA1 trabalha com bytes, não com strings diretamente.
            var array = encoding.GetBytes(valor); // Converte a string valor em um array de bytes usando a codificação ASCII.

            array = hash.ComputeHash(array); // Calcula o hash SHA1 para o array de bytes gerado anteriormente. Isso produz um novo array de bytes que representa o hash da string.

            var strHexa = new StringBuilder(); // Cria um StringBuilder para construir a representação final do hash em formato hexadecimal (uma string com números e letras de "0" a "f").

            foreach (var item in array)
            {
                strHexa.Append(item.ToString("x2"));
            }

            // Para cada byte no array de hash resultante,  converte cada byte em uma representação hexadecimal de dois dígitos (o "x2" formata o byte para hexadecimal com dois dígitos, por exemplo, 10 vira 0a).
            // Adiciona essa representação hexadecimal ao StringBuilder.

            return strHexa.ToString(); //  Converte o StringBuilder para uma string e retorna o hash da string original, agora em formato hexadecimal.
        }
    }
}