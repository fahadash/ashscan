namespace Extensibility
{
    public interface IUserInfo
    {
        string Mask { get; set; }
        string Host { get; set; }
        string Nick { get; set; }

        string Identd { get; set; }

        bool IsRegistered { get; }
    }
}
