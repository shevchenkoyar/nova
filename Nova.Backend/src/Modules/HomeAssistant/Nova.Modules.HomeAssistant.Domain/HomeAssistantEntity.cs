namespace Nova.Modules.HomeAssistant.Domain;

public sealed class HomeAssistantEntity
{
    private HomeAssistantEntity()
    {
    }

    public Guid Id { get; private set; }

    public string EntityId { get; private set; } = string.Empty;

    public string Domain { get; private set; } = string.Empty;

    public string State { get; private set; } = string.Empty;

    public string? FriendlyName { get; private set; }

    public string? DeviceClass { get; private set; }

    public string? UnitOfMeasurement { get; private set; }

    public string? Area { get; private set; }

    public string AttributesJson { get; private set; } = "{}";

    public bool IsDeleted { get; private set; }

    public DateTimeOffset SyncedAt { get; private set; }

    public static HomeAssistantEntity Create(
        string entityId,
        string state,
        string? friendlyName,
        string? deviceClass,
        string? unitOfMeasurement,
        string? area,
        string attributesJson)
    {
        if (string.IsNullOrWhiteSpace(entityId))
            throw new ArgumentException("EntityId is required.", nameof(entityId));

        return new HomeAssistantEntity
        {
            Id = Guid.NewGuid(),
            EntityId = entityId.Trim(),
            Domain = ExtractDomain(entityId),
            State = state,
            FriendlyName = friendlyName,
            DeviceClass = deviceClass,
            UnitOfMeasurement = unitOfMeasurement,
            Area = area,
            AttributesJson = attributesJson,
            IsDeleted = false,
            SyncedAt = DateTimeOffset.UtcNow
        };
    }

    public void Update(
        string state,
        string? friendlyName,
        string? deviceClass,
        string? unitOfMeasurement,
        string? area,
        string attributesJson)
    {
        State = state;
        FriendlyName = friendlyName;
        DeviceClass = deviceClass;
        UnitOfMeasurement = unitOfMeasurement;
        Area = area;
        AttributesJson = attributesJson;
        IsDeleted = false;
        SyncedAt = DateTimeOffset.UtcNow;
    }

    public void MarkDeleted()
    {
        IsDeleted = true;
        SyncedAt = DateTimeOffset.UtcNow;
    }

    private static string ExtractDomain(string entityId)
    {
        var index = entityId.IndexOf('.');

        return index <= 0
            ? string.Empty
            : entityId[..index];
    }
}