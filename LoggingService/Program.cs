using System.Net.Sockets;
using System.Text;

using var client = new TcpClient();

await client.ConnectAsync(
    "127.0.0.1",
    5000);

Console.WriteLine("================================");
Console.WriteLine("       LOGGING SERVICE");
Console.WriteLine("================================");
Console.WriteLine("Connected to MessageServer.");
Console.WriteLine();

using var stream = client.GetStream();

var buffer = new byte[4096];

while (true)
{
    var bytesRead = await stream.ReadAsync(buffer);

    if (bytesRead == 0)
        break;

    var message = Encoding.UTF8.GetString(
        buffer,
        0,
        bytesRead);

    Console.WriteLine(
        $"[LOG] {DateTime.Now:HH:mm:ss} - {message}");
}
