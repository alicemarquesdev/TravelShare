namespace TravelShare.Helper
{
    public static class TempoDecorrido
    {
        public static string TempoDecorridoPost(this DateTime dataCriacao)
        {
            var tempoPassado = DateTime.Now - dataCriacao;

            if (tempoPassado.TotalMinutes < 1)
            {
                return "Agora mesmo";
            }
            else if (tempoPassado.TotalMinutes < 60)
            {
                return $"{(int)tempoPassado.TotalMinutes} minutos";
            }
            else if (tempoPassado.TotalHours < 24)
            {
                return $"{(int)tempoPassado.TotalHours} horas";
            }
            else if (tempoPassado.TotalDays < 7)
            {
                return $"{(int)tempoPassado.TotalDays} dias";
            }
            else
            {
                return dataCriacao.ToString("dd/MM/yyyy");
            }
        }
    }
}