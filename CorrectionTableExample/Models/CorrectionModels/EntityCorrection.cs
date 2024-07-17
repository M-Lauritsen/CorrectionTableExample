namespace CorrectionTableExample.Models.CorrectionModels;

public class EntityCorrection<TEntity> where TEntity : class, IEntity
{
    public int Id { get; set; }
    public int OriginalEntityId { get; set; }
    public DateTime CorrectionDate { get; set; }
    public List<PropertyCorrection> PropertyCorrections { get; set; } = new List<PropertyCorrection>();

    public TEntity OriginalEntity { get; set; }
}
