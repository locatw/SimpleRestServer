using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SimpleRestServer
{
    public class RestServer : IDisposable
    {
        private static readonly uint BufferSize = 8192;

        private readonly StreamSocketListener socketListener = null;

        private ServerConfig config = null;

        private Dictionary<string, Func<HttpRequest, HttpResponse>> routingTable = null;

        public RestServer(ServerConfig config)
        {
            this.config = config;
            routingTable = new Dictionary<string, Func<HttpRequest, HttpResponse>>();

            this.socketListener = new StreamSocketListener();
            socketListener.ConnectionReceived += (sender, e) => OnConnectionReceived(e.Socket);
        }

        private void SocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            throw new NotImplementedException();
        }

        public void Route(string uri, Func<HttpRequest, HttpResponse> action)
        {
            routingTable.Add(uri, action);
        }

        public IAsyncAction RunAsync()
        {
            return socketListener.BindServiceNameAsync(config.Port.ToString());
        }

        public void Dispose()
        {
            socketListener.Dispose();
        }

        private async void OnConnectionReceived(StreamSocket socket)
        {
            byte[] requestBytes = null;
            using (IInputStream input = socket.InputStream)
            {
                List<byte[]> byteArrayParts = new List<byte[]>();
                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                bool readCompleted = false;

                while (!readCompleted)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);

                    byteArrayParts.Add(buffer.ToArray());

                    readCompleted = buffer.Length != BufferSize;
                }

                requestBytes = byteArrayParts.SelectMany(x => x).ToArray();
            }

            var request = new HttpRequest(requestBytes);

            Func<HttpRequest, HttpResponse> action = null;
            if (routingTable.TryGetValue(request.Uri, out action))
            {
                var response = action(request);

                WriteResponseAsync(socket, response);
            }
            else
            {
                var response = new HttpResponse(HttpVersion.Version1_1, HttpStatus.NotFound);

                WriteResponseAsync(socket, response);
            }
        }

        private async void WriteResponseAsync(StreamSocket socket, HttpResponse response)
        {
            using (IOutputStream output = socket.OutputStream)
            {
                byte[] responseBytes = response.ToByteArray();

                using (Stream responseStream = output.AsStreamForWrite())
                {
                    await responseStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    await responseStream.FlushAsync();
                }
            }
        }
    }
}