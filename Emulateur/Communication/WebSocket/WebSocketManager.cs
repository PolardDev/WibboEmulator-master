using Butterfly.Core;
using Butterfly.Net;
using ConnectionManager;
using System;

namespace Butterfly.Communication.WebSocket
{
    public class WebSocketManager
    {
        public GameSocketManager manager;

        public WebSocketManager(int port, int maxConnections, int connectionsPerIP)
        {
            this.manager = new GameSocketManager();
            this.manager.init(port, maxConnections, connectionsPerIP, new InitialPacketParser());

            this.manager.connectionEvent += new GameSocketManager.ConnectionEvent(this.Manager_connectionEvent);
        }

        private void Manager_connectionEvent(ConnectionInformation connection)
        {
            connection.connectionClose += new ConnectionInformation.ConnectionChange(this.ConnectionChanged);

            ButterflyEnvironment.GetGame().GetClientWebManager().CreateAndStartClient(connection.getConnectionID(), connection);
        }

        private void ConnectionChanged(ConnectionInformation information)
        {
            this.CloseConnection(information);
            information.connectionClose -= new ConnectionInformation.ConnectionChange(this.ConnectionChanged);
        }

        private void CloseConnection(ConnectionInformation Connection)
        {
            try
            {
                ButterflyEnvironment.GetGame().GetClientWebManager().DisposeConnection(Connection.getConnectionID());
                Connection.Dispose();
            }
            catch (Exception ex)
            {
                Logging.LogException((ex).ToString());
            }
        }


        public void Destroy()
        {
            this.manager.Destroy();
        }
    }
}
