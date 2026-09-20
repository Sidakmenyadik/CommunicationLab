using System.Net.Sockets;
using System.Text;

using var client = new TcpClient();

await client.ConnectAsync(
    "127.0.0.1",
    5000);

Console.WriteLine("================================");
Console.WriteLine("       ORDER SERVICE");
Console.WriteLine("================================");
Console.WriteLine("Connected to MessageServer.");
Console.WriteLine();

using var stream = client.GetStream();

while (true)
{
    Console.Write("Order ID: ");

    var orderId = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(orderId))
        continue;

    var message =
        $"OrderCreated: {orderId}";

    var data = Encoding.UTF8.GetBytes(message);

    await stream.WriteAsync(data);

    Console.WriteLine(
        $"Sent: {message}");

    Console.WriteLine();
}
