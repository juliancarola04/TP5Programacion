namespace API.Excepciones;

public class RecursoNoExisteException : Exception
{
    // Error 404
    public RecursoNoExisteException(string mensaje) :
        base(mensaje)
    {

    }
}