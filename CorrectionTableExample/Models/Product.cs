using CorrectionTableExample.Models.CorrectionModels;

namespace CorrectionTableExample.Models;

public class Product : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
