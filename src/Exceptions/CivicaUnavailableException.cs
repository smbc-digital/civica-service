using System;

namespace civica_service.Exceptions
{
    [Serializable]
    public class CivicaUnavailableException : Exception
    {
        public CivicaUnavailableException()
        {

        }

        public CivicaUnavailableException(string name): base($"Civica Unavailable: {name}")
        {

        }
    }
}
