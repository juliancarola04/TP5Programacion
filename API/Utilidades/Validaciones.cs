namespace API.Utilidades
{
    public static class Validaciones
    {
        public static bool Requeridos(params string?[] valores)
        {
            if (valores == null || valores.Length == 0)
                return false;

            // Si alguno está vacío, rebotamos.
            return valores.All(v => !string.IsNullOrWhiteSpace(v));
        }

        public static bool Requeridos(params int?[] valores)
        {
            if (valores == null || valores.Length == 0)
                return false;


            return valores.All(v => v.HasValue);
        }


        public static bool Requeridos(params decimal?[] valores)
        {
            if (valores == null || valores.Length == 0) 
                return false;
            

            return valores.All(v => v.HasValue);
        }
    }
}
