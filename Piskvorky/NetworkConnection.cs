using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Piskvorky
{
    internal class NetworkConnection
    {
        public static int serverPort = 40701;
        public static int clientPort = 40702;

        private TcpClient? tcpClient;

        public async Task<NetworkMessage> RunServerAsync(string playerName, Player playerMark, CancellationToken token)
        {
            var acceptedConnection = new IPEndPoint(IPAddress.Any, serverPort);
            var response = new NetworkMessage() { Name = playerName, Mark = playerMark };
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
            var request = new NetworkMessage() { Name = player };

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

        public async Task SendMessage()
        {
            throw new NotImplementedException();
        }

        public async Task<NetworkMessage> ReceiveMessage()
        {
            throw new NotImplementedException();
        }
    }

    internal class NetworkMessage
    {
        //public MessageType Type { get; set; }

        public string Name { get; set; } = "";
        public Player Mark { get; set; }
        public int X { get; set; } = -1;
        public int Y { get; set; } = -1;
    }

    /*internal enum MessageType
    {
        Handshake,
        NextTurn,
        ConnectionClosed,
    }*/
}
