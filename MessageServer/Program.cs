using System.Net;
using System.Net.Sockets;
using System.Text;

var listener = new TcpListener(
    IPAddress.Loopback,
    5000);

listener.Start();

Console.WriteLine("================================");
Console.WriteLine("       MESSAGE SERVER");
Console.WriteLine("================================");
Console.WriteLine("Listening on 127.0.0.1:5000");
Console.WriteLine();

var clients = new List<TcpClient>();

while (true)
{
    var client = await listener.AcceptTcpClientAsync();

    lock (clients)
    {
        clients.Add(client);
    }

    Console.WriteLine("Service connected.");

    _ = HandleClient(client);
}

async Task HandleClient(TcpClient client)
{
    try
    {
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

            Console.WriteLine($"Received: {message}");

            await Broadcast(message, client);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Client error: {ex.Message}");
    }
    finally
    {
        lock (clients)
        {
            clients.Remove(client);
        }

        client.Dispose();

        Console.WriteLine("Service disconnected.");
    }
}

async Task Broadcast(
    string message,
    TcpClient sender)
{
    var data = Encoding.UTF8.GetBytes(message);

    List<TcpClient> currentClients;

    lock (clients)
    {
        currentClients = clients.ToList();
    }

    foreach (var client in currentClients)
    {
        // Jangan kirim kembali ke pengirim
        if (client == sender)
            continue;

        try
        {
            await client
                .GetStream()
                .WriteAsync(data);
        }
        catch
        {
            // Client mungkin sudah disconnect
        }
    }
}
