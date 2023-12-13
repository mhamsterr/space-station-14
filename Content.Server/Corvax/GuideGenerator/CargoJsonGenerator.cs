using System.IO;
using System.Linq;
using System.Text.Json;
using Content.Shared.Cargo.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.GuideGenerator;

public sealed class CargoJsonGenerator
{
    public static void PublishJson(StreamWriter file)
    {
        var prototype = IoCManager.Resolve<IPrototypeManager>();
        var prototypes =
            prototype
                .EnumeratePrototypes<CargoProductPrototype>()
                .Select(x => new CargoCatalogueEntry(x))
                .ToList();

        var categorizedCatalogue = new Dictionary<string, dynamic>();
        foreach (var proto in prototypes)
        {
            if (categorizedCatalogue.ContainsKey(proto.Category))
            {
                categorizedCatalogue[proto.Category].Add(proto.Id, proto);
            }
            else
            {
                categorizedCatalogue.Add(proto.Category, new Dictionary<string, dynamic>());
                categorizedCatalogue[proto.Category].Add(proto.Id, proto);
            }
        }

        var serializeOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        file.Write(JsonSerializer.Serialize(categorizedCatalogue, serializeOptions));
    }
}

