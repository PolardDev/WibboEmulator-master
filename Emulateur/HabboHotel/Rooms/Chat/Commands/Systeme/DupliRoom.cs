using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class DupliRoom : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            int OldRoomId = Room.Id;
            int RoomId;

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                Room.GetRoomItemHandler().SaveFurniture(dbClient);

                dbClient.SetQuery("INSERT INTO `rooms` (`caption`, `owner`, `description`, `model_name`, `icon_bg`, `icon_fg`, `icon_items`, `wallpaper`, `floor`, `landscape`, `allow_hidewall`, `wallthick`, `floorthick`, `allow_rightsoverride`, `allow_hidewireds`)" +
                "SELECT 'Appart " + OldRoomId + " copie', '" + Session.GetHabbo().Username + "', `description`, `model_name`, `icon_bg`, `icon_fg`, `icon_items`, `wallpaper`, `floor`, `landscape`, `allow_hidewall`, `wallthick`, `floorthick`, `allow_rightsoverride`, `allow_hidewireds` FROM rooms WHERE id = '" + OldRoomId + "'; ");
                RoomId = (int)dbClient.InsertQuery();

                dbClient.RunQuery("INSERT INTO `room_models_customs` (`roomid`, `door_x`, `door_y`, `door_z`, `door_dir`, `heightmap`, `murheight`) " +
                    "SELECT '" + RoomId + "', `door_x`, `door_y`, `door_z`, `door_dir`, `heightmap`, `murheight` FROM room_models_customs WHERE roomid = '" + OldRoomId + "'");

                Dictionary<int, int> newItemsId = new Dictionary<int, int>();
                List<int> wiredId = new List<int>();
                List<int> teleportId = new List<int>();

                dbClient.SetQuery("SELECT id, base_item FROM items WHERE room_id = @roomid");
                dbClient.AddParameter("roomid", OldRoomId);
                foreach (DataRow dataRow in dbClient.GetTable().Rows)
                {

                    int OldItemId = Convert.ToInt32(dataRow[0]);
                    int baseID = Convert.ToInt32(dataRow[1]);

                    ItemData Data = null;
                    ButterflyEnvironment.GetGame().GetItemManager().GetItem(baseID, out Data);
                    if (Data == null || Data.IsRare || Data.InteractionType == InteractionType.EXCHANGE || Data.InteractionType == InteractionType.EXTRABOX || Data.InteractionType == InteractionType.LEGENDBOX || Data.InteractionType == InteractionType.BADGEBOX)
                        continue;



                    dbClient.SetQuery("INSERT INTO `items` (`user_id`, `room_id`, `base_item`, `extra_data`, `x`, `y`, `z`, `rot`, `wall_pos`)" +
                        " SELECT '" + Session.GetHabbo().Id + "', '" + RoomId + "', base_item, extra_data, x, y, z, rot, wall_pos FROM items WHERE id = '" + OldItemId + "'");
                    int ItemId = (int)dbClient.InsertQuery();

                    newItemsId.Add(OldItemId, ItemId);

                    if (Data.InteractionType == InteractionType.TELEPORT || Data.InteractionType == InteractionType.ARROW)
                    {
                        teleportId.Add(OldItemId);
                    }

                    if (Data.InteractionType == InteractionType.MOODLIGHT)
                    {
                        dbClient.RunQuery("INSERT INTO `room_items_moodlight` (`item_id`, `enabled`, `current_preset`, `preset_one`, `preset_two`, `preset_three`)" +
                        "SELECT '" + ItemId + "', `enabled`, `current_preset`, `preset_one`, `preset_two`, `preset_three` FROM room_items_moodlight WHERE item_id = '" + OldItemId + "'");
                    }

                    if (WiredUtillity.TypeIsWired(Data.InteractionType))
                    {
                        dbClient.RunQuery("INSERT INTO `wired_items` (`trigger_id`, `trigger_data_2`, `trigger_data`, `all_user_triggerable`, `triggers_item`) " +
                            "SELECT '" + ItemId + "', trigger_data_2, trigger_data, all_user_triggerable, triggers_item FROM wired_items WHERE trigger_id = '" + OldItemId + "'");

                        wiredId.Add(ItemId);
                    }
                }

                foreach(int oldId in teleportId)
                {
                    int newId;
                    if (!newItemsId.TryGetValue(oldId, out newId))
                        continue;

                    dbClient.SetQuery("SELECT tele_two_id FROM tele_links WHERE tele_one_id = '"+ oldId +"'");
                    DataRow rowTele = dbClient.GetRow();
                    if (rowTele == null)
                        continue;

                    int newIdTwo;
                    if (!newItemsId.TryGetValue(Convert.ToInt32(rowTele["tele_two_id"]), out newIdTwo))
                        continue;

                    dbClient.RunQuery("INSERT INTO `tele_links` (`tele_one_id`, `tele_two_id`) VALUES ('" + newId + "', '" + newIdTwo + "');");
                }

                foreach (int id in wiredId)
                {
                    dbClient.SetQuery("SELECT triggers_item FROM wired_items WHERE trigger_id = '" + id + "' AND triggers_item != ''");
                    DataRow wiredRow = dbClient.GetRow();

                    if (wiredRow == null)
                        continue;

                    string triggerItems = "";

                    string OldItem = (string)wiredRow["triggers_item"];

                    if (OldItem.Contains(":"))
                    {
                        foreach (string oldItem in OldItem.Split(';'))
                        {
                            string[] itemData = oldItem.Split(':');
                            if (itemData.Length != 6)
                                continue;

                            int oldId = Convert.ToInt32(itemData[0]);

                            int newId;
                            if (!newItemsId.TryGetValue(Convert.ToInt32(oldId), out newId))
                                continue;

                            triggerItems += newId + ":" + Convert.ToInt32(itemData[1]) + ":" + Convert.ToInt32(itemData[2]) + ":" + Convert.ToDouble(itemData[3]) + ":" + Convert.ToInt32(itemData[4]) + ":" + itemData[5] + ";";
                        }
                    }
                    else
                    {
                        foreach (string oldId in OldItem.Split(';'))
                        {
                            int newId;
                            if (!newItemsId.TryGetValue(Convert.ToInt32(oldId), out newId))
                                continue;

                            triggerItems += newId + ";";
                        }
                    }

                    if(triggerItems.Length > 0)
                        triggerItems = triggerItems.Remove(triggerItems.Length - 1);

                    dbClient.SetQuery("UPDATE wired_items SET triggers_item=@triggeritems WHERE trigger_id = '"+ id +"' LIMIT 1");
                    dbClient.AddParameter("triggeritems", triggerItems);
                    dbClient.RunQuery();
                }

                dbClient.RunQuery("INSERT INTO `bots` (`user_id`, `name`, `motto`, `gender`, `look`, `room_id`, `walk_enabled`, `x`, `y`, `z`, `rotation`, `chat_enabled`, `chat_text`, `chat_seconds`, `is_dancing`, `is_mixchat`) " +
                    "SELECT '" + Session.GetHabbo().Id + "', `name`, `motto`, `gender`, `look`, '" + RoomId + "', `walk_enabled`, `x`, `y`, `z`, `rotation`, `chat_enabled`, `chat_text`, `chat_seconds`, `is_dancing`, `is_mixchat` FROM bots WHERE room_id = '" + OldRoomId + "'");

                dbClient.RunQuery("INSERT INTO `user_pets` (`user_id`, `room_id`, `name`, `race`, `color`, `type`, `expirience`, `energy`, `nutrition`, `respect`, `createstamp`, `x`, `y`, `z`, `have_saddle`, `hairdye`, `pethair`, `anyone_ride`) " +
                    "SELECT '" + Session.GetHabbo().Id + "', '" + RoomId + "', `name`, `race`, `color`, `type`, `expirience`, `energy`, `nutrition`, `respect`, '" + ButterflyEnvironment.GetUnixTimestamp() + "', `x`, `y`, `z`, `have_saddle`, `hairdye`, `pethair`, `anyone_ride` FROM user_pets WHERE room_id = '" + OldRoomId + "'");
            }

            RoomData roomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (roomData == null)
                return;
            Session.GetHabbo().UsersRooms.Add(roomData);
            Session.SendPacket(new FlatCreatedComposer(roomData.Id, "Appart " + OldRoomId + " copie"));
        }
    }
}