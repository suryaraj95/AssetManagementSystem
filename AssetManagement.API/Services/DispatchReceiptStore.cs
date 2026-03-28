namespace AssetManagement.API.Services;

/// <summary>
/// In-memory store for dispatch receipt files.
/// Files are kept for the lifetime of the application process.
/// </summary>
public record ReceiptEntry(byte[] Data, string ContentType, string FileName);

public class DispatchReceiptStore
{
    private readonly Dictionary<Guid, ReceiptEntry> _store = new();

    public void Save(Guid assetId, ReceiptEntry entry) => _store[assetId] = entry;

    public ReceiptEntry? Get(Guid assetId) =>
        _store.TryGetValue(assetId, out var entry) ? entry : null;

    public bool Has(Guid assetId) => _store.ContainsKey(assetId);
}
