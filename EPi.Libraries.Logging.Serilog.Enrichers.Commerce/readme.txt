NOTE: You will need a Logger configuration. 

    [ServiceConfiguration(ServiceType = typeof(ILoggerConfigurator), Lifecycle = ServiceInstanceScope.Singleton)]
    public class LoggerConfigurator : ILoggerConfigurator
    {
        public ILogger GetLogger()
        {
            return your own configuration here;
        }
    }

Sample configuration:

protected LoggerConfiguration GetLoggerConfiguration()
		{
			return new LoggerConfiguration()
			       .Enrich.WithCustomerData()
			       .Enrich.WithDefaultCartData()
			       .Enrich.WithMarketData()
		}

