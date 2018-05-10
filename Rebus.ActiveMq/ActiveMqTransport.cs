using System;
using System.Threading;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using Apache.NMS.Util;
using Rebus.Messages;
using Rebus.Transport;

namespace Rebus.ApacheActiveMq
{
    internal class ActiveMqTransport : ITransport
    {
        private readonly IActiveMqTransportSettings _activeMqTransportSettings;

        public ActiveMqTransport(IActiveMqTransportSettings activeMqTransportSettings)
        {
            _activeMqTransportSettings = activeMqTransportSettings;
        }

        public string Address => _activeMqTransportSettings.Address;

        public void CreateQueue(string address)
        {
            var factory = new ConnectionFactory(_activeMqTransportSettings.BrokerUri)
            {
                UserName = _activeMqTransportSettings.UserName, 
                Password = _activeMqTransportSettings.Password

            };
            using(var connection = factory.CreateConnection())
            using(var session = connection.CreateSession(AcknowledgementMode.ClientAcknowledge))
            using (var queue = SessionUtil.GetQueue(session, Address))
            {
                if (queue.IsTemporary)
                {
                    throw new ArgumentException("Can only use durrable queues");
                }
            }


            throw new NotImplementedException();
        }

        public async Task<TransportMessage> Receive(ITransactionContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task Send(string destinationAddress, TransportMessage message, ITransactionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
