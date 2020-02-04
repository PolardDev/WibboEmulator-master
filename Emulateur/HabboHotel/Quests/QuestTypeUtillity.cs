﻿// Type: Butterfly.HabboHotel.Quests.QuestTypeUtillity




namespace Butterfly.HabboHotel.Quests
{
  public class QuestTypeUtillity
  {
    public static string GetString(QuestType type)
    {
      switch (type)
      {
        case QuestType.FURNI_MOVE:
          return "MOVE_ITEM";
        case QuestType.FURNI_ROTATE:
          return "ROTATE_ITEM";
        case QuestType.FURNI_PLACE:
          return "PLACE_ITEM";
        case QuestType.FURNI_PICK:
          return "PICKUP_ITEM";
        case QuestType.FURNI_SWITCH:
          return "SWITCH_ITEM_STATE";
        case QuestType.FURNI_STACK:
          return "STACK_ITEM";
        case QuestType.FURNI_DECORATION_FLOOR:
          return "PLACE_FLOOR";
        case QuestType.FURNI_DECORATION_WALL:
          return "PLACE_WALLPAPER";
        case QuestType.SOCIAL_VISIT:
          return "ENTER_OTHERS_ROOM";
        case QuestType.SOCIAL_CHAT:
          return "CHAT_WITH_SOMEONE";
        case QuestType.SOCIAL_FRIEND:
          return "REQUEST_FRIEND";
        case QuestType.SOCIAL_RESPECT:
          return "GIVE_RESPECT";
        case QuestType.SOCIAL_DANCE:
          return "DANCE";
        case QuestType.SOCIAL_WAVE:
          return "WAVE";
        case QuestType.PROFILE_CHANGE_LOOK:
          return "CHANGE_FIGURE";
        case QuestType.PROFILE_CHANGE_MOTTO:
          return "CHANGE_MOTTO";
        case QuestType.PROFILE_BADGE:
          return "WEAR_BADGE";
        default:
          return "FIND_STUFF";
      }
    }
  }
}