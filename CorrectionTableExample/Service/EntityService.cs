using CorrectionTableExample.Data;
using CorrectionTableExample.Models.CorrectionModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace CorrectionTableExample.Service;

public class EntityService<TEntity> where TEntity : class, IEntity, new()
{
    private readonly DataContext _context;

    public EntityService(DataContext context)
    {
        _context = context;
    }

    public TEntity GetEntityWithCorrections(int entityId)
    {
        var entity = _context.Set<TEntity>().Find(entityId);
        if (entity == null) return null;

        var latestCorrection = _context.Set<EntityCorrection<TEntity>>()
            .Include(ec => ec.PropertyCorrections)
            .Where(ec => ec.OriginalEntityId == entityId)
            .OrderByDescending(ec => ec.CorrectionDate)
            .FirstOrDefault();

        if (latestCorrection != null)
        {
            ApplyCorrections(entity, latestCorrection.PropertyCorrections);
        }

        return entity;
    }

    public void ApplyCorrection(int entityId, Dictionary<string, string> corrections)
    {
        var entity = _context.Set<TEntity>().Find(entityId);
        if (entity == null) throw new ArgumentException("Entity not found");

        var entityCorrection = new EntityCorrection<TEntity>
        {
            OriginalEntityId = entityId,
            CorrectionDate = DateTime.Now
        };

        foreach (var correction in corrections)
        {
            var propertyInfo = typeof(TEntity).GetProperty(correction.Key);
            if (propertyInfo == null) continue;

            var oldValue = propertyInfo.GetValue(entity)?.ToString();
            var newValue = correction.Value;

            if (oldValue != newValue)
            {
                entityCorrection.PropertyCorrections.Add(new PropertyCorrection
                {
                    PropertyName = correction.Key,
                    OldValue = oldValue,
                    NewValue = newValue
                });

                var convertedValue = Convert.ChangeType(newValue, propertyInfo.PropertyType);
                propertyInfo.SetValue(entity, convertedValue);
            }
        }

        if (entityCorrection.PropertyCorrections.Any())
        {
            _context.Set<EntityCorrection<TEntity>>().Add(entityCorrection);
            _context.SaveChanges();
        }
    }

    private void ApplyCorrections(TEntity entity, List<PropertyCorrection> corrections)
    {
        foreach (var correction in corrections)
        {
            var propertyInfo = typeof(TEntity).GetProperty(correction.PropertyName);
            if (propertyInfo != null)
            {
                var convertedValue = Convert.ChangeType(correction.NewValue, propertyInfo.PropertyType);
                propertyInfo.SetValue(entity, convertedValue);
            }
        }
    }
}