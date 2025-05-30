namespace BarkeepersHandbook.Api.Exceptions;

public class ServiceLayerException : Exception
{
   public ServiceLayerException() { } 
   
   public ServiceLayerException(string message) : base(message) { }

   public ServiceLayerException(string message, Exception inner) : base(message, inner)
   { }
}