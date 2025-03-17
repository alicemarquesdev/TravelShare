namespace TravelShare.Helper
{
    // Classe estática para utilitários relacionados ao tempo decorrido
    public static class TempoDecorrido
    {
        // Função de extensão que retorna o tempo decorrido desde a data de criação
        public static string CalcularTempoDecorrido(this DateTime dataCriacao)
        {
            // Calcula o tempo que passou desde a data de criação
            var tempoPassado = DateTime.Now - dataCriacao;

            // Se passou menos de 1 minuto, retorna "Agora mesmo"
            if (tempoPassado.TotalMinutes < 1)
            {
                return "Agora mesmo";
            }
            // Se passou menos de uma hora, retorna o número de minutos
            else if (tempoPassado.TotalMinutes < 60)
            {
                return $"{(int)tempoPassado.TotalMinutes} minutos";
            }
            // Se passou menos de um dia, retorna o número de horas
            else if (tempoPassado.TotalHours < 24)
            {
                return $"{(int)tempoPassado.TotalHours} horas";
            }
            // Se passou menos de uma semana, retorna o número de dias
            else if (tempoPassado.TotalDays < 7)
            {
                return $"{(int)tempoPassado.TotalDays} dias";
            }
            // Caso contrário, retorna a data formatada
            else
            {
                return dataCriacao.ToString("dd/MM/yyyy");
            }
        }
    }
}
