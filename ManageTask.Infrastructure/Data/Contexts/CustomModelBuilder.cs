using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Reflection;

namespace ManageTask.Infrastructure.Data.Contexts
{
    public class DateTimeKindValueConverter(DateTimeKind kind, ConverterMappingHints? mappingHints = default) :
        ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, kind), mappingHints
        )
    {
    }
    public static class CustomModelBuilder
    {
        public static void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureForecastModel(modelBuilder);
            modelBuilder.SetDefualtDateTimeKind(DateTimeKind.Utc);
        }

        private static void ConfigureForecastModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("uuid-ossp"); // установка расширения для uuid-ossp в postgresql

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public static ModelBuilder SetDefualtDateTimeKind(this ModelBuilder builder, DateTimeKind kind)
        {
            var converter = new DateTimeKindValueConverter(kind);
            builder.UseValueConverterForType<DateTime>(converter);
            builder.UseValueConverterForType<DateTime?>(converter);
            return builder;
        }

        private static ModelBuilder UseValueConverterForType<T>(this ModelBuilder builder, ValueConverter converter)
            => builder.UseValueConverterForType(typeof(T), converter);

        private static ModelBuilder UseValueConverterForType(this ModelBuilder builder, Type type, ValueConverter converter)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var properties = entityType.ClrType.GetProperties()
                    .Where(t => t.PropertyType == type);

                foreach (var property in properties)
                {
                    builder.Entity(entityType.Name).Property(property.Name)
                        .HasConversion(converter);
                }
            }

            return builder;
        }
    }
}
