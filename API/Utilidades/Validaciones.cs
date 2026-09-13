using System.Text.RegularExpressions;

namespace API.Utilidades
{
    public static class Validaciones
    {
        public static bool EstanDatosBien(params string?[] valores)
        {
            if (valores == null || valores.Length == 0)
                return false;

            // Si alguno está vacío, rebotamos.
            return valores.All(v => !string.IsNullOrWhiteSpace(v));
        }

        public static bool EstanDatosBien(params int?[] valores)
        {
            if (valores == null || valores.Length == 0)
                return false;


            return valores.All(v => v.HasValue);
        }


        public static bool EstanDatosBien(params decimal?[] valores)
        {
            if (valores == null || valores.Length == 0)
                return false;


            return valores.All(v => v.HasValue);
        }

        // https://www.youtube.com/watch?v=2ujFcfybwhw
        public static bool EsUnEmailValido(string email)
        {
            Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase);

            return emailRegex.IsMatch(email);
        }
    }
}
