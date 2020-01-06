namespace SIS.HTTP.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Common;


    using Cookies;

    using Enums;

    using Exceptions;

    using Extensions;

    using Headers;
    using SIS.HTTP.Sessions;

    public class HttpRequest : IHttpRequest
    {
        private const string QueryStringRegexPattern = @"^(\w+(=[\w-]*)?(&\w+(=[\w-]*)?)*)?$";

        public HttpRequest(string requestString)
        {
            CoreValidator.ThrowIfNullOrEmpty(requestString, nameof(requestString));

            this.FormData = new Dictionary<string, object>();
            this.QueryData = new Dictionary<string, object>();
            this.Headers = new HttpHeaderCollection();
            this.Cookies = new HttpCookieCollection();

            this.ParseRequest(requestString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public IHttpCookieCollection Cookies { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

        public IHttpSession Session { get; set; }

        private bool IsValidRequestLine(string[] requestLine)
        {
            return requestLine.Length == 3 
                && requestLine[2] == GlobalConstants.HttpOneProtocolFragment;
        }

        private bool IsValidRequestQueryString(string queryString)
        {
            return Regex.IsMatch(queryString, QueryStringRegexPattern);
        }

        private bool HasQueryString()
        {
            return this.Url.Split('?').Length > 1;
        }

        private void ParseRequestMethod(string[] requestLine)
        {
            bool parseResult = Enum.TryParse(requestLine[0].Capitalize(), out HttpRequestMethod method);

            if (!parseResult)
            {
                throw new BadRequestException(string.Format(GlobalConstants.UnsupportedMethodExceptionMessage, requestLine[0]));
            }

            this.RequestMethod = method;
        }

        private void ParseRequestUrl(string[] requestLine)
        {
            this.Url = requestLine[1];
        }

        private void ParseRequestPath()
        {
            this.Path = this.Url
                .Split(new[] { '?' }, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        private void ParseRequestHeaders(string[] requestContent)
        {
            bool isValidRequest = false;

            foreach (var headerLine in requestContent)
            {
                if (headerLine == string.Empty)
                {
                    break;
                }

                var httpHeaderParams = headerLine.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                string key = httpHeaderParams[0];
                string value = httpHeaderParams[1];

                if (key == HttpHeader.Host)
                {
                    isValidRequest = true;
                }

                HttpHeader header = new HttpHeader(key, value);
                this.Headers.AddHeader(header);
            }

            if (!isValidRequest)
            {
                throw new BadRequestException();
            }
        }

        private void ParseCookeis()
        {
            if (this.Headers.ContainsHeader(HttpHeader.Cookie))
            {
                var cookiePairs = this.Headers.GetHeader(HttpHeader.Cookie)
                    .Value.Split(new[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var pair in cookiePairs)
                {
                    var keyValuePair = pair.Split(new[] { '=' }, 2);

                    var httpCookie = new HttpCookie(keyValuePair[0], keyValuePair[1], false);

                    this.Cookies.AddCookie(httpCookie);
                }
            }
        }

        private string[] SplitQueryParameter(string queryParameter)
        {
            string[] keyValuePairParams = queryParameter.Split('=');

            string key = keyValuePairParams[0];

            string value = keyValuePairParams.Length > 1 ? keyValuePairParams[1] : null;

            return new string[] { key, value };
        }

        private void ParseRequestQueryParameters()
        {
            if (this.HasQueryString())
            {
                string queryString = this.Url
                .Split('?')[1];

                if (this.IsValidRequestQueryString(queryString))
                {
                    var queryParameters = queryString.Split('&')
                    .ToList();

                    foreach (var queryParameterKeyValuePair in queryParameters)
                    {
                        string[] keyValueArguments = this.SplitQueryParameter(queryParameterKeyValuePair);

                        this.QueryData.Add(keyValueArguments[0], keyValueArguments[1]);
                    }
                }
            }
        }

        private void ParseRequestFormDataParameters(string requestBody)
        {
            if (!string.IsNullOrEmpty(requestBody))
            {
                if (this.IsValidRequestQueryString(requestBody))
                {
                    var queryParameters = requestBody.Split('&')
                    .ToList();

                    foreach (var queryParameterKeyValuePair in queryParameters)
                    {
                        string[] keyValueArguments = this.SplitQueryParameter(queryParameterKeyValuePair);

                        this.FormData.Add(keyValueArguments[0], keyValueArguments[1]);
                    }
                }
            }
        }

        private void ParseRequestParameters(string formData)
        {
            this.ParseRequestQueryParameters();
            this.ParseRequestFormDataParameters(formData);
        }

        private void ParseRequest(string requestString)
        {
            string[] splitRequestContent = requestString
                .Split(new[] { GlobalConstants.HttpNewLine }, StringSplitOptions.None);

            string[] requestLine = splitRequestContent[0]
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (!this.IsValidRequestLine(requestLine))
            {
                throw new BadRequestException();
            }

            this.ParseRequestMethod(requestLine);
            this.ParseRequestUrl(requestLine);
            this.ParseRequestPath();

            this.ParseRequestHeaders(splitRequestContent.Skip(1).ToArray());
            this.ParseCookeis();

            this.ParseRequestParameters(splitRequestContent[splitRequestContent.Length - 1]);
        }
    }
}
