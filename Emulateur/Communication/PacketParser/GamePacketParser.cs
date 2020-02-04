using Butterfly.HabboHotel.GameClients;
using SharedPacketLib;
using System;
using System.IO;
using Butterfly.Communication.Packets.Incoming;
using Butterfly.Utilities;
using System.Text;
using Buttefly.Communication.Encryption.Crypto.Prng;
using Buttefly.Communication.Encryption;
using Buttefly.Utilities;

namespace Butterfly.Net
{
    public class GamePacketParser : IDataParser, IDisposable, ICloneable
    {
        private GameClient currentClient;

        public event GamePacketParser.HandlePacket OnNewPacket;

        private bool _halfDataRecieved = false;
        private byte[] _halfData = null;

        public GamePacketParser(GameClient me)
        {
            this.currentClient = me;
            this.OnNewPacket = (GamePacketParser.HandlePacket)null;
        }

        public void handlePacketData(byte[] Data, bool deciphered = false)
        {
            try
            {
                if (OnNewPacket == null)
                    return;

                if (Data.Length >= 2)
                {
                    if (Data[0] == 60 && Data[1] == 112)
                    {
                        this.currentClient.GetConnection().SendData(Encoding.Default.GetBytes(GetXmlPolicy()));
                    }
                }

                if (currentClient != null && currentClient.RC4Client != null && !deciphered)
                {
                    //currentClient.RC4Client.Decrypt(ref Data);
                }

                if (this._halfDataRecieved)
                {
                    byte[] FullDataRcv = new byte[this._halfData.Length + Data.Length];
                    Buffer.BlockCopy(this._halfData, 0, FullDataRcv, 0, this._halfData.Length);
                    Buffer.BlockCopy(Data, 0, FullDataRcv, this._halfData.Length, Data.Length);

                    this._halfDataRecieved = false; // mark done this round
                    handlePacketData(FullDataRcv); // repeat now we have the combined array
                    return;
                }

                using (BinaryReader Reader = new BinaryReader(new MemoryStream(Data)))
                {
                    if (Data.Length < 4)
                        return;

                    int MsgLen = HabboEncoding.DecodeInt32(Reader.ReadBytes(4));
                    if (MsgLen <= 0 || MsgLen > (5120 * 2))
                        return;

                    if ((Reader.BaseStream.Length - 4) < MsgLen)
                    {
                        this._halfData = Data;
                        this._halfDataRecieved = true;
                        return;
                    }

                    byte[] Packet = Reader.ReadBytes(MsgLen);

                    using (BinaryReader R = new BinaryReader(new MemoryStream(Packet)))
                    {
                        int Header = HabboEncoding.DecodeInt16(R.ReadBytes(2));

                        byte[] Content = new byte[Packet.Length - 2];
                        Buffer.BlockCopy(Packet, 2, Content, 0, Packet.Length - 2);

                        ClientPacket Message = new ClientPacket(Header, Content);
                        OnNewPacket.Invoke(Message);
                    }

                    if (Reader.BaseStream.Length - 4 > MsgLen)
                    {
                        byte[] Extra = new byte[Reader.BaseStream.Length - Reader.BaseStream.Position];
                        Buffer.BlockCopy(Data, (int)Reader.BaseStream.Position, Extra, 0, ((int)Reader.BaseStream.Length - (int)Reader.BaseStream.Position));
                        
                        handlePacketData(Extra, true);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Packet Error! " + e);
            }
        }

        private static string GetXmlPolicy()
        {
            return "<?xml version=\"1.0\"?>\r\n" +
                          "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                          "<cross-domain-policy>\r\n" +
                          "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                          "</cross-domain-policy>\x0";
        }

        public void Dispose()
        {
            this.OnNewPacket = null;
            GC.SuppressFinalize(this);
        }

        public object Clone()
        {
            return new GamePacketParser(this.currentClient);
        }

        public delegate void HandlePacket(ClientPacket message);
    }
}
