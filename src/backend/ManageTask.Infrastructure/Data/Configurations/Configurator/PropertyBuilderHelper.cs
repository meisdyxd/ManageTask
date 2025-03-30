using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageTask.Infrastructure.Data.Configurations.Configurator
{
    public static class PropertyBuilderHelper
    {
        public static PropertyBuilder<Guid> IsGuid(this PropertyBuilder<Guid> propertyBuilder)
            => propertyBuilder.HasColumnType("uuid");
        
    }
    public struct Constants
    {
        public static int MaxLengthName = 32;
        public static int MaxLengthEmail = 32;
    }
}
