using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace Publisher
{
    public class SocketClient
    {
        public async void StartClient(string HostNameString, string PortNumberString)
        {
            HostName hostName;

            try
            {
                // The server hostname that we will be establishing a connection to. In this example, the server and client are in the same process.
                //                    var hostName = new Windows.Networking.HostName("localhost");
                //           hostName = NetworkInformation.GetHostNames().Where(q => q.Type == HostNameType.Ipv4).First();
                hostName = new HostName(HostNameString);

                // Create the StreamSocket and establish a connection to the echo server.
                using (var streamSocket = new Windows.Networking.Sockets.StreamSocket())
                {

 //                   this.clientListBox.Items.Add("client is trying to connect...");

                    await streamSocket.ConnectAsync(hostName, PortNumberString);

 //                   this.clientListBox.Items.Add("client connected");

                    // Send a request to the echo server.
                    string request = "Hello, World!";
                    using (Stream outputStream = streamSocket.OutputStream.AsStreamForWrite())
                    {
                        using (var streamWriter = new StreamWriter(outputStream))
                        {
                            await streamWriter.WriteLineAsync(request);
                            await streamWriter.FlushAsync();
                        }
                    }

//                    this.clientListBox.Items.Add(string.Format("client sent the request: \"{0}\"", request));

                    // Read data from the echo server.
                    string response;
                    using (Stream inputStream = streamSocket.InputStream.AsStreamForRead())
                    {
                        using (StreamReader streamReader = new StreamReader(inputStream))
                        {
                            response = await streamReader.ReadLineAsync();
                        }
                    }

 //                   this.clientListBox.Items.Add(string.Format("client received the response: \"{0}\" ", response));
                }

//                this.clientListBox.Items.Add("client closed its socket");
            }
            catch (Exception ex)
            {
                Windows.Networking.Sockets.SocketErrorStatus webErrorStatus = Windows.Networking.Sockets.SocketError.GetStatus(ex.GetBaseException().HResult);
//                this.clientListBox.Items.Add(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

    }
}
