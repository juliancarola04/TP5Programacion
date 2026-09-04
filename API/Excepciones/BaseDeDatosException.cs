namespace API.Excepciones
{
    public class BaseDeDatosException : Exception
    {
        // Error 500
        public BaseDeDatosException(string mensaje) :
            base(mensaje)
        {

        }
    }
}
