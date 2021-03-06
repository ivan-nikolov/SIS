﻿namespace SIS.MvcFramework
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using SIS.Common;
    using SIS.HTTP.Cookies;
    using SIS.HTTP.Enums;
    using SIS.HTTP.Exceptions;
    using SIS.HTTP.Requests;
    using SIS.HTTP.Responses;
    using SIS.HTTP.Sessions;
    using SIS.MvcFramework.Results;
    using SIS.MvcFramework.Routing;
    using SIS.MvcFramework.Sessions;

    public class ConnectionHandler
    {
        private readonly Socket client;
        private readonly IServerRoutingTable serverRoutingTable;
        private readonly IHttpSessionStorage httpSessionStorage;

        public ConnectionHandler(Socket client, IServerRoutingTable serverRoutingTable, IHttpSessionStorage httpSessionStorage)
        {
            client.ThrowIfNull(nameof(client));
            serverRoutingTable.ThrowIfNull(nameof(serverRoutingTable));
            httpSessionStorage.ThrowIfNull(nameof(httpSessionStorage));

            this.client = client;
            this.serverRoutingTable = serverRoutingTable;
            this.httpSessionStorage = httpSessionStorage;
        }

        public async Task ProcessRequestAsync()
        {
            try
            {
                var httpRequest = await this.ReadRequestAsync();

                if (httpRequest != null)
                {
                    Console.WriteLine($"Processing {httpRequest.RequestMethod} {httpRequest.Path}...");

                    var sessionId = this.SetRequstSession(httpRequest);
                    var httpResponse = this.HandleRequest(httpRequest);
                    this.SetResponseSession(httpResponse, sessionId);

                    await this.PrepareResponseAsync(httpResponse);
                }
            }
            catch (BadRequestException bre)
            {
                await this.PrepareResponseAsync(new TextResult(bre.ToString(), HttpResponseStatusCode.BadRequest));
            }
            catch (Exception e)
            {
                await this.PrepareResponseAsync(new TextResult(e.ToString(), HttpResponseStatusCode.InternalServerError));
            }

            this.client.Shutdown(SocketShutdown.Both);
        }

        private async Task<IHttpRequest> ReadRequestAsync()
        {
            var result = new StringBuilder();
            var data = new ArraySegment<byte>(new byte[1024]);

            while (true)
            {
                int numberOfBytesRead = await this.client.ReceiveAsync(data, SocketFlags.None);

                if (numberOfBytesRead == 0)
                {
                    break;
                }

                var bitesAsString = Encoding.UTF8.GetString(data.Array, 0, numberOfBytesRead);
                result.Append(bitesAsString);

                if (numberOfBytesRead < 1023)
                {
                    break;
                }
            }

            if (result.Length == 0)
            {
                return null;
            }

            return new HttpRequest(result.ToString());
        }

        private IHttpResponse ReturnIfResource(IHttpRequest httpRequest)
        {
            string folderPrefix = "/..";
            string folderPath = "/Resources";
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string requestPath = httpRequest.Path;

            string fullPath = assemblyPath + folderPrefix + folderPath + requestPath;

            if (File.Exists(fullPath))
            {
                byte[] content = File.ReadAllBytes(fullPath);

                return new InlineResourceResult(content, HttpResponseStatusCode.Ok);
            }
            else
            {
                return new TextResult($"Route with method {httpRequest.RequestMethod} and path \"{httpRequest.Path}\" not found.", HttpResponseStatusCode.NotFound);
            }
        }

        private IHttpResponse HandleRequest(IHttpRequest httpRequest)
        {
            if (!this.serverRoutingTable.Contains(httpRequest.RequestMethod, httpRequest.Path))
            {
                return this.ReturnIfResource(httpRequest);
            }

            return this.serverRoutingTable.Get(httpRequest.RequestMethod, httpRequest.Path).Invoke(httpRequest);
        }

        private async Task PrepareResponseAsync(IHttpResponse httpResponse)
        {
            ArraySegment<byte> byteSegments = new ArraySegment<byte>(httpResponse.GetBytes());

            await this.client.SendAsync(byteSegments, SocketFlags.None);
        }

        private string SetRequstSession(IHttpRequest httpRequest)
        {
            string sessionId = null;

            if (httpRequest.Cookies.ContainsCookie(HttpSessionStorage.SessionCookieKey))
            {
                var cookie = httpRequest
                    .Cookies
                    .GetCookie(HttpSessionStorage.SessionCookieKey);

                sessionId = cookie.Value;

                if (this.httpSessionStorage.ContainsSession(sessionId))
                {
                    httpRequest.Session = this.httpSessionStorage.GetSession(sessionId);
                }
            }

            if (httpRequest.Session == null)
            {
                sessionId = Guid.NewGuid().ToString();
                httpRequest.Session = this.httpSessionStorage.GetSession(sessionId);
            }


            return httpRequest.Session?.Id;
        }

        private void SetResponseSession(IHttpResponse httpResponse, string sessionId)
        {
            IHttpSession responseSession = this.httpSessionStorage.GetSession(sessionId);
            
            if (responseSession.IsNew)
            {
                responseSession.IsNew = false;
                httpResponse.AddCookie(new HttpCookie(HttpSessionStorage.SessionCookieKey, sessionId));
            }
        }
    }
}
