using System;
using System.Collections.Concurrent;
using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using Rebus.Extensions;

namespace Rebus.ApacheActiveMq
{
    internal class ActiveMqSession : IDisposable
    {
        private readonly IConnection _connection;
        private readonly ISession _session;
        private bool _disposed = false;

        private readonly ConcurrentDictionary<string, IMessageConsumer> _messageConsumers = new ConcurrentDictionary<string, IMessageConsumer>();

        public ActiveMqSession(IConnectionFactory connectionFactory)
        {
            _connection = connectionFactory.CreateConnection();

            _session = _connection.CreateSession(AcknowledgementMode.ClientAcknowledge);
        }

        public IConnection Connection => _connection;

        public ISession Session => _session;

        public IMessageConsumer CreateQueueMessageConsumer(string queue)
        {
            return _messageConsumers.GetOrAdd(queue, name => Session.CreateConsumer(new ActiveMQQueue(name)));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool dispose)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = dispose;

            _messageConsumers.Values.ForEach(c => c.Dispose());

            _session.Dispose();

            _connection.Dispose();
        }
    }
}