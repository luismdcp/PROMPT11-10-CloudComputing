using System;

namespace CloudNotes.Infrastructure.Exceptions
{
    public class EntityAlreadyExistsException : Exception
    {
        #region Constructors

        public EntityAlreadyExistsException() { }

        public EntityAlreadyExistsException(string message) : base(message) { }

        public EntityAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }

        #endregion Constructors
    }
}