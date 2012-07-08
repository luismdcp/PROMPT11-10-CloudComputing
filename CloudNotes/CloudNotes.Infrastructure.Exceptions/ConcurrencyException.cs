using System;

namespace CloudNotes.Infrastructure.Exceptions
{
    public class ConcurrencyException : Exception
    {
        #region Constructors

        public ConcurrencyException() { }

        public ConcurrencyException(string message) : base(message) { }

        public ConcurrencyException(string message, Exception innerException) : base(message, innerException) { }

        #endregion Constructors
    }
}