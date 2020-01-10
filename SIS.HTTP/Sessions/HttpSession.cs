namespace SIS.HTTP.Sessions
{
    using System.Collections.Generic;
    using SIS.Common;

    public class HttpSession : IHttpSession
    {
        private readonly Dictionary<string, object> sessionParameters;

        public HttpSession(string id)
        {
            this.sessionParameters = new Dictionary<string, object>();
            this.IsNew = true;
            this.Id = id;
        }

        public string Id { get; }

        public bool IsNew { get; set; }

        public void AddParameter(string name, object parameter)
        {
            name.ThrowIfNullOrEmpty(nameof(name));
            parameter.ThrowIfNull(nameof(parameter));

            this.sessionParameters[name]  = parameter;
        }

        public void ClearParameters()
        {
            this.sessionParameters.Clear();
        }

        public bool ContainsParameter(string name)
        {
            name.ThrowIfNullOrEmpty(nameof(name));

            return this.sessionParameters.ContainsKey(name);
        }

        public object GetParameter(string name)
        {
            name.ThrowIfNullOrEmpty(nameof(name));

            object parameter = null;

            if (this.sessionParameters.ContainsKey(name))
            {
                parameter = this.sessionParameters[name];
            }

            return parameter;
        }
    }
}
