using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.Common
{
    public static class Guard
    {
        public static void ArgumentNotNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        public static void ArgumentNotNullOrEmpty(string argumentValue, string argumentName)
        {
            ArgumentNotNull(argumentValue, argumentName);
            if (string.IsNullOrEmpty(argumentValue))
            {
                throw new ArgumentException(argumentName);
            }
        }

        public static void Argument(Func<bool> validationFunction, string argumentName, string argumentMessage = null)
        {
            if (validationFunction == null)
            {
                return;
            }

            if (!validationFunction())
            {
                throw !string.IsNullOrEmpty(argumentMessage)
                    ? new ArgumentException(argumentMessage, argumentName)
                    : new ArgumentException(argumentName);
            }
        }
    }
}