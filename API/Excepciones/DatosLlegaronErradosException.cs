namespace API.Excepciones
{
    public class DatosLlegaronErradosException : Exception
    {
        // Error 400
        public DatosLlegaronErradosException(string mensaje) :
            base(mensaje)
        {

        }
    }
}
