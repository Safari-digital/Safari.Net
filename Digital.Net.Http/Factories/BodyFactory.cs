using System.Net.Http.Json;
using Digital.Net.Http.Factories.models;

namespace Digital.Net.Http.Factories;

public static class BodyFactory
{
    public static JsonContent BuildPatch(object payload) =>
        JsonContent.Create(BuildPatchRows(payload));

    public static List<PatchRow> BuildPatchRows(object payload)
    {
        List<PatchRow> patch = [];
        foreach (var property in payload.GetType().GetProperties())
        {
            var value = property.GetValue(payload);
            if (value is not null)
                patch.Add(new PatchRow("replace", $"/{property.Name}", value));
        }

        return patch;
    }
}