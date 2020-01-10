namespace SIS.MvcFramework.Sessions
{
    using System.Collections.Concurrent;
    using SIS.Common;
    using SIS.HTTP.Sessions;

    public class HttpSessionStorage : IHttpSessionStorage
    {
        private readonly ConcurrentDictionary<string, IHttpSession> sessions;
        public const string SessionCookieKey = "SIS_ID";

        public HttpSessionStorage()
        {
            this.sessions = new ConcurrentDictionary<string, IHttpSession>();
        }

        public IHttpSession GetSession(string sessionId)
        {
            sessionId.ThrowIfNullOrEmpty(nameof(sessionId));

            return sessions.GetOrAdd(sessionId, _ => new HttpSession(sessionId));
        }

        public bool ContainsSession(string sessionId)
        {
            return this.sessions.ContainsKey(sessionId);
        }
    }
}
