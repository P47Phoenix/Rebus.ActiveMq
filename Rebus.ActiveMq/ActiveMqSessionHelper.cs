using System;
using System.Threading.Tasks;
using Apache.NMS;
using System.Collections.Concurrent;
using Apache.NMS.ActiveMQ;

namespace Rebus.ApacheActiveMq
{

    internal class ActiveMqSessionHelper : IActiveMqSessionHelper
    {
        private static ConcurrentStack<ActiveMqSession> s_concurrentStack = new ConcurrentStack<ActiveMqSession>();
        private readonly IActiveMqTransportSettings _activeMqTransportSettings;
        private readonly IConnectionFactory _connectionFactory;

        public ActiveMqSessionHelper(IActiveMqTransportSettings activeMqTransportSettings)
        {
            _activeMqTransportSettings = activeMqTransportSettings;
            
            _connectionFactory = new ConnectionFactory(_activeMqTransportSettings.BrokerUri)
            {
                UserName = _activeMqTransportSettings.UserName, 
                Password = _activeMqTransportSettings.Password
            };
        }

        private async Task<ActiveMqSession> GetSession()
        {
            if (s_concurrentStack.TryPop(out ActiveMqSession session))
            {
                return session;
            }

            return new ActiveMqSession(_connectionFactory);
        }

        private async Task ReturnSession(ActiveMqSession activeMqSession)
        {
            s_concurrentStack.Push(activeMqSession);
        }
        

        public async Task UseSession(Func<ActiveMqSession, Task> action)
        {
            var session = await GetSession();
            try
            {
                await action(session);
            }
            finally
            {
                await ReturnSession(session);
            }
        }

        public async Task<T> UseSession<T>(Func<ActiveMqSession, Task<T>> action)
        {
            var session = await GetSession();
            try
            {
                return await action(session);
            }
            finally
            {
                await ReturnSession(session);
            }
        }
    }
}