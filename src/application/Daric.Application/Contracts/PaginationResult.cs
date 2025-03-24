namespace Daric.Application.Contracts
{
    public record PaginationResult<T>(int Count, T[] Result)
    {

    }
}
