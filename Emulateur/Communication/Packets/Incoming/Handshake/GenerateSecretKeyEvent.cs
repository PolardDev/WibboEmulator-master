using Buttefly.Communication.Encryption;
using Buttefly.Communication.Encryption.Crypto.Prng;
using Buttefly.Utilities;
using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    class GenerateSecretKeyEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string CipherPublickey = Packet.PopString();

            //BigInteger SharedKey = HabboEncryptionV2.CalculateDiffieHellmanSharedKey(CipherPublickey);
            //if (SharedKey != 0)

                //Session.RC4Client = new ARC4(SharedKey.getBytes());
                Session.SendPacket(new SecretKeyComposer(HabboEncryptionV2.GetRsaDiffieHellmanPublicKey()));

        }
    }
}
