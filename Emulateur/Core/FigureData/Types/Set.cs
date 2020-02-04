using System.Collections.Generic;

namespace Butterfly.Core.FigureData.Types
{
    class Set
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public int ClubLevel { get; set; }
        public bool Colorable { get; set; }

        private Dictionary<string, Part> _parts;

        public Set(int id, string gender, int clubLevel, bool colorable)
        {
            this.Id = id;
            this.Gender = gender;
            this.ClubLevel = clubLevel;
            this.Colorable = colorable;

            this._parts = new Dictionary<string, Part>();
        }

        public Dictionary<string, Part> Parts
        {
            get { return this._parts; }
            set { this._parts = value; }
        }
    }
}
