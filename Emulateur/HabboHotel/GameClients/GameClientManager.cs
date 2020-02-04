using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Core;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Users.Messenger;
using ConnectionManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Butterfly.HabboHotel.GameClients
{
    public class GameClientManager
    {

        public ConcurrentDictionary<int, GameClient> _clients;

        public ConcurrentDictionary<string, int> _usernameRegister;
        public ConcurrentDictionary<int, int> _userIDRegister;

        private readonly List<int> _userStaff;


        public int Count
        {
            get
            {
                return this._userIDRegister.Count;
            }
        }

        public GameClientManager()
        {
            this._clients = new ConcurrentDictionary<int, GameClient>();
            this._usernameRegister = new ConcurrentDictionary<string, int>();
            this._userIDRegister = new ConcurrentDictionary<int, int>();
            this._userStaff = new List<int>();
        }

        public List<GameClient> GetStaffUsers()
        {
            List<GameClient> Users = new List<GameClient>();

            foreach(int UserId in this._userStaff)
            {
                GameClient Client = this.GetClientByUserID(UserId);
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                Users.Add(Client);
            }

            return Users;
        }

        public GameClient GetClientByUserID(int userID)
        {
            if (this._userIDRegister.ContainsKey(userID))
            {
                GameClient Client = null;
                if (!TryGetClient(this._userIDRegister[userID], out Client))
                    return null;
                return Client;
            }
            else
                return (GameClient)null;
        }

        public GameClient GetClientByUsername(string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
            {
                GameClient Client = null;
                if (!TryGetClient(_usernameRegister[username.ToLower()], out Client))
                    return null;
                return Client;
            }
            return null;
        }

        public bool UpdateClientUsername(int ClientId, string OldUsername, string NewUsername)
        {
            if (!_usernameRegister.ContainsKey(OldUsername.ToLower()))
                return false;

            _usernameRegister.TryRemove(OldUsername.ToLower(), out ClientId);
            _usernameRegister.TryAdd(NewUsername.ToLower(), ClientId);
            return true;
        }

        public bool TryGetClient(int ClientId, out GameClient Client)
        {
            return this._clients.TryGetValue(ClientId, out Client);
        }

        public string GetNameById(int Id)
        {
            GameClient clientByUserId = this.GetClientByUserID(Id);
            if (clientByUserId != null)
                return clientByUserId.GetHabbo().Username;
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT username FROM users WHERE id = " + Id);
                return queryreactor.GetString();

            }
        }

        public List<GameClient> GetClientsById(Dictionary<int, MessengerBuddy>.KeyCollection users)
        {
            List<GameClient> ClientOnline = new List<GameClient>();
            foreach (int userID in users)
            {
                GameClient client = this.GetClientByUserID(userID);
                if (client != null)
                    ClientOnline.Add(client);
            }

            return ClientOnline;
        }

        public void SendMessageStaff(IServerPacket Packet)
        {
            foreach (int UserId in this._userStaff)
            {
                GameClient Client = this.GetClientByUserID(UserId);
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                Client.SendPacket(Packet);
            }
        }

        public void SendMessage(IServerPacket Packet)
        {
            foreach (GameClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.GetHabbo() == null)
                    continue;

                Client.SendPacket(Packet);
            }
        }

        public void CreateAndStartClient(int clientID, ConnectionInformation connection)
        {
            GameClient Client = new GameClient(clientID, connection);
            if (this._clients.TryAdd(Client.ConnectionID, Client))
                Client.StartConnection();
            else
                connection.Dispose();
        }

        public void DisposeConnection(int clientID)
        {
            GameClient Client = null;
            if (!TryGetClient(clientID, out Client))
                return;

            if (Client != null)
                Client.Dispose();

            this._clients.TryRemove(clientID, out Client);
        }

        public void LogClonesOut(int UserID)
        {
            GameClient clientByUserId = this.GetClientByUserID(UserID);
            if (clientByUserId == null)
                return;
            clientByUserId.Disconnect();
        }

        public void RegisterClient(GameClient client, int userID, string username)
        {
            if (_usernameRegister.ContainsKey(username.ToLower()))
                _usernameRegister[username.ToLower()] = client.ConnectionID;
            else
                _usernameRegister.TryAdd(username.ToLower(), client.ConnectionID);

            if (_userIDRegister.ContainsKey(userID))
                _userIDRegister[userID] = client.ConnectionID;
            else
                _userIDRegister.TryAdd(userID, client.ConnectionID);
        }

        public void UnregisterClient(int userid, string username)
        {
            int Client = 0;
            this._userIDRegister.TryRemove(userid, out Client);
            this._usernameRegister.TryRemove(username.ToLower(), out Client);
        }

        public void AddUserStaff(int UserId)
        {
            if (!this._userStaff.Contains(UserId))
                this._userStaff.Add(UserId);
        }

        public void RemoveUserStaff(int UserId)
        {
            if (this._userStaff.Contains(UserId))
                this._userStaff.Remove(UserId);
        }

        public void CloseAll()
        {
            StringBuilder stringBuilder = new StringBuilder();

            foreach (GameClient client in this.GetClients.ToList())
            {
                if (client == null)
                    continue;

                if (client.GetHabbo() != null)
                {
                    try
                    {
                        stringBuilder.Append(client.GetHabbo().GetQueryString);
                    }
                    catch
                    {
                    }
                }
            }
            try
            {
                if (stringBuilder.Length > 0)
                {
                    using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                        queryreactor.RunQuery((stringBuilder).ToString());
                }
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex, "GameClientManager.CloseAll()");
            }
            Console.WriteLine("Done saving users inventory!");
            Console.WriteLine("Closing server connections...");
            try
            {
                foreach (GameClient client in this.GetClients.ToList())
                {

                    if (client == null || client.GetConnection() == null)
                        continue;
                    try
                    {
                        client.GetConnection().Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogCriticalException((ex).ToString());
            }
            this._clients.Clear();
            Console.WriteLine("Connections closed!");
        }

        public void BanUser(GameClient Client, string Moderator, double LengthSeconds, string Reason, bool IpBan, bool MachineBan)
        {
            if (string.IsNullOrEmpty(Reason))
                Reason = "Ne respect pas les régles";

            string Variable = Client.GetHabbo().Username.ToLower();
            string str = "user";
            double Expire = (double)ButterflyEnvironment.GetUnixTimestamp() + LengthSeconds;
            if (IpBan)
            {
                //Variable = Client.GetConnection().getIp();
                Variable = Client.GetHabbo().IP;
                str = "ip";
            }

            if (MachineBan)
            {
                Variable = Client.MachineId;
                str = "machine";
            }

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("INSERT INTO bans (bantype,value,reason,expire,added_by,added_date) VALUES (@rawvar, @var, @reason, '" + Expire + "', @mod, UNIX_TIMESTAMP())");
                queryreactor.AddParameter("rawvar", str);
                queryreactor.AddParameter("var", Variable);
                queryreactor.AddParameter("reason", Reason);
                queryreactor.AddParameter("mod", Moderator);
                queryreactor.RunQuery();
            }
            if (MachineBan)
            {
                this.BanUser(Client, Moderator, LengthSeconds, Reason, true, false);
            }
            else if (IpBan)
            {
                this.BanUser(Client, Moderator, LengthSeconds, Reason, false, false);
            }
            else
            {
                Client.Disconnect();
            }
        }

        public void SendSuperNotif(string Title, string Notice, string Picture, string Link, string LinkTitle, bool Broadcast, bool Event)
        {
            this.SendMessage(new RoomNotificationComposer(Title, Notice, Picture, LinkTitle, Link));
        }

        public ICollection<GameClient> GetClients
        {
            get
            {
                return this._clients.Values;
            }
        }
    }
}
