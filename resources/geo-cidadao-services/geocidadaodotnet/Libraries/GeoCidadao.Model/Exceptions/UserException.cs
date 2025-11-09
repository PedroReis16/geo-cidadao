using GeoCidadao.Model.Enums;

namespace GeoCidadao.Model.Exceptions
{
    public class UserException : HttpResponseException
    {
        public UserException(int statusCode, string errorMessage, Dictionary<string, object>? additionalDetails = null) : base(statusCode, errorMessage, additionalDetails)
        {
        }

        public UserException(string errorMessage, ErrorCodes errorCode) : base(400, errorMessage, errorCode)
        {

        }

        public UserException(int statusCode, string errorMessage, ErrorCodes errorCode, Dictionary<string, object>? additionalDetails = null) : base(statusCode, errorMessage, errorCode, additionalDetails)
        {

        }
    }
}