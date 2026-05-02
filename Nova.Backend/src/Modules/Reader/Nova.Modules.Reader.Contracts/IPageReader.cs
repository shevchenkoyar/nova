namespace Nova.Modules.Reader.Contracts;

public interface IPageReader
{
    Task<ReadPageResult> FetchAsync(
        FetchUrlRequest request,
        CancellationToken ct);
}