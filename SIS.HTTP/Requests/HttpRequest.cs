namespace SIS.HTTP.Requests
{
    using System;
    using System.Collections.Generic;

    using Common;
    using Enums;
    using Extensions;
    using Headers;
    using Headers.Contracts;

    using Contracts;
    using SIS.HTTP.Exceptions;
    using System.Linq;
    using System.Text.RegularExpressions;

    public class HttpRequest : IHttpRequest
    {
        private const string HostHeaderKey = "Host";

        private const string QueryStringRegexPattern = @"^\?(\w+(=[\w-]*)?(&\w+(=[\w-]*)?)*)?$";

        public HttpRequest(string requestString)
        {
            CoreValidator.ThrowIfNullOrEmpty(requestString, nameof(requestString));

            this.FormData = new Dictionary<string, object>();
            this.QueryData = new Dictionary<string, object>();
            this.Headers = new HttpHeaderCollection();

            this.ParseRequest(requestString);
        }

        public string Path { get; private set; }

        public string Url { get; private set; }

        public Dictionary<string, object> FormData { get; }

        public Dictionary<string, object> QueryData { get; }

        public IHttpHeaderCollection Headers { get; }

        public HttpRequestMethod RequestMethod { get; private set; }

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
            bool parseResult = Enum.TryParse(requestLine[0], out HttpRequestMethod method);

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
                if (headerLine == GlobalConstants.HttpNewLine)
                {
                    break;
                }

                var httpHeaderParams = headerLine.Split(new[] { ": " }, StringSplitOptions.RemoveEmptyEntries);

                string key = httpHeaderParams[0];
                string value = httpHeaderParams[1];

                if (key == HostHeaderKey)
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

            this.ParseRequestParameters(splitRequestContent[splitRequestContent.Length - 1]);
        }
    }
}
