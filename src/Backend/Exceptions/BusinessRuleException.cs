namespace Rte.Api.Exceptions;

public class BusinessRuleException : AppException
{
    public BusinessRuleException(string message) : base(message, StatusCodes.Status422UnprocessableEntity)
    {
    }
}
