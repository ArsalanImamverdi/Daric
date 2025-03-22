namespace Daric.Domain.Shared.Extensions;

public static class ExceptionExtensions
{
    public static string GetMessage(this Exception exception)
    {
        if (exception is AggregateException aggregateException && aggregateException.InnerException is not null)
        {
            return string.Join("[IN]:", exception.Message, aggregateException.InnerException.GetMessage());
        }
        return exception.Message;
    }
}
