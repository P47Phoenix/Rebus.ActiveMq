using System;
using System.Threading.Tasks;
using Apache.NMS;

namespace Rebus.ApacheActiveMq
{
    internal interface IActiveMqSessionHelper
    {
        Task UseSession(Func<ActiveMqSession, Task> action);

        Task<T> UseSession<T>(Func<ActiveMqSession, Task<T>> action);
    }
}