﻿using Butterfly.Database.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.EffectsInventory
{
    public class EffectsInventoryManager
    {
        private readonly List<int> _effects;
        private readonly List<int> _effectsStaff;

        public List<int> GetEffects()
        {
            return this._effects;
        }

        public EffectsInventoryManager()
        {
            this._effects = new List<int>();
            this._effectsStaff = new List<int>();
        }

        public void Init()
        {
            this._effects.Clear();
            this._effectsStaff.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM systeme_effects ORDER by id ASC");
                DataTable table = dbClient.GetTable();
                if (table == null)
                    return;

                foreach (DataRow dataRow in table.Rows)
                {
                    int EffectId = (int)dataRow["id"];
                    bool OnlyStaff = ButterflyEnvironment.EnumToBool((string)dataRow["only_staff"]);

                    if (OnlyStaff)
                    {
                        if (!this._effectsStaff.Contains(EffectId))
                            this._effectsStaff.Add(EffectId);
                    }
                    else
                    {
                        if (!this._effects.Contains(EffectId))
                            this._effects.Add(EffectId);
                    }
                }
            }
        }

        public bool EffectExist(int EffectId, bool Staff = false)
        {
            if (this._effects.Contains(EffectId))
                return true;
            if (Staff && this._effectsStaff.Contains(EffectId))
                return true;

            return false;
        }
    }
}
