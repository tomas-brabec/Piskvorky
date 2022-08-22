using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Piskvorky
{
    public class NetworkConnection
    {
        public static int serverPort = 40701;
        public static int clientPort = 40702;

        private TcpClient? tcpClient;

        public event EventHandler<MessageArgs>? OnMessageReceive;

        public async Task<NetworkMessage> RunServerAsync(string playerName, Player playerMark, CancellationToken token)
        {
            var acceptedConnection = new IPEndPoint(IPAddress.Any, serverPort);
            var response = new NetworkMessage() { Type = MessageType.Handshake, Name = playerName, Mark = playerMark };
            UdpClient udpClient = null!;
            TcpListener tcpListener = null!;

            try
            {
                tcpClient?.Dispose();
                udpClient = new UdpClient(acceptedConnection);
                tcpListener = new TcpListener(acceptedConnection);
                tcpListener.Start();
                var message = await ReceiveHandshakeAsync(udpClient, response, token);
                tcpClient = await tcpListener.AcceptTcpClientAsync(token);

                return message;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                udpClient?.Dispose();
                tcpListener?.Stop();
            }
        }

        private async Task<NetworkMessage> ReceiveHandshakeAsync(UdpClient client, NetworkMessage response, CancellationToken token)
        {
            var requestData = await client.ReceiveAsync(token);

            var message = JsonSerializer.Deserialize<NetworkMessage>(Encoding.UTF8.GetString(requestData.Buffer));

            if (message is null)
                throw new InvalidDataException();

            var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

            await client.SendAsync(responseBytes, requestData.RemoteEndPoint, token);

            return message;
        }

        public async Task<NetworkMessage> ConnectToServerAsync(string player, CancellationToken token)
        {
            UdpClient udpClient = null!;
            var acceptedConnection = new IPEndPoint(IPAddress.Any, clientPort);
            var request = new NetworkMessage() { Type = MessageType.Handshake, Name = player };

            try
            {
                tcpClient?.Dispose();
                udpClient = new UdpClient(acceptedConnection);
                var responseData = await SendHandshakeAsync(udpClient, request, token);
                tcpClient = new TcpClient(acceptedConnection);
                await tcpClient.ConnectAsync(responseData.connection, token);

                return responseData.message;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                udpClient?.Dispose();
            }
        }

        private async Task<(NetworkMessage message, IPEndPoint connection)> SendHandshakeAsync(UdpClient client, NetworkMessage request, CancellationToken token)
        {
            var broadcastConnection = new IPEndPoint(IPAddress.Broadcast, serverPort);
            var requestBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(request));

            await client.SendAsync(requestBytes, broadcastConnection, token);
            var responseData = await client.ReceiveAsync(token);

            var message = JsonSerializer.Deserialize<NetworkMessage>(Encoding.UTF8.GetString(responseData.Buffer));
            if (message is null)
                throw new InvalidDataException();

            return (message, responseData.RemoteEndPoint);
        }

        public async Task SendMessage(NetworkMessage message)
        {
            try
            {
                if (tcpClient != null)
                {
                    var stream = tcpClient.GetStream();
                    var messageBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                    await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task ReceiveMessage(CancellationToken token = default)
        {
            /*if (tcpClient is null)
                throw new InvalidOperationException();

            var stream = tcpClient.GetStream();
            var bytes = new byte[256];
            while (true)
            {
                await stream.ReadAsync(bytes, 0, bytes.Length);
                var message = JsonSerializer.Deserialize<NetworkMessage>(Encoding.UTF8.GetString(bytes));

                if(message != null)
                    OnMessageReceive?.Invoke(this, new MessageArgs(message));
            }*/

            try
            {
                if(tcpClient != null)
                {
                    var stream = tcpClient.GetStream();
                    var bytes = new byte[256];
                    await stream.ReadAsync(bytes, 0, bytes.Length, token);

                    var end = bytes.Length;

                    for (int i = 0; i < bytes.Length; i++)
                    {
                        if (bytes[i] == 0)
                        {
                            end = i;
                            break;
                        }
                    }

                    var message = JsonSerializer.Deserialize<NetworkMessage>(Encoding.UTF8.GetString(bytes, 0, end));

                    if (message is null)
                        throw new InvalidDataException();

                    OnMessageReceive?.Invoke(this, new MessageArgs(message));   
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Close()
        {
            //tcpClient?.GetStream().Close();
            tcpClient?.Dispose();
        }
    }

    public class NetworkMessage
    {
        public MessageType Type { get; set; }
        public string Name { get; set; } = "";
        public Player Mark { get; set; }
        public int X { get; set; } = -1;
        public int Y { get; set; } = -1;
    }

    public class MessageArgs : EventArgs
    {
        public NetworkMessage NetworkMessage { get; init; }

        public MessageArgs(NetworkMessage networkMessage)
        {
            NetworkMessage = networkMessage;
        }
    }

    public enum MessageType
    {
        Handshake,
        NextMove,
        ConnectionClosed,
    }
}
