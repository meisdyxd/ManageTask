using ManageTask.Infrastructure.Data.Configurations.Configurator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ManageTask.Infrastructure.Data.Contexts
{
    public class DataContextConfiguration
    {
        internal class DataContextConfigurator(IConfiguration configuration, ILoggerFactory loggerFactory) :
        BaseDbContextConfigurator<DataContext>(configuration, loggerFactory)
        {
            protected override string ConnectionStringName => "Postgres";
        }
    }
}
