using System.Text.Json.Serialization;
using Content.Shared.Cargo.Prototypes;
using Content.Shared.Storage.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.GuideGenerator;

public sealed class CargoCatalogueEntry
{
    /// <summary>
    ///     ID of cargoProduct.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; }

    /// <summary>
    ///     Name of cargoProduct. Shown on cargo catalogue.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; }

    /// <summary>
    ///     Description of cargoProduct. Shown on cargo catalogue.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; }

    /// <summary>
    ///     ID of a product that will be delivered on order.
    /// </summary>
    [JsonPropertyName("product")]
    public string Product { get; }

    /// <summary>
    ///     List of entities that "product" (thing that will be received when
    ///     ordered) will contain. If product cannot contain items then it will
    ///     have a single entry that will be product itself.
    /// </summary>
    [JsonPropertyName("contains")]
    public List<dynamic> Contains { get; }

    /// <summary>
    ///     Price of a product
    /// </summary>
    [JsonPropertyName("price")]
    public int Price { get; }

    /// <summary>
    ///     Catalogue category of a product (e.g. Engineering, Medical).
    /// </summary>
    [JsonPropertyName("category")]
    public string Category { get; }

    /// <summary>
    ///     Catalogue group of a product (e.g. Market, Contraband)
    /// </summary>
    [JsonPropertyName("group")]
    public string Group { get; }

    public CargoCatalogueEntry(CargoProductPrototype proto)
    {
        Id = proto.ID;
        Name = TextTools.TextTools.CapitalizeString(proto.Name);
        Description = TextTools.TextTools.CapitalizeString(proto.Description);
        Product = proto.Product;
        Contains = new List<dynamic>();
        if (IoCManager.Resolve<IPrototypeManager>().TryIndex(proto.Product, out EntityPrototype? prototype))
        {
            if (prototype.Components.TryGetComponent("StorageFill", out var storageFillCompRaw))
            {
                if (storageFillCompRaw != null)
                {
                    var storage = (StorageFillComponent)storageFillCompRaw;
                    foreach (var product in storage.Contents)
                    {
                        Contains.Add(new ContainedEntry(product.PrototypeId, product.SpawnProbability, product.Amount, product.MaxAmount, product.GroupId));
                    }
                }
            }
            else
            {
                Contains.Add(new ContainedEntry(proto.Product, 1, 1, null, null));
            }
        }
        Price = proto.PointCost;
        Category = proto.Category;
        Group = proto.Group;
    }
}

public sealed class ContainedEntry
{
    /// <summary>
    ///     ID of item.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; }

    /// <summary>
    ///     The probability that an item will spawn.
    /// </summary>
    [JsonPropertyName("prob")]
    public float Prob { get; }

    /// <summary>
    ///     Group to pick items from.
    /// </summary>
    [JsonPropertyName("group")]
    public string? GroupId { get; }

    /// <summary>
    ///     Minimum amount of item that will be spawned.
    /// </summary>
    [JsonPropertyName("amount")]
    public int Amount { get; }

    /// <summary>
    ///     Maximum amount of items that can be spawned.
    /// </summary>
    [JsonPropertyName("maxAmount")]
    public int? MaxAmount { get; }

    public ContainedEntry(string? id, float prob, int amount, int? maxAmount, string? group)
    {
        Id = id;
        Prob = prob;
        Amount = amount;
        if (maxAmount != null)
            MaxAmount = maxAmount;
        else
            MaxAmount = amount;
        GroupId = group;
    }
}
