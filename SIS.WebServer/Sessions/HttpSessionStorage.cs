namespace SIS.WebServer.Sessions
{
    using System.Collections.Concurrent;
    using SIS.HTTP.Common;
    using SIS.HTTP.Sessions;

    public class HttpSessionStorage
    {
        public const string SessionCookieKey = "SIS_ID";

        private static readonly ConcurrentDictionary<string, IHttpSession> sessions = new ConcurrentDictionary<string, IHttpSession>();

        public static IHttpSession GetSession(string id)
        {
            CoreValidator.ThrowIfNullOrEmpty(id, nameof(id));

            return sessions.GetOrAdd(id, _ => new HttpSession(id));
        }
    }
}
