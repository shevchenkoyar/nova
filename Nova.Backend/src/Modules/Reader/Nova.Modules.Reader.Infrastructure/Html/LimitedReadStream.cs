namespace Nova.Modules.Reader.Infrastructure.Html;

internal sealed class LimitedReadStream(
    Stream inner,
    long maxBytes)
    : Stream
{
    private long _totalRead;

    public override bool CanRead => inner.CanRead;
    public override bool CanSeek => false;
    public override bool CanWrite => false;
    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => _totalRead;
        set => throw new NotSupportedException();
    }

    public override int Read(
        byte[] buffer,
        int offset,
        int count)
    {
        var remaining = maxBytes - _totalRead;

        if (remaining <= 0)
            throw new InvalidOperationException($"Response is too large. Max bytes: {maxBytes}.");

        var allowed = (int)Math.Min(count, remaining);
        var read = inner.Read(buffer, offset, allowed);

        _totalRead += read;

        return read;
    }

    public override async ValueTask<int> ReadAsync(
        Memory<byte> buffer,
        CancellationToken cancellationToken = default)
    {
        var remaining = maxBytes - _totalRead;

        if (remaining <= 0)
            throw new InvalidOperationException($"Response is too large. Max bytes: {maxBytes}.");

        var allowed = (int)Math.Min(buffer.Length, remaining);
        var read = await inner.ReadAsync(buffer[..allowed], cancellationToken);

        _totalRead += read;

        return read;
    }

    public override void Flush() => throw new NotSupportedException();

    public override long Seek(
        long offset,
        SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(
        byte[] buffer,
        int offset,
        int count) => throw new NotSupportedException();
}