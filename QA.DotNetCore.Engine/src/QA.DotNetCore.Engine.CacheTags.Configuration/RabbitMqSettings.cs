using System.ComponentModel.DataAnnotations;

namespace QA.DotNetCore.Engine.CacheTags.Configuration;

public class RabbitMqSettings
{
    private const string DefaultHost = "localhost";
    private const ushort DefaultPort = 5672;
    private const string DefaultUsername = "guest";
    private const string DefaultPassword = "guest";
    private const string RootVirtualPath = "/";
    private const int DefaultRetryLimit = 5;
    private static readonly TimeSpan _defaultRetryDelay = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan _defaultHeartbeat = TimeSpan.FromSeconds(20);

    public string Host { get; set; } = DefaultHost;

    public ushort Port { get; set; } = DefaultPort;

    public string Username { get; set; } = DefaultUsername;

    public string Password { get; set; } = DefaultPassword;

    public string VirtualPath { get; set; } = RootVirtualPath;

    [Range(0, int.MaxValue)]
    public int RetryLimit { get; set; } = DefaultRetryLimit;

    public TimeSpan RetryDelay { get; set; } = _defaultRetryDelay;

    public TimeSpan Heartbeat { get; set; } = _defaultHeartbeat;
}
