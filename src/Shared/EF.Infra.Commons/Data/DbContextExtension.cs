using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EF.Infra.Commons.Data;

public static class DbContextExtension
{
    public static void SetDates(IEnumerable<EntityEntry> entries)
    {
        foreach (var entry in entries
                     .Where(entry =>
                         entry.Entity.GetType().GetProperty("DataCriacao") != null &&
                         entry.Entity.GetType().GetProperty("DataAtualizacao") != null))
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("DataCriacao").CurrentValue = DateTime.Now.ToUniversalTime();
                entry.Property("DataAtualizacao").CurrentValue = null;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Property("DataAtualizacao").CurrentValue = DateTime.Now.ToUniversalTime();
                entry.Property("DataCriacao").IsModified = false;
            }
        }
    }
}