namespace API.Excepciones;

public class RecursoExistenteException : Exception
{
    // Error 409
    public RecursoExistenteException(string mensaje) :
        base(mensaje)
    {

    }
}