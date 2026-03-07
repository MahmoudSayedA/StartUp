namespace Application.Common.Exceptions
{
    public class IntegrationBadResponseException : AppException
    {
        public IntegrationBadResponseException() : base($"Bad Response was receive from integration service.", 503, null) { }

        public IntegrationBadResponseException( string message ) : base( message, 503, null ) { }
    }
}
