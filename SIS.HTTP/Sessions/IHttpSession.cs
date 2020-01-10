namespace SIS.HTTP.Sessions
{
    public interface IHttpSession
    {
        string Id { get; }

        bool IsNew { get; set; }
        object GetParameter(string name);

        bool ContainsParameter(string name);

        void AddParameter(string name, object parameter);

        void ClearParameters();
    }
}
