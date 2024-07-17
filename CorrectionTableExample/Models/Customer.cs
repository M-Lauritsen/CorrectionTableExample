using CorrectionTableExample.Models.CorrectionModels;

namespace CorrectionTableExample.Models;

public class Customer : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}