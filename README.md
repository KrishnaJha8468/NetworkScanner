# NetworkScanner
Network Hosts and Port Scanner
A simple Network Hosts and Port Scanner built with C# and .NET. This tool allows you to scan a network or a specific IP address for active hosts and open ports, providing a quick overview of potential services running on those hosts.

Features
IP Range Scanning: Scans your local network (or a custom IP range) for live hosts.

Port Scanning: Checks for open ports on discovered hosts, including common ports like FTP, SSH, HTTP, HTTPS, and more.

Custom IP Support: Ability to scan a specific IP address for open ports instead of scanning an entire network range.

Installation
Clone the repository:

bash
Copy
Edit
git clone https://github.com/yourusername/NetworkScanner.git
cd NetworkScanner
Build the project:
If you haven't already, make sure you have .NET SDK installed and then run:

bash
Copy
Edit
dotnet build
Usage
Scan the local network:
Run the following command to scan your local network and find active hosts and open ports:

bash
Copy
Edit
dotnet run
Scan a specific IP address:
To scan a specific IP address for open ports, use:

bash
Copy
Edit
dotnet run -- <target-ip>
Replace <target-ip> with the IP address you want to scan (e.g., 192.168.1.1).

Example Output
markdown
Copy
Edit
Network Hosts and Port Scanner
==============================

Scanning network: 192.168.0.0/24
[1/2] Scanning for active hosts...
Found 4 active hosts.
[2/2] Scanning common ports on active hosts...

Scanning 192.168.0.10...
OPEN PORTS:
- 22: SSH
- 80: HTTP

Scanning 192.168.0.12...
No open ports found.
Requirements
.NET 6 or later

An internet connection for downloading dependencies (if needed)
