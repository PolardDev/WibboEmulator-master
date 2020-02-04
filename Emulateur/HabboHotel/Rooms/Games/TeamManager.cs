using System.Collections.Generic;

namespace Butterfly.HabboHotel.Rooms.Games
{
    public enum Team
    {
        none = 0,
        red = 1,
        green = 2,
        blue = 3,
        yellow = 4,
    }

    public class TeamManager
    {
        public List<RoomUser> BlueTeam;
        public List<RoomUser> RedTeam;
        public List<RoomUser> YellowTeam;
        public List<RoomUser> GreenTeam;

        public TeamManager()
        {
            BlueTeam = new List<RoomUser>();
            RedTeam = new List<RoomUser>();
            GreenTeam = new List<RoomUser>();
            YellowTeam = new List<RoomUser>();
        }

        public List<RoomUser> GetAllPlayer()
        {
            List<RoomUser> Players = new List<RoomUser>();

            Players.AddRange(this.BlueTeam);
            Players.AddRange(this.RedTeam);
            Players.AddRange(this.GreenTeam);
            Players.AddRange(this.YellowTeam);

            return Players;
        }

        public bool CanEnterOnTeam(Team t)
        {
            if (t.Equals(Team.blue))
                return this.BlueTeam.Count < 5;
            if (t.Equals(Team.red))
                return this.RedTeam.Count < 5;
            if (t.Equals(Team.yellow))
                return this.YellowTeam.Count < 5;
            if (t.Equals(Team.green))
                return this.GreenTeam.Count < 5;
            else
                return false;
        }

        public void AddUser(RoomUser user)
        {
            if (user.team.Equals(Team.blue))
                this.BlueTeam.Add(user);
            else if (user.team.Equals(Team.red))
                this.RedTeam.Add(user);
            else if (user.team.Equals(Team.yellow))
                this.YellowTeam.Add(user);
            else if (user.team.Equals(Team.green))
                this.GreenTeam.Add(user);
        }

        public void OnUserLeave(RoomUser user)
        {
            if (user.team.Equals(Team.blue))
                this.BlueTeam.Remove(user);
            else if (user.team.Equals(Team.red))
                this.RedTeam.Remove(user);
            else if (user.team.Equals(Team.yellow))
                this.YellowTeam.Remove(user);
            else if (user.team.Equals(Team.green))
                this.GreenTeam.Remove(user);
        }
    }
}
