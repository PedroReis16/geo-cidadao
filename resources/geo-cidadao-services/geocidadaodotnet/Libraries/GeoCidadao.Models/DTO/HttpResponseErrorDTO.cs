using GeoCidadao.Models.Exceptions;

namespace GeoCidadao.Models.DTO
{
    public class HttpResponseErrorDTO
    {
        public int StatusCode { get; set; }
        public string ErrorMessage { get; set; } = null!;
        public string ErrorCode { get; set; } = null!;

        public HttpResponseErrorDTO()
        {

        }

        public HttpResponseErrorDTO(HttpResponseException ex)
        {
            StatusCode = ex.StatusCode;
            ErrorMessage = ex.ErrorMessage;
            ErrorCode = ex.ErrorCode.ToString();
        }
    }
}