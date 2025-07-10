namespace ApiGateway.Configurations
{
    using System.Text.RegularExpressions;
    using System.Xml.Linq;
    using Consul;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Primitives;
    using Yarp.ReverseProxy.Configuration;

    /**
        * Description : ApiGateway Configuration for Consul
        * @author     : Nafiz Imtiaz khan
        * @since      : 06/29/2025      
        */
    public class ConsulConfig : BackgroundService, Yarp.ReverseProxy.Configuration.IProxyConfigProvider
    {
        private /*readonly*/ IConsulClient _consul = new ConsulClient(c => c.Address = new Uri("http://localhost:8500"));
        private volatile Config _config = new Config(
            new List<Yarp.ReverseProxy.Configuration.RouteConfig>(),
            new List<Yarp.ReverseProxy.Configuration.ClusterConfig>()
        );

        public Yarp.ReverseProxy.Configuration.IProxyConfig GetConfig() => _config;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var services = await _consul.Agent.Services();

                    var routes = new List<Yarp.ReverseProxy.Configuration.RouteConfig>
                {
                    // Match the SARB reporting service route
                    new Yarp.ReverseProxy.Configuration.RouteConfig
                    {
                        RouteId             = "report-route",
                        ClusterId           = "report-cluster",
                        Match = new RouteMatch
                        {
                            Path = "/api/sarb/{**catch-all}"         // ✔ proxies anything under /api/report/ *Match /sarb/ part after api/ with controller Name* <<<<<<<<<<<<<
                        }
                    },
                    // Match the GoAML reporting service route
                    new Yarp.ReverseProxy.Configuration.RouteConfig
                    {
                        RouteId             = "goaml-route",
                        ClusterId           = "goaml-cluster",
                        Match = new RouteMatch
                        {
                            Path = "/api/goaml/{**catch-all}"                 // ✔ proxies anything not matched by other routes
                        }
                    },
                    // Match the User Service route
                    new Yarp.ReverseProxy.Configuration.RouteConfig
                    {
                        RouteId             = "userservice-route",
                        ClusterId           = "userservice-cluster",
                        Match = new RouteMatch
                        {
                            Path = "/api/authentication/{**catch-all}"     //Match api/<sarb>/   (sarb) part with controller Name*
                        }
                    }
                };

                    var clusters = new List<Yarp.ReverseProxy.Configuration.ClusterConfig>();

                    // Find sarb report-services
                    var reportServices = services.Response.Values
                                         .Where(s => s.Service == "sarb-reporting")
                                         .ToList();

                    if (reportServices.Any())
                    {
                        clusters.Add(new ClusterConfig
                        {
                            ClusterId = "report-cluster",
                            Destinations = reportServices.ToDictionary(
                                s => s.ID,
                                s => new Yarp.ReverseProxy.Configuration.DestinationConfig
                                {
                                    // If SARB-Reporting exposes HTTPS only, keep https://
                                    Address = $"https://{s.Address}:{s.Port}/"
                                })
                        });
                    }

                    // Find goaml services
                    var goamlServices = services.Response.Values
                                         .Where(s => s.Service == "sagoaml-reporting")
                                         .ToList();
                    if (goamlServices.Any())
                    {
                        clusters.Add(new ClusterConfig
                        {
                            ClusterId = "goaml-cluster",
                            Destinations = goamlServices.ToDictionary(
                                s => s.ID,
                                s => new Yarp.ReverseProxy.Configuration.DestinationConfig
                                {
                                    // If SARB-GoAML exposes HTTPS only, keep https://
                                    Address = $"https://{s.Address}:{s.Port}/"
                                })
                        });
                    }

                    // Find user service
                    var userServices = services.Response.Values
                                         .Where(s => s.Service == "user-service")
                                         .ToList();
                    if (userServices.Any())
                    {
                        clusters.Add(new ClusterConfig
                        {
                            ClusterId = "userservice-cluster",
                            Destinations = userServices.ToDictionary(
                                s => s.ID,
                                s => new Yarp.ReverseProxy.Configuration.DestinationConfig
                                {
                                    // If SARB-User-Service exposes HTTPS only, keep https://
                                    Address = $"https://{s.Address}:{s.Port}/"
                                })
                        });
                    }
                        var oldConfig = _config;
                        _config = new Config(routes, clusters);
                        oldConfig.SignalChange();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating from Consul: {ex.Message}");
                }

                await Task.Delay(5000, stoppingToken);
            }
        }

        private class Config : Yarp.ReverseProxy.Configuration.IProxyConfig
        {
            private readonly CancellationTokenSource _cts = new CancellationTokenSource();

            public Config(
                IReadOnlyList<Yarp.ReverseProxy.Configuration.RouteConfig> routes,
                IReadOnlyList<Yarp.ReverseProxy.Configuration.ClusterConfig> clusters)
            {
                Routes = routes;
                Clusters = clusters;
                ChangeToken = new CancellationChangeToken(_cts.Token);
            }

            public IReadOnlyList<Yarp.ReverseProxy.Configuration.RouteConfig> Routes { get; }
            public IReadOnlyList<Yarp.ReverseProxy.Configuration.ClusterConfig> Clusters { get; }
            public IChangeToken ChangeToken { get; }

            public void SignalChange() => _cts.Cancel();
        }
    }
}
