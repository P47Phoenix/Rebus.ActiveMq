namespace Rebus.ApacheActiveMq
{
    public interface IActiveMqTransportSettings
    {
        string Address { get; }
        string BrokerUri { get; }
        string UserName { get; }
        string Password { get; }
    }
}