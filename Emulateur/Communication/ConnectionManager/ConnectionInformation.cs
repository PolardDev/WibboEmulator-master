using SharedPacketLib;
using System;
using System.Net.Sockets;

namespace ConnectionManager
{
    public class ConnectionInformation : IDisposable
    {
        private readonly Socket _dataSocket;
        private readonly string _ip;
        private readonly int _connectionID;
        private bool _isConnected;
        private readonly byte[] _buffer;
        private readonly AsyncCallback _sendCallback;

        public IDataParser parser { get; set; }

        public event ConnectionInformation.ConnectionChange connectionClose;

        public ConnectionInformation(Socket dataStream, int connectionID, IDataParser parser, string ip)
        {
            this.parser = parser;
            this._buffer = new byte[GameSocketManagerStatics.BUFFER_SIZE];

            this._dataSocket = dataStream;
            this._dataSocket.SendTimeout = 1000 * 30;
            this._dataSocket.ReceiveTimeout = 1000 * 30;
            this._dataSocket.SendBufferSize = GameSocketManagerStatics.BUFFER_SIZE;
            this._dataSocket.ReceiveBufferSize = GameSocketManagerStatics.BUFFER_SIZE;

            _sendCallback = _sentData;

            this._ip = ip;
            this._connectionID = connectionID;
        }

        public void startPacketProcessing()
        {
            if (this._isConnected)
                return;
            this._isConnected = true;
            try
            {
                this._dataSocket.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, this._incomingDataPacket, (object)this._dataSocket);
            }
            catch
            {
                this.disconnect();
            }
        }

        public string getIp()
        {
            return this._ip;
        }

        public int getConnectionID()
        {
            return this._connectionID;
        }

        public void disconnect()
        {
            try
            {
                if (this._isConnected)
                {
                    this._isConnected = false;
                    if (this._dataSocket != null)
                    {
                        try
                        {

                            if (this._dataSocket.Connected)
                            {
                                this._dataSocket.Shutdown(SocketShutdown.Both);
                                this._dataSocket.Close();
                            }
                        }
                        catch
                        {
                        }
                        this._dataSocket.Dispose();
                    }
                    if(this.parser != null)
                        this.parser.Dispose();

                    if (this.connectionClose != null)
                        this.connectionClose(this);

                    this.connectionClose = null;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Dispose()
        {
            if (this._isConnected)
            {
                this.disconnect();
            }

            GC.SuppressFinalize(this);
        }

        private void _incomingDataPacket(IAsyncResult iAr)
        {
            if (!_isConnected)
                return;
            int length = 0;
            try
            {
                length = this._dataSocket.EndReceive(iAr);
            }
            catch
            {
                this.disconnect();
                return;
            }

            if (length == 0)
            {
                this.disconnect();
            }
            else
            {
                try
                {
                    //if (length >= GameSocketManagerStatics.BUFFER_SIZE)
                    //{
                        //Console.WriteLine("Reçu Packet taille max du Buffer !");
                    //}
                    //else
                    //{
                        byte[] packet = new byte[length];
                        Array.Copy(this._buffer, packet, length);

                        if (this.parser != null)
                        {
                            this.parser.handlePacketData(packet);
                        }
                    //}
                }
                catch
                {
                    this.disconnect();
                }
                finally
                {
                    try
                    {
                        this._dataSocket.BeginReceive(this._buffer, 0, this._buffer.Length, SocketFlags.None, new AsyncCallback(this._incomingDataPacket), (object)this._dataSocket);
                    }
                    catch
                    {
                        this.disconnect();
                    }
                }
            }
        }

        public void SendData(byte[] packet)
        {
            if (!this._isConnected)
                return;
            try
            {
                this._dataSocket.BeginSend(packet, 0, packet.Length, SocketFlags.None, _sendCallback, null);
            }
            catch
            {
                this.disconnect();
            }
        }

        private void _sentData(IAsyncResult iAr)
        {
            try
            {
                this._dataSocket.EndSend(iAr);
            }
            catch
            {
                this.disconnect();
            }
        }

        public delegate void ConnectionChange(ConnectionInformation information);
    }
}
