using System;
using Calabonga.Microservices.Core;

namespace Calabonga.UnitOfWork.Controllers.Exceptions
{
    [Serializable]
    public class UowArgumentNullException: Exception
    {
        public UowArgumentNullException() : base(AppContracts.Exceptions.ArgumentNullException)
        {

        }

        public UowArgumentNullException(string message) : base(message)
        {

        }

        public UowArgumentNullException(string message, Exception exception) : base(message, exception)
        {

        }

        public UowArgumentNullException(Exception exception) : base(AppContracts.Exceptions.ArgumentNullException, exception)
        {

        }
    }
}
