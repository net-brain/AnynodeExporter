using System.Net.Http;
using System.Net.Http.Headers;
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
        _lics = Metrics.CreateGauge("anynode_license_rest", "Rest in days for the license.",
         new GaugeConfiguration
         {
             LabelNames = new[] { "lic" }
         });
        _certs = Metrics.CreateGauge("anynode_certificate_rest", "Rest in days for the certificate.",
         new GaugeConfiguration
         {
             LabelNames = new[] { "cert" }
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

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Checking Dashboard:",ex);
            }

            try
            {
                var licenses = await GetApiResponse<List<License>>(_settings.Url + "/api/licenses/get?version=0");
                foreach (var lic in licenses)
                {
                    if (lic.validUntil!=null)
                    {
                        if (lic.validUntil.StartsWith("UTC"))
                        {
                            _lics.WithLabels(lic.name).Set((DateTime.Parse(lic.validUntil[4..]) - DateTime.Now).Days);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Checking Licenses: {ex}",ex);
            }

            try
            {
                var nodes = await GetApiResponse<List<Node>>(_settings.Url + "/api/nodes/get?version=0");
                foreach (var node in nodes)
                {
                    var certs = await GetApiResponse<List<Certificates>>($"{_settings.Url}/api/nodes/certificates/get?version=0&node={node.id}");
                    foreach (var cert in certs)
                    {
                        if (cert.certificate != null) _certs.WithLabels(cert.certificate.subject).Set((DateTime.Parse(cert.certificate.validUntil[4..]) - DateTime.Now).Days);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in Checking Licenses: {ex}",ex);
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

