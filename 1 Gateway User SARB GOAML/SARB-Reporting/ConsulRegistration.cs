namespace SARB_Reporting
{
    using Consul;
    public class ConsulRegistration : IHostedService
    {
        private readonly IConfiguration _config;
        private readonly IConsulClient _consul;
        private string _serviceId = default!;

        public ConsulRegistration(IConfiguration config)
        {
            _config = config;
            _consul = new ConsulClient(c => c.Address = new Uri("http://localhost:8500"));
        }

        public async Task StartAsync(CancellationToken ct)
        {
            // The service already listens on 5003 in launchSettings.json
            var uri = new Uri(_config["urls"] ?? "https://localhost:5004");

            _serviceId = $"sarb-reporting-{Guid.NewGuid()}";

            var registration = new AgentServiceRegistration
            {
                ID = _serviceId,
                Name = "sarb-reporting",           // ★ the name the gateway will look for
                Address = uri.Host,                    // e.g. "localhost"
                Port = uri.Port,                       // 5003
                Check = new AgentServiceCheck
                {
                    HTTP = $"{uri.Scheme}://{uri.Host}:{uri.Port}/health",
                    Interval = TimeSpan.FromSeconds(10),
                    DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
                }
            };

            await _consul.Agent.ServiceRegister(registration, ct);
        }

        public async Task StopAsync(CancellationToken ct) =>
            await _consul.Agent.ServiceDeregister(_serviceId, ct);
    }
}
