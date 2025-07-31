/**
* Description : ApiGateway Registration for Consul
* @author     : Nafiz Imtiaz khan
* @since      : 06/29/2025      
*/
namespace ApiGateway.Configurations
{
    using Consul;
    /// <summary>
    /// Handles the registration and deregistration of the API Gateway service with Consul.
    /// This class implements <see cref="IHostedService"/> to manage the service lifecycle
    /// within a .NET Core host. It ensures that the API Gateway is discoverable by other
    /// services via Consul.
    /// </summary>
    public class GatewayConsulRegistration : IHostedService
    {
        private readonly IConfiguration _config;
        private readonly IConsulClient _consul;
        private string _serviceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayConsulRegistration"/> class.
        /// </summary>
        /// <param name="config">The application's configuration, used to retrieve URL information.</param>
        public GatewayConsulRegistration(IConfiguration config)
        {
            _config = config;
            _consul = new ConsulClient(c => c.Address = new Uri("http://localhost:8500"));
        }

        /// <summary>
        /// Starts the API Gateway service registration process with Consul.
        /// This method is called by the .NET Core host when the application starts.
        /// It registers the API Gateway with a unique ID, name, address, port, and a health check.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the registration process.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous start operation.</returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var uri = new Uri(_config["urls"] ?? "http://localhost:5000");
            _serviceId = $"api-gateway-{Guid.NewGuid()}";

            var registration = new AgentServiceRegistration
            {
                ID = _serviceId,
                Name = "api-gateway",
                Address = uri.Host,
                Port = uri.Port,
                Tags = new[] { "gateway", "yarp", "api", "Consul" },
                Check = new AgentServiceCheck
                {
                    HTTP = $"{uri}health",
                    Interval = TimeSpan.FromSeconds(10),
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)
                }
            };

            await _consul.Agent.ServiceRegister(registration);
            Console.WriteLine($"API Gateway registered with Consul as {_serviceId}");
        }

        /// <summary>
        /// Stops the API Gateway service and deregisters it from Consul.
        /// This method is called by the .NET Core host when the application is gracefully shutting down.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that can be used to cancel the deregistration process.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous stop operation.</returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _consul.Agent.ServiceDeregister(_serviceId);
            Console.WriteLine("API Gateway deregistered from Consul");
        }
    }
}
