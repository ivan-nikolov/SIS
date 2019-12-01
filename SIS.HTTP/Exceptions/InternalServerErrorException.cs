namespace SIS.HTTP.Exceptions
{
    using System;

    public class InternalServerErrorException : Exception
    {
        private const string DefaultMessage = "The Server has encountered an error.";

        public InternalServerErrorException()
            : this(DefaultMessage)
        {
        }

        public InternalServerErrorException(string message)
            : base(message)
        {
        }
    }
}
