namespace Nova.Common.Application.Tools;

using System.Text.Json;

public static class ToolArgumentValidator
{
    public static bool Validate(
        object schema,
        JsonElement args,
        out string error)
    {
        error = string.Empty;

        if (args.ValueKind != JsonValueKind.Object)
        {
            error = "Arguments must be a JSON object.";
            return false;
        }

        var schemaJson = JsonSerializer.Serialize(schema);

        using var schemaDoc = JsonDocument.Parse(schemaJson);

        if (!schemaDoc.RootElement.TryGetProperty("required", out var required))
            return true;

        foreach (var requiredProperty in required.EnumerateArray())
        {
            var name = requiredProperty.GetString();

            if (string.IsNullOrWhiteSpace(name))
                continue;

            if (!args.TryGetProperty(name, out _))
            {
                error = $"Missing required field: {name}.";
                return false;
            }
        }

        return true;
    }
}