using System;

namespace CloudNotes.Infrastructure.Exceptions
{
    public class InvalidValueTypeException : Exception
    {
        #region Constructors

        public InvalidValueTypeException() { }

        public InvalidValueTypeException(string message) : base(message) { }

        public InvalidValueTypeException(string message, Exception innerException) : base(message, innerException) { }

        #endregion Constructors
    }
}