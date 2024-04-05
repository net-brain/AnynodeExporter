using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using AnynodeExporter.Model;

namespace AnynodeExporter.Service;

public class DataChecker : BackgroundService
{
    private readonly ILogger<DataChecker> _logger;
    private readonly AnynodeSettings _settings;
    private readonly IHttpClientFactory _api;
    private readonly HttpClient _client;
    private static Gauge _nodeState;
    private static Gauge _ldapState;
    private static Gauge _outgoingCalls;
    private static Gauge _incomingCalls;
    private static Gauge _lics;
    private static Gauge _certs;
    private static Gauge _certsregistered;

    public DataChecker(IOptions<AnynodeSettings> settings,
                    ILogger<DataChecker> logger,
                    IHttpClientFactory clientFactory)
    {
        _settings = settings.Value;
        _logger = logger;
        _api = clientFactory;

        var auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_settings.User}:{ _settings.Password}"));
        _client = _api.CreateClient("api");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", auth);

        Metrics.SuppressDefaultMetrics(); // keine Defaultausgaben

        _incomingCalls = Metrics.CreateGauge("anynode_incoming_calls", "Number of active incoming calls.",
        new GaugeConfiguration
        {
            LabelNames = new[] { "node" }
        });

        _outgoingCalls = Metrics.CreateGauge("anynode_outgoing_calls", "Number of active outgoing calls.",
        new GaugeConfiguration
        {
            LabelNames = new[] { "node" }
        });

        _nodeState = Metrics.CreateGauge("anynode_node_state", "Status of the configured nodes.",
         new GaugeConfiguration
         {
             LabelNames = new[] { "node" }
         });

        _ldapState = Metrics.CreateGauge("anynode_ldap_state", "Status of the configured LDAP connections.",
         new GaugeConfiguration
         {
             LabelNames = new[] { "ldap" }
         });
        _certs = Metrics.CreateGauge("anynode_certificate_node_expiration_in_days", "Certificate expiration in days for the node certificate.",
         new GaugeConfiguration
         {
             LabelNames = new[] { "cert" }
         });
        _certsregistered = Metrics.CreateGauge("anynode_certificate_registered_expiration_in_days", "Certificate expiration in days for certificates used for registered apps.",
        new GaugeConfiguration
        {
            LabelNames = new[] { "cn" }
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug($"DataChecker is starting.");

        stoppingToken.Register(() => _logger.LogDebug($" DataChecker background task is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogDebug($"DataChecker checks dashboard.");
            try
            {
                var dashboard = await GetApiResponse<Dashboard>(_settings.Url + "/api/dashboard/get?version=0");

                foreach (var node in dashboard.sipNodes)
                {
                    _incomingCalls.WithLabels(node.displayName).Set(node.sessions.incomingCalls);
                    _outgoingCalls.WithLabels(node.displayName).Set(node.sessions.outgoingCalls);
                    _nodeState.WithLabels(node.displayName).Set(node.operational ? 1 : 0);
                }
                foreach (var ldap in dashboard.ldapConnections)
                {
                    _ldapState.WithLabels(ldap.displayName).Set(ldap.state == "connected" ? 1 : 0);
                }
                foreach (var cert in dashboard.certificates)
                {
                    if (cert.commonName != null) _certsregistered.WithLabels(cert.commonName).Set(cert.expiresInDays);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Checking Dashboard: {ex}",ex);
            }

            await Task.Delay(_settings.Period * 1000, stoppingToken);
        }

        _logger.LogDebug($"DataChecker background task is stopping.");
    }

    private async Task<T> GetApiResponse<T>(string uri)
    {
        var innerresponse = await _client.GetAsync(uri);
        innerresponse.EnsureSuccessStatusCode();
        var result = await innerresponse.Content.ReadAsStringAsync();
        if (string.IsNullOrEmpty(result)) return default;
        return JsonSerializer.Deserialize<T>(result);
    }
}
