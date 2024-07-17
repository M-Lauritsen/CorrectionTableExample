namespace CorrectionTableExample.Models.CorrectionModels;

public class PropertyCorrection
{
    public int Id { get; set; }
    public string PropertyName { get; set; }
    public string OldValue { get; set; }
    public string NewValue { get; set; }
}