using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeoCidadao.Model.Enums;

namespace GeoCidadao.Model.Exceptions
{
    public class EntityValidationException : HttpResponseException
    {
        public string PropertyName { get; set; } = string.Empty;

        public EntityValidationException(string propertyName, string validationError, Dictionary<string, object>? additionalDetails = null)
            : base(400, $"Erro na validação da propriedade '{propertyName}': {validationError}", ErrorCodes.VALIDATION_ERROR, additionalDetails)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        public EntityValidationException(string propertyName, string validationError, ErrorCodes errorCode, Dictionary<string, object>? additionalDetails = null)
            : base(400, $"Erro na validação da propriedade '{propertyName}': {validationError}", errorCode, additionalDetails)
        {
            PropertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }
    }
}