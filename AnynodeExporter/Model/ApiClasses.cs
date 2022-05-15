namespace AnynodeExporter.Model;

public class Dashboard
{
    public Sipnode[] sipNodes { get; set; }
    public Ldapconnection[] ldapConnections { get; set; }
    public object[] sfbUcmaNodes { get; set; }
}

public class Sipnode
{
    public Sessions sessions { get; set; }
    public Optionspackets optionsPackets { get; set; }
    public string displayName { get; set; }
    public string remoteSipDomain { get; set; }
    public string ipAddress { get; set; }
    public bool operational { get; set; }
    public Portranges portRanges { get; set; }
    public bool enabled { get; set; }
    public string networkState { get; set; }
    public Registeredclientlist[] registeredClientList { get; set; }
    public bool natTraversalEnabled { get; set; }
    public Localports localPorts { get; set; }
    public Transportconnection[] transportConnections { get; set; }
    public string registrationState { get; set; }
}

public class Sessions
{
    public int incomingCalls { get; set; }
    public int outgoingCalls { get; set; }
}

public class Optionspackets
{
    public Sent sent { get; set; }
    public Received received { get; set; }
}

public class Sent
{
    public int success { get; set; }
    public int failed { get; set; }
}

public class Received
{
    public int success { get; set; }
    public int failed { get; set; }
}

public class Portranges
{
    public Udp udp { get; set; }
    public Tcp tcp { get; set; }
}

public class Udp
{
    public int firstPort { get; set; }
    public int lastPort { get; set; }
}

public class Tcp
{
    public int firstPort { get; set; }
    public int lastPort { get; set; }
}

public class Localports
{
    public Udp1 udp { get; set; }
    public Tcp1 tcp { get; set; }
    public Tls tls { get; set; }
}

public class Udp1
{
    public bool enabled { get; set; }
    public int port { get; set; }
}

public class Tcp1
{
    public bool enabled { get; set; }
    public int port { get; set; }
}

public class Tls
{
    public int port { get; set; }
    public bool enabled { get; set; }
}

public class Registeredclientlist
{
    public string bindingIri { get; set; }
    public string lastRefresh { get; set; }
    public string addressOfRecord { get; set; }
    public string ipAddress { get; set; }
    public string registration { get; set; }
    public string expiration { get; set; }
    public string user { get; set; }
    public string sipUserAgent { get; set; }
}

public class Transportconnection
{
    public bool transportUp { get; set; }
    public string transportId { get; set; }
    public string transportTargetIri { get; set; }
}

public class Ldapconnection
{
    public string hostname { get; set; }
    public string encryption { get; set; }
    public string establishTime { get; set; }
    public int port { get; set; }
    public int cachedItems { get; set; }
    public string displayName { get; set; }
    public string lastSearchTime { get; set; }
    public int searchCount { get; set; }
    public string state { get; set; }
    public string username { get; set; }
}

public class Node
{
    public string displayName { get; set; }
    public string id { get; set; }
}

public class Facility
{
    public string name { get; set; }
    public int value { get; set; }
}

public class License
{
    public string identifier { get; set; }
    public string requiredSystemIds { get; set; }
    public string name { get; set; }
    public string validUntil { get; set; }
    public string validFrom { get; set; }
    public string id { get; set; }
    public List<Facility> facilities { get; set; }
    public string softwareUpdateServiceUntil { get; set; }
    public string products { get; set; }
    public string status { get; set; }
}

public class Certificate
{
    public List<string> extendedUsage { get; set; }
    public string subject { get; set; }
    public List<string> subjectAlternativeNames { get; set; }
    public List<string> usage { get; set; }
    public string validFrom { get; set; }
    public string version { get; set; }
    public string issuer { get; set; }
    public string serial { get; set; }
    public string signingAlgorithm { get; set; }
    public string fingerprintAlgorithm { get; set; }
    public string fingerprint { get; set; }
    public string validUntil { get; set; }
    public string certificateAuthority { get; set; }
}

public class PrivateKey
{
    public int keySize { get; set; }
    public string keyType { get; set; }
}

public class Certificates
{
    public Certificate certificate { get; set; }
    public PrivateKey privateKey { get; set; }
}


