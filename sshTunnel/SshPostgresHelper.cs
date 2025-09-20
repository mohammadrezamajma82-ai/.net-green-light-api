using Renci.SshNet;
using Npgsql;

public class SshPostgresHelper
{
    private SshClient? _sshClient;
    private ForwardedPortLocal? _portForward;
 
    public NpgsqlConnection Connect(
        string sshHost, string sshUser, string sshKeyPath,
        string dbHost, int dbPort, string dbName, string dbUser, string dbPass, uint localPort = 5433)
    {
        var keyFile = new PrivateKeyFile(sshKeyPath);
        var keyAuth = new PrivateKeyAuthenticationMethod(sshUser, keyFile);

        var connInfo = new Renci.SshNet.ConnectionInfo(sshHost, 22, sshUser, keyAuth)
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        _sshClient = new SshClient(connInfo);
        _sshClient.KeepAliveInterval = TimeSpan.FromSeconds(10);
        _sshClient.Connect();

        _portForward = new ForwardedPortLocal("127.0.0.1", localPort, dbHost, (uint)dbPort);
        _sshClient.AddForwardedPort(_portForward);
        _portForward.Start();

        string connString = $"Host=127.0.0.1;Port={localPort};Username={dbUser};Password={dbPass};Database={dbName}";
        var conn = new NpgsqlConnection(connString);
        conn.Open();
        return conn;
    }

    public void Disconnect()
    {
        _portForward?.Stop();
        _sshClient?.Disconnect();
    }
}
