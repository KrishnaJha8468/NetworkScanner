using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Network Hosts and Port Scanner");
        Console.WriteLine("==============================\n");

        try
        {
            string localIp = GetLocalIpAddress();
            string baseIp = localIp.Substring(0, localIp.LastIndexOf('.') + 1);
            Console.WriteLine($"Scanning network: {baseIp}0/24");

            Console.WriteLine("\n[1/2] Scanning for active hosts...");
            var activeHosts = await DiscoverHosts(baseIp);
            Console.WriteLine($"Found {activeHosts.Count} active hosts.");

            Console.WriteLine("\n[2/2] Scanning common ports on active hosts...");
            foreach (var host in activeHosts)
            {
                Console.WriteLine($"\nScanning {host}...");
                var openPorts = await ScanPorts(host, new int[] { 21, 22, 23, 25, 53, 80, 110, 143, 443, 3389 });

                if (openPorts.Count > 0)
                {
                    Console.WriteLine("OPEN PORTS:");
                    foreach (var port in openPorts)
                    {
                        Console.WriteLine($"- {port.Key}: {port.Value}");
                    }
                }
                else
                {
                    Console.WriteLine("No open ports found.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("\nScan complete. Press any key to exit...");
        Console.ReadKey();
    }

    static async Task<List<IPAddress>> DiscoverHosts(string baseIp, int timeout = 1000)
    {
        var activeHosts = new List<IPAddress>();
        var tasks = new List<Task>();

        for (int i = 1; i < 255; i++)
        {
            string ip = baseIp + i;
            tasks.Add(Task.Run(async () =>
            {
                if (await PingHost(ip, timeout))
                {
                    lock (activeHosts)
                    {
                        activeHosts.Add(IPAddress.Parse(ip));
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);
        return activeHosts;
    }

    static async Task<bool> PingHost(string ip, int timeout)
    {
        try
        {
            using var ping = new Ping();
            PingReply reply = await ping.SendPingAsync(ip, timeout);
            return reply.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }

    static async Task<Dictionary<int, string>> ScanPorts(IPAddress ip, int[] ports, int timeout = 1000)
    {
        var openPorts = new Dictionary<int, string>();
        var tasks = new List<Task>();

        foreach (int port in ports)
        {
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    using var client = new TcpClient();
                    var task = client.ConnectAsync(ip, port);
                    if (await Task.WhenAny(task, Task.Delay(timeout)) == task && client.Connected)
                    {
                        string service = GetServiceName(port);
                        lock (openPorts)
                        {
                            openPorts.Add(port, service);
                        }
                    }
                }
                catch { }
            }));
        }

        await Task.WhenAll(tasks);
        return openPorts;
    }

    static string GetLocalIpAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No IPv4 address found!");
    }

    static string GetServiceName(int port)
    {
        return port switch
        {
            21 => "FTP",
            22 => "SSH",
            23 => "Telnet",
            25 => "SMTP",
            53 => "DNS",
            80 => "HTTP",
            110 => "POP3",
            143 => "IMAP",
            443 => "HTTPS",
            3389 => "RDP",
            _ => "Unknown"
        };
    }
}
