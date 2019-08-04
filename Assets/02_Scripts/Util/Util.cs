using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

using STExtensions;
using Newtonsoft.Json;


#if UNITY_EDITOR
using UnityEditor;
#endif

using Random = UnityEngine.Random;

namespace Giant
{
	public class Util
	{
//		public static string GetHttpUrl(string url)
//		{
//			if (url.IndexOf("//") >= 0)
//				return url;
//			return AL.Util.StringFormat(StringFormat.HTTP_URL, url);
//		}

		public static Transform FindChildRecursively(Transform transform, string name)
		{
			Transform child = transform.Find(name);
			if (child != null)
				return child;
			for (int i = 0; i < transform.childCount; ++i)
			{
				child = FindChildRecursively(transform.GetChild(i), name);
				if (child != null)
					return child;
			}
			return null;
		}

		public static GameObject Instantiate(UnityEngine.Object prefab, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent)
		{
			GameObject obj = UnityEngine.Object.Instantiate(prefab) as GameObject;
			return GameObjectInit(obj, pos, rot, scale, parent);
		}

		public static GameObject GameObjectInit(GameObject obj, Vector3 pos, Quaternion rot, Vector3 scale, Transform parent)
		{
			Transform objTransform = obj.transform;
			objTransform.SetParent(parent);
			objTransform.localPosition = pos;
			objTransform.localRotation = rot;
			objTransform.localScale = scale;
			return obj;
		}

		/// <summary>
		/// Makes the item.
		/// </summary>
		/// <param name="item"> 이미 생성된 item이면 다시 생성하지 않고 item을 return 한다. </param>
		public static T MakeItem<T>(UnityEngine.Object prefab, Transform parent, bool instantiateInWorldSpace, ref T item)
		{
			if (item != null)
				return item;

			item = MakeItem<T>(prefab, parent, instantiateInWorldSpace);
			return item;
		}
			
		public static T MakeItem<T>(string prefabPath, Transform parent, bool instantiateInWorldSpace)
		{
			UnityEngine.Object prefab = Resources.Load(prefabPath);
			return MakeItem<T>(prefab, parent, instantiateInWorldSpace);
		}

		public static T MakeItem<T>(UnityEngine.Object prefab, Transform parent, bool instantiateInWorldSpace)
		{
			GameObject obj = MakeItem(prefab, parent, instantiateInWorldSpace);
			T item = obj.GetComponent<T>();
			return item;
		}

		public static GameObject MakeItem(UnityEngine.Object prefab, Transform parent, bool instantiateInWorldSpace)
		{
			return UnityEngine.Object.Instantiate(prefab, parent, instantiateInWorldSpace) as GameObject;
		}

		public static T MakeItem<T>(T prefab, Transform parent, bool instantiateInWorldSpace) where T : UnityEngine.Object
		{
			return UnityEngine.Object.Instantiate(prefab, parent, instantiateInWorldSpace) as T;
		}

		public static T MakeListItem<T>(UnityEngine.Object prefab, Transform parent, bool instantiateInWorldSpace, List<T> list)
		{
			T item = MakeItem<T>(prefab, parent, instantiateInWorldSpace);
			list.Add(item);
			return item;
		}
		
		public static void SetFaceSprite(RawImage rawImage, Sprite sprite)
		{
//			SetSprite(rawImage, sprite, ImageFillOrigin.Top, Constant.FaceAspectRatio);
		}

		public static void SetSprite(RawImage rawImage, Sprite sprite)
		{
			SetSprite(rawImage, sprite, 0, 0, 0, 0);
		}

		public static void SetSprite(RawImage rawImage, Sprite sprite, float left, float right, float top, float bottom)
		{
			float textureWidth = sprite.texture.width;
			float textureHeight = sprite.texture.height;
			Rect textureRect = sprite.textureRect;

			Vector2 position = new Vector2(textureRect.position.x / textureWidth, textureRect.position.y / textureHeight);
			Vector2 size = new Vector2(textureRect.size.x / textureWidth, textureRect.size.y / textureHeight);

			float leftMargin = left * size.x;
			float rightMargin = right * size.x;
			float topMargin = top * size.y;
			float bottomMargin = bottom * size.y;

			position += new Vector2(leftMargin, bottomMargin);
			size -= new Vector2(leftMargin + rightMargin, topMargin + bottomMargin);

			rawImage.texture = sprite.texture;
			rawImage.uvRect = new Rect(position, size);
		}

		// 텍스쳐의 ImageFillOrigin 을 기준으로 Sprite 의 텍스쳐보다 RawImage 의 비율이 더 길면 Image 컴포넌트의 Image Type 이 Filled 일 경우와 동일한 방식으로 동작하고,
		// Sprite 보다 RawImage 의 비율이 더 작으면 기준의 되는 변의 가운데를 중심으로 양옆으로 자른 이미지로 보이게 한다.
		public static void SetSprite(RawImage rawImage, Sprite sprite, ImageFillOrigin origin, float unscaledTextureAspect)
		{
			if (sprite == null || sprite.texture == null)
			{
				rawImage.texture = null;
				return;
			}
			
			Vector2 rawImageSize = rawImage.rectTransform.sizeDelta;
			float rawImageAspect = rawImageSize.y / rawImageSize.x / unscaledTextureAspect;

			float textureWidth = sprite.texture.width;
			float textureHeight = sprite.texture.height;
			Rect textureRect = sprite.textureRect;
			float textureAspect = textureRect.height / textureRect.width;

			Vector2 position, size;
			switch (origin)
			{
			case ImageFillOrigin.Left:
				if (textureAspect < rawImageAspect)
				{
					size = new Vector2(textureRect.size.y / rawImageAspect, textureRect.size.y);
					position = new Vector2(textureRect.position.x, textureRect.position.y);
				}
				else
				{
					size = new Vector2(textureRect.size.x, textureRect.size.x * rawImageAspect);
					position = new Vector2(textureRect.position.x, textureRect.position.y + (textureRect.size.y - size.y) * 0.5f);
				}
				break;
			case ImageFillOrigin.Right:
				if (textureAspect < rawImageAspect)
				{
					size = new Vector2(textureRect.size.y / rawImageAspect, textureRect.size.y);
					position = new Vector2(textureRect.position.x + textureRect.size.x - size.x, textureRect.position.y);
				}
				else
				{
					size = new Vector2(textureRect.size.x, textureRect.size.x * rawImageAspect);
					position = new Vector2(textureRect.position.x, textureRect.position.y + (textureRect.size.y - size.y) * 0.5f);
				}
				break;
			case ImageFillOrigin.Bottom:
				if (textureAspect < rawImageAspect)
				{
					size = new Vector2(textureRect.size.y / rawImageAspect, textureRect.size.y);
					position = new Vector2(textureRect.position.x + (textureRect.size.x - size.x) * 0.5f, textureRect.position.y);
				}
				else
				{
					size = new Vector2(textureRect.size.x, textureRect.size.x * rawImageAspect);
					position = new Vector2(textureRect.position.x, textureRect.position.y);
				}
				break;
			case ImageFillOrigin.Top:
				if (textureAspect < rawImageAspect)
				{
					size = new Vector2(textureRect.size.y / rawImageAspect, textureRect.size.y);
					position = new Vector2(textureRect.position.x + (textureRect.size.x - size.x) * 0.5f, textureRect.position.y);
				}
				else
				{
					size = new Vector2(textureRect.size.x, textureRect.size.x * rawImageAspect);
					position = new Vector2(textureRect.position.x, textureRect.position.y - size.y + textureRect.size.y);
				}
				break;
			default:
				size = new Vector2(textureRect.size.x, textureRect.size.y);
				position = new Vector2(textureRect.position.x, textureRect.position.y);
				break;
			}

			rawImage.texture = sprite.texture;
			rawImage.uvRect = new Rect(position.x / textureWidth, position.y / textureHeight, size.x / textureWidth, size.y / textureHeight);
		}

		public static void SetCommonLevel(Text text, int level, int size = -1)
		{
//			if (size < 0)
//				text.text = Datatable.Inst.GetUIText(UITextEnum.COMMON_LEVEL_NOSIZE, level);
//			else
//				text.text = Datatable.Inst.GetUIText(UITextEnum.COMMON_LEVEL, level, size);
		}

		public static void SetCommonLevelName(Text text, int level, int limitBreak, string name, int size)
		{
//			if (limitBreak > 0)
//				text.text = Datatable.Inst.GetUIText(UITextEnum.COMMON_LEVEL_NAME_LIMITBREAK, level, size, name, GlobalDataStore.Inst.GetGameColorString(ColorType.Soul), limitBreak);
//			else
//				text.text = Datatable.Inst.GetUIText(UITextEnum.COMMON_LEVEL_NAME, level, size, name);
		}

//		public static string GetFormattedTime(int second, int size)
//		{
//			float time;
//			return GetFormattedTime(second, size, out time);
//		}

//		public static string GetFormattedTimeSingle(int minute, int size = 10)
//		{
//			string text = "";
//			int day = 60 * 24;
//			int hour = 60;
//
//			if (minute % day == 0)
//			{
//				text = Datatable.Inst.GetUIText(UITextEnum.COMMON_FORMAT_TIME_DD, minute / day, size);	
//			}
//			else if (minute % hour == 0)
//			{
//				text = Datatable.Inst.GetUIText(UITextEnum.COMMON_FORMAT_TIME_HH, minute / hour, size);
//			}
//			else
//			{
//				text = Datatable.Inst.GetUIText(UITextEnum.COMMON_FORMAT_TIME_MM, minute, size);
//			}
//			return text;
//		}

//		public static string GetFormattedTime(int second, int size, out float nextUpdateTerm)
//		{
//			float diff_F = (float)second;
//			long diff = (long)second;
//			long value0, value1;
//
//			string text = "";
//
//			nextUpdateTerm = 0;
//
//			if (diff >= 86400)		// 1일 이상
//			{
//				value0 = diff / 86400;
//				value1 = (diff - value0 * 86400) / 3600;
//				if (value1 == 0)
//					text = Datatable.Inst.GetUIText(UITextEnum.COMMON_FORMAT_TIME_DD, value0, size);
//				else
//					text = Datatable.Inst.GetUIText(UITextEnum.COMMON_FORMAT_TIME_DDHH, value0, value1, size);
//				nextUpdateTerm = (diff_F - (value0 * 86400 + value1 * 3600));
//			}
//			else if (diff >= 3600)	// 1시간 이상
//			{
//				value0 = diff / 3600;
//				value1 = (diff - value0 * 3600) / 60;
//				if (value1 == 0)
//					text = Datatable.Inst.GetUIText(UITextEnum.COMMON_FORMAT_TIME_HH, value0, size);
//				else
//					text = Datatable.Inst.GetUIText(UITextEnum.COMMON_FORMAT_TIME_HHMM, value0, value1, size);
//				nextUpdateTerm = (diff_F - (value0 * 3600 + value1 * 60));
//			}
//			else if (diff >= 60)	// 1분 이상
//			{
//				value0 = diff / 60;
//				value1 = diff - value0 * 60;
//				if (value1 == 0)
//					text = Datatable.Inst.GetUIText(UITextEnum.COMMON_FORMAT_TIME_MM, value0, size);
//				else
//					text = Datatable.Inst.GetUIText(UITextEnum.COMMON_FORMAT_TIME_MMSS, value0, value1, size);
//				nextUpdateTerm = (diff_F - diff);
//			}
//			else
//			{
//				if (diff < 0)
//					diff = 0;
//				
//				text = Datatable.Inst.GetUIText(UITextEnum.COMMON_FORMAT_TIME_SS, diff, size);
//				nextUpdateTerm = (diff_F - diff);
//			}
//
//			return text;
//		}

//		public static bool IsToday(long time)
//		{
//			DateTime dateTime1 = GetServerLocalDateTime(time);
//			DateTime dateTime2 = GetServerLocalDateTime(GiantHandler.Inst.serverTime);
//			return dateTime1.Year == dateTime2.Year && dateTime1.DayOfYear == dateTime2.DayOfYear;
//		}
//
//		public static long GetNextDayServerTime()
//		{
//			DateTime dateTime1 = GetServerLocalDateTime(GiantHandler.Inst.serverTime);
//			DateTime dateTime2 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day);
//			dateTime2 = dateTime2.AddDays(1);
//			TimeSpan timeSpan = dateTime2.Subtract(new DateTime(1970, 1, 1).AddMilliseconds(GiantHandler.Inst.timeOffset));
//			return (long)timeSpan.TotalMilliseconds;
//		}
//
//		public static DateTime GetServerLocalDateTime(long time)
//		{
//			return GetDateTimeFromUnixTime(time + GiantHandler.Inst.timeOffset);
//		}
//
//		public static TimeSpan GetLocalTimeOffset()
//		{
//			DateTime currTime = Giant.Util.GetDateTimeFromUnixTime(GiantHandler.Inst.serverTime);
//			return TimeZone.CurrentTimeZone.GetUtcOffset(currTime);
//		}

		public static DateTime GetDateTimeFromUnixTime(long unixTime)
		{
			DateTime date = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			date = date.AddMilliseconds(unixTime);
			return date;
		}

		public static long GetUnixTimeFromDateTime(DateTime date)
		{
			DateTime epochTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return (long)(date - epochTime).TotalMilliseconds;
		}

		public static int GetDayOfWeekNum(DayOfWeek dayOfWeek)
		{
			int day = 0;
			switch (dayOfWeek)
			{
			case DayOfWeek.Sunday:
				day = 0;
				break;
			case DayOfWeek.Monday:
				day = 1;
				break;
			case DayOfWeek.Tuesday:
				day = 2;
				break;
			case DayOfWeek.Wednesday:
				day = 3;
				break;
			case DayOfWeek.Thursday:
				day = 4;
				break;
			case DayOfWeek.Friday:
				day = 5;
				break;
			case DayOfWeek.Saturday:
				day = 6;
				break;
			}
			return day;
		}

		public static long GetLocalUnixTime()
		{
			var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
			return (long)timeSpan.TotalSeconds;
		}

		public static string GetLocalTimeString()
		{
			DateTime now = DateTime.Now;
			return	AL.Util.StringFormat("{0}{1}{2}{3}{4}{5}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
		}
		
//		public static IEnumerator CheckRemainTime(Text text, long serverEndTime, Action endAction = null)
//		{
//			long remainTime = serverEndTime - GiantHandler.Inst.serverTime;
//			long remainSecond, lastRemainSecond = 0;
//			while (remainTime > 0)
//			{
//				remainSecond = remainTime / 1000;
//
//				if (remainSecond != lastRemainSecond)
//				{
//					text.text = AL.Util.StringFormat(StringFormat.COMMON_REMAIN_TIME, (remainSecond / 60), (remainSecond % 60));
//					lastRemainSecond = remainSecond;
//				}
//
//				yield return null;
//
//				remainTime = serverEndTime - GiantHandler.Inst.serverTime;
//			}
//
//			text.text = AL.Util.StringFormat(StringFormat.COMMON_REMAIN_TIME, 0, 0);
//			
//			if (endAction != null)
//				endAction();
//		}

//		public static string GetBeforeTimeString(long time)
//		{
//			long timeDiff = (GiantHandler.Inst.serverTime - time) / 1000;
//			return GetBeforeTimeStringByDiff(timeDiff);
//		}
//
//		public static string GetBeforeTimeString(long baseTime, long time)
//		{
//			long timeDiff = (baseTime - time) / 1000;
//			return GetBeforeTimeStringByDiff(timeDiff);
//		}
//
//		public static string GetBeforeTimeStringByDiff(long timeDiff)
//		{
//			return Datatable.Inst.GetUIText(UITextEnum.BASE_BATTLE_LOG_BEFORE_TIME, Giant.Util.GetFormattedTime((int)timeDiff, Constant.FormattedTimeSize_S));
//		}

		public static long GetMidNightTime()
		{
			var now = DateTime.UtcNow;
			DateTime midNight = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(1);
			var timeSpan = (midNight -  new DateTime(1970, 1, 1, 0, 0, 0));
			return (long)timeSpan.TotalMilliseconds;
		}

//		public static int GetCompleteCost(long currentMillis, long endMillis, float instantFactor)
//		{
//			float time;
//			return GetCompleteCost(currentMillis, endMillis, instantFactor, out time);
//		}

//		public static int GetCompleteCost(long currentMillis, long endMillis, float instantFactor, out float nextUpdateTerm)
//		{
//			nextUpdateTerm = 0;
//
//			if (currentMillis > endMillis)
//				return 0;
//			
//			long delta = (long)((endMillis - currentMillis) * instantFactor);
//			float diff_F = (float)delta * 0.001f;
//			long diff = (long)diff_F;
//			float cost;
//
//			if (diff >= 86400)		// 1일 이상
//				cost = (((float)(1000 - 260)/(604800 - 86400f)) * (diff - 86400)) + 260;
//			else if (diff >= 3600)	// 1시간 이상
//				cost = (((float)(260 - 20)/(86400- 3600)) * (diff - 3600)) + 20;
//			else if (diff >= 60)	// 1분 이상
//				cost = (((float)(20 - 1)/(3600 - 60)) * (diff - 60)) + 1;
//			else
//				cost = 1;
//
//			nextUpdateTerm = (cost == 1) ? 0.001f * (endMillis - currentMillis) : Constant.CheckUIWaitTime;
//
//			return (int)cost;
//		}

//		public static bool IsSpecialItem(int itemID)
//		{
//			switch (itemID)
//			{
//			case SpecialItemID.BaseExp:
//			case SpecialItemID.Gold:
//			case SpecialItemID.Cash:
//			case SpecialItemID.Ether:
//			case SpecialItemID.Ethereum:
//			case SpecialItemID.DungeonToken:
//			case SpecialItemID.DarkEther:
//			case SpecialItemID.Lumber:
//			case SpecialItemID.Cubic:
//				return true;
//			}
//			return false;
//		}
		
//		public static QuestIconData GetQuestIconData(QUEST_OBJECTIVE objType, int objTarget)
//		{
//			QuestIconData data = new QuestIconData();
//			data.Clear();
//			data.objTarget = objTarget;
//			switch (objType)
//			{
//				// face
//			case QUEST_OBJECTIVE.KILL_MONSTER:
//				data.objCharType = CHARACTER_TYPE.MONSTER;
//				if (Datatable.Inst.dtMonsterData.ContainsKey(objTarget))
//					data.frameIndex = Datatable.Inst.dtMonsterData[objTarget].Grade;
//				data.isFace = true;
//				break;
//			case QUEST_OBJECTIVE.SUMMON_HERO:
//			case QUEST_OBJECTIVE.HERO_LV:
//			case QUEST_OBJECTIVE.HERO_RANK:
//				data.objCharType = CHARACTER_TYPE.HERO;
//				data.isFace = true;
//				break;
//			case QUEST_OBJECTIVE.CLEAR_STAGE:
//				{
//					Datatable.StageEnemiesInfo stageEnemiesInfo;
//					int grade, level;
//					bool isBoss;
//					if (Datatable.Inst.GetStageEnemyLeaderInfo(objTarget, (int)StageDifficulty.Normal, out stageEnemiesInfo, out grade, out level, out isBoss))
//					{
//						data.frameIndex = grade;
//						data.objCharType = CHARACTER_TYPE.MONSTER;
//						data.objTarget = stageEnemiesInfo.EnemyID;
//						data.isBoss =  isBoss;
//					}
//					data.isFace = true;
//				}
//				break;
//
//				// item
//			case QUEST_OBJECTIVE.OWN_ITEM:
//			case QUEST_OBJECTIVE.PROMOTE_ITEM:
//			case QUEST_OBJECTIVE.EXTRACT_ITEM:
//			case QUEST_OBJECTIVE.GATHER_ITEM:
//			case QUEST_OBJECTIVE.MIX_ITEM:
//				if (Datatable.Inst.dtItemBase.ContainsKey(objTarget))
//					data.frameIndex = Datatable.Inst.dtItemBase[objTarget].Tier;
//				data.isItem = true;
//				break;
//
//				// etc
//			case QUEST_OBJECTIVE.HERO_NUMBER:
//				data.iconSpriteType = SpriteType.IconQuest_Hero;
//				break;
//			case QUEST_OBJECTIVE.SEARCH_COUNT:
//				data.iconSpriteType = SpriteType.IconQuest_Search;
//				break;
//			case QUEST_OBJECTIVE.GET_TREASURE:
//				data.iconSpriteType = SpriteType.IconQuest_Treasure;
//				break;
//			case QUEST_OBJECTIVE.PVP_BASE_BATTLE:
//				data.iconSpriteType = SpriteType.IconQuest_Base_Battle;
//				break;
//			case QUEST_OBJECTIVE.PVP_HERO_BATTLE:
//				data.iconSpriteType = SpriteType.IconQuest_PVP_Battle;
//				break;
//			case QUEST_OBJECTIVE.CLEAR_EXTRABOSS:
//				data.iconSpriteType = SpriteType.IconQuest_Extraboss;
//				break;
//			case QUEST_OBJECTIVE.INSCRIPTION:
//				data.iconSpriteType = SpriteType.IconQuest_Inscription;
//				break;
//			}
//			return data;
//		}

		public static QuestIconData GetDailyQuestIconData(DAILY_QUEST_OBJECTIVE objType, int objTarget)
		{
			QuestIconData data = new QuestIconData();
			data.Clear();
			data.objTarget = objTarget;

			switch (objType)
			{
			case DAILY_QUEST_OBJECTIVE.COUNT_SEARCH:
				data.iconSpriteType = SpriteType.Daily_Quest_Icon_Search;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_MONSTER_KILL:
				data.frameIndex = objTarget;
				data.iconSpriteType = SpriteType.Daily_Quest_Icon_Monster_Kill;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_BOSS_KILL:
				data.iconSpriteType = SpriteType.Daily_Quest_Icon_Boss_Kill;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_FEVER_STAR:
				data.iconSpriteType = SpriteType.Daily_Quest_Icon_Fever_Star;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_TREASURE_BOX:
				data.iconSpriteType = SpriteType.Daily_Quest_Icon_Treasure_Box;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_GATHER:
				EP_CATEGORY craftType = (EP_CATEGORY)objTarget;
				if (craftType == (EP_CATEGORY.GATHER_TREE))
					data.iconSpriteType = SpriteType.Daily_Quest_Icon_Gather_Tree;
				else if (craftType == (EP_CATEGORY.GATHER_HERB))
					data.iconSpriteType = SpriteType.Daily_Quest_Icon_Gather_Herb;
				else if (craftType == (EP_CATEGORY.GATHER_VEIN))
					data.iconSpriteType = SpriteType.Daily_Quest_Icon_Gather_Vein;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_GOLD_NPC_SHOP:
				data.iconSpriteType = SpriteType.Daily_Quest_Icon_Gold_Npc_Shop;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_LIKE_UP_NPC:
				data.iconSpriteType = SpriteType.Daily_Quest_Icon_Like_Up;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_FEVER_SEARCH:
				data.iconSpriteType = SpriteType.Daily_Quest_Icon_Fever_Search;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_BUFF_SEARCH:
				SEARCH_BUFF_TYPE buffType = (SEARCH_BUFF_TYPE)objTarget;
				if (buffType == SEARCH_BUFF_TYPE.SB_ELIXIR_TYPE)
					data.iconSpriteType = SpriteType.Daily_Quest_Icon_Search_Buff_Elixer;
				else if (buffType == SEARCH_BUFF_TYPE.SB_SCROLL_TYPE)
					data.iconSpriteType = SpriteType.Daily_Quest_Icon_Search_Buff_Scroll;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_CORE_ENCHANT:
				data.iconSpriteType = SpriteType.Daily_Quest_Icon_Core_Enchant;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_CORE_EVO:
				data.iconSpriteType = SpriteType.Daily_Quest_Icon_Core_Evolution;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_USER_BATTLE:
				EP_CATEGORY category = (EP_CATEGORY)objTarget;
				if (category == EP_CATEGORY.PVP_ARENA_BATTLE)
					data.iconSpriteType = SpriteType.Daily_Quest_Icon_User_Battle_Arena;
				else if (category == EP_CATEGORY.PVP_BASE_BATTLE || category == EP_CATEGORY.PVP_BASE_REVENGE)
					data.iconSpriteType = SpriteType.Daily_Quest_Icon_User_Battle_Base;
				else if (category == EP_CATEGORY.PVP_HERO_BATTLE || category == EP_CATEGORY.PVP_HERO_REVENGE)
					data.iconSpriteType = SpriteType.Daily_Quest_Icon_User_Battle_PVP;
				break;
			case DAILY_QUEST_OBJECTIVE.COUNT_RACE_BATTLE:
				data.iconSpriteType = SpriteType.Daily_Quest_Icon_Race_Battle;
				break;
			}
			return data;
		}

//		public static QuestType GetQuestType(int idQuest)
//		{
//			if (Datatable.Inst.dtSubQuest.ContainsKey(idQuest))
//				return QuestType.SubQuest;
//			if (Datatable.Inst.dtDailyQuest.ContainsKey(idQuest))
//				return QuestType.DailyQuest;
//			return QuestType.None;
//		}
		
		public static IEnumerator ChangeCanvasGroupAlpha(CanvasGroup canvasGroup, float fromAlpha, float toAlpha, float duration)
		{
			float elapsedTime = 0f;
			while (elapsedTime < duration)
			{
				canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / duration);
				yield return null;
				elapsedTime += Time.deltaTime;
			}
			canvasGroup.alpha = toAlpha;
		}
		
		public static void Deselect(BaseEventData data, List<GameObject> selectableObjectLists, Action cb)
		{
			PointerEventData pointerEventData = data as PointerEventData;
			if (pointerEventData != null)
			{
				if (selectableObjectLists.Contains(pointerEventData.pointerCurrentRaycast.gameObject))
					return;
			}
			cb();
		}

		private static char [] separators = new char[] { '{', '}', '=', ',' };
//		public static string GetActiveSkillDesc(Datatable.ActiveSkill activeSkill, int skillLv)
//		{
//			int lv = Mathf.Max(skillLv, 1);
//			string srcDesc = (skillLv >= activeSkill.DescAfterSkillLv) ? activeSkill.DescAfter : activeSkill.Desc;
//			StringBuilder stringBuilder = GetStringBuilder();
//			stringBuilder.Append(srcDesc);
//
//			int startIndex = 0, endIndex;
//			int index = srcDesc.IndexOf('{', startIndex);
//			while (index >= 0)
//			{
//				endIndex = srcDesc.IndexOf('}', startIndex + 1);
//
//				string subString = srcDesc.Substring(index, endIndex - index + 1);
//				if (subString.CompareTo("{obj}") == 0)				// objective
//				{
//					stringBuilder.Replace(subString, Datatable.Inst.GetUIText(UITextEnum.SKILL_OBJECTIVE_START + activeSkill.Objective));
//				}
//				else if (subString.CompareTo("{dmg}") == 0)			// damage
//				{
//					ColorType colorType = ColorType.None;
//					switch ((DAMAGE_TYPE)activeSkill.DamageType)
//					{
//					case DAMAGE_TYPE.PHYSICAL_DAMAGE:	colorType = ColorType.Physical;		break;
//					case DAMAGE_TYPE.MAGICAL_DAMAGE:	colorType = ColorType.Magical;		break;
//					}
//					string damageTypeString = Datatable.Inst.GetUIText(UITextEnum.DAMAGE_TYPE_START + activeSkill.DamageType);
//					stringBuilder.Replace(subString, colorType == ColorType.None ? damageTypeString : AL.Util.StringFormat(StringFormat.COMMON_NAME_COLOR, GlobalDataStore.Inst.GetGameColorString(colorType), damageTypeString));
//				}
//				else if (subString.StartsWith("{f="))				// function
//				{
//					string [] splitStrings = subString.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
//					if (splitStrings.Length >= 3 && splitStrings[0].CompareTo("f") == 0)
//					{
//						int funcIndex = int.Parse(splitStrings[1]);
//						Datatable.ActiveSkillFunction function = Datatable.Inst.FindActiveSkillFunction(activeSkill.ID, funcIndex);
//						if (function != null)
//						{
//							if (splitStrings[2].CompareTo("target") == 0)		// target
//							{
//								if (splitStrings.Length == 3)
//								{
//									stringBuilder.Replace(subString, Datatable.Inst.GetUIText(UITextEnum.SKILL_TARGET_START + function.TargetType));
//								}
//							}
//							else if (splitStrings[2].CompareTo("p") == 0)		// param
//							{
//								if (splitStrings.Length == 5)
//								{
//									bool isValidParam = true;
//									float value = 0;
//									if (splitStrings[3].CompareTo("12") == 0)
//										value = function.Param1 + (lv - 1) * function.Param2;
//									else if (splitStrings[3].CompareTo("34") == 0)
//										value = function.Param3 + (lv - 1) * function.Param4;
//									else
//										isValidParam = false;
//
//									if (isValidParam)
//									{
//										string valueString = GetStringByValueType(splitStrings[4], value);
//										if (!string.IsNullOrEmpty(valueString))
//										{	
//											stringBuilder.Replace(subString, valueString);
//										}
//									}
//								}
//							}
//						}
//					}
//				}
//
//				startIndex = endIndex + 1;
//				index = srcDesc.IndexOf('{', startIndex);
//			}
//
//			return stringBuilder.ToString();
//		}

//		public static string GetPassiveSkillDesc(Datatable.PassiveSkill passiveSkill, int skillLv)
//		{
//			int lv = Mathf.Max(skillLv, 1);
//			string srcDesc = passiveSkill.Desc;
//			StringBuilder stringBuilder = GetStringBuilder();
//			stringBuilder.Append(srcDesc);
//
//			int startIndex = 0, endIndex;
//			int index = srcDesc.IndexOf('{', startIndex);
//			while (index >= 0)
//			{
//				endIndex = srcDesc.IndexOf('}', startIndex + 1);
//
//				string subString = srcDesc.Substring(index, endIndex - index + 1);
//				if (subString.StartsWith("{s="))				// stat
//				{
//					string [] splitStrings = subString.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
//					if (splitStrings.Length >= 3)
//					{
//						int statIndex = int.Parse(splitStrings[1]);
//						if (statIndex == 0 || statIndex == 1)
//						{
//							float value = (statIndex == 0) ? passiveSkill.Stat_0_Base + (lv - 1) * passiveSkill.Stat_0_Up : passiveSkill.Stat_1_Base + (lv - 1) * passiveSkill.Stat_1_Up;
//							if (splitStrings.Length > 3 && splitStrings[3].CompareTo("abs") == 0)
//								value = Mathf.Abs(value);
//							string valueString = GetStringByValueType(splitStrings[2], value);
//							if (!string.IsNullOrEmpty(valueString))
//								stringBuilder.Replace(subString, valueString);
//						}
//					}
//				}
//
//				startIndex = endIndex + 1;
//				index = srcDesc.IndexOf('{', startIndex);
//			}
//
//			return stringBuilder.ToString();
//		}

		public static string GetStringByValueType(string str, float value)
		{
//			bool isTRate = str.CompareTo("trate") == 0;
//			bool isRate = str.CompareTo("rate") == 0;
//			bool isReal = str.CompareTo("real") == 0;
//
//			if (isTRate || isRate || isReal)
//			{
//				if (isTRate)
//					return AL.Util.StringFormat(StringFormat.COMMON_PERCENT_COLOR, GlobalDataStore.Inst.GetGameColorString(ColorType.Rate), (value * 0.1f).ToString(ValueFormat.COMMON_F1));
//				else if (isRate)
//					return AL.Util.StringFormat(StringFormat.COMMON_PERCENT_COLOR, GlobalDataStore.Inst.GetGameColorString(ColorType.Rate), (value * 100f).ToString(ValueFormat.COMMON_F1));
//				else
//					return AL.Util.StringFormat(StringFormat.COMMON_NAME_COLOR, GlobalDataStore.Inst.GetGameColorString(ColorType.Real), value.ToString(ValueFormat.COMMON_F1));
//			}
			return null;
		}

		public static Vector3 GetBoxRewardPosition(int index)
		{
			Vector3 tempVector3 = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0f);
			if (index % 2 == 0)
				return tempVector3 + new Vector3(45f + ((index % 3) * 80f), -160f + ((index / 3) * 80f), 1f);
			return tempVector3 + new Vector3(-45f - ((index % 3) * 80f), -160f + ((index / 3) * 80f), 1f);
		}

//		public static SpriteType GetIconSpriteType(int idItem)
//		{
//			ITEM_TYPE type = Datatable.Inst.GetItemType(idItem);
//
//			switch (type)
//			{
//			case ITEM_TYPE.MASTER_EXP:		return SpriteType.Icon_BaseExp;
//			case ITEM_TYPE.FEVER_STAR:		return SpriteType.Icon_FeverStar_Large;
//			case ITEM_TYPE.GOLD:			return SpriteType.Icon_Gold;
//			case ITEM_TYPE.CASH:			return SpriteType.Icon_Cash;
//			case ITEM_TYPE.ETHER:			return SpriteType.Icon_Ether;
//			case ITEM_TYPE.ETHEREUM:		return SpriteType.Icon_Ethereum;
//			case ITEM_TYPE.DARKETHER:		return SpriteType.Icon_DarkEther;
//			case ITEM_TYPE.LUMBER:			return SpriteType.Icon_Lumber;
//			case ITEM_TYPE.CUBIC:			return SpriteType.Icon_Cubic;
//			case ITEM_TYPE.ARENA_CURRENCY:	return SpriteType.Icon_ArenaCurrency;
//			case ITEM_TYPE.DUNGEON_TOKEN:	return SpriteType.Icon_DungeonToken;
//			case ITEM_TYPE.GUILD_POINT:		return SpriteType.Icon_GuildPoint;
//			default:
//				return SpriteType.None;
//			}
//		}

//		public static SpriteType GetCurrencyCardSpriteType(int idItem, int count)
//		{
//			switch(idItem)
//			{
//			case SpecialItemID.Gold:
//				if (count >= 1000)
//					return SpriteType.IconCardGold_3;
//				else if (count >= 500)
//					return SpriteType.IconCardGold_2;
//				else if (count >= 100)
//					return SpriteType.IconCardGold_1;
//				else
//					return SpriteType.IconCardGold_0;
//			case SpecialItemID.Cash:
//				if (count >= 100)
//					return SpriteType.IconCardCash_1;
//				else
//					return SpriteType.IconCardCash_0;
//			case SpecialItemID.Ether:
//				if (count >= 3000)
//					return SpriteType.IconCardEther_3;
//				else if (count >= 1000)
//					return SpriteType.IconCardEther_2;
//				else if (count >= 300)
//					return SpriteType.IconCardEther_1;
//				else
//					return SpriteType.IconCardEther_0;
//			case SpecialItemID.DarkEther:		return SpriteType.Icon_DarkEther;
//			case SpecialItemID.Lumber:			return SpriteType.Icon_Lumber;
//			case SpecialItemID.Cubic:			return SpriteType.Icon_Cubic;
//			case SpecialItemID.ArenaCurrency: 	return SpriteType.Icon_ArenaCurrency;
//			case SpecialItemID.DungeonToken:	return SpriteType.Icon_DungeonToken;
//
//			default:
//				return SpriteType.None;
//			}
//		}

//		public static bool GetIsHighQualityReward(int idItem, int value)
//		{
//			switch(idItem)
//			{
//			case SpecialItemID.Gold:
//				return value >= Datatable.Inst.settingData.HqRewardOfGold;
//			case SpecialItemID.Cash:
//				return value >= Datatable.Inst.settingData.HqRewardOfCash;
//			case SpecialItemID.Ether:
//				return value >= Datatable.Inst.settingData.HqRewardOfEther;
//
//			default:
//				if (!Datatable.Inst.dtItemBase.ContainsKey(idItem))
//					return false;
//				return Datatable.Inst.dtItemBase[idItem].Tier >= Datatable.Inst.settingData.HqRewardOfItemGrade;
//			}
//		}
//
//		public static string CheckNicknameEmpty(string name)
//		{
//			if (string.IsNullOrEmpty(name))
//				return Constant.NicknameEmptyMark;
//			return name;
//		}
//
//		public static void CheckValidName(string name, TextLimitType type, Action<bool> success)
//		{
//			UITextEnum belowTextEnum = UITextEnum.POPUP_NICKNAME_LENGTH_LIMIT_BELOW;
//			UITextEnum overTextEnum = UITextEnum.POPUP_NICKNAME_LENGTH_LIMIT_OVER;
//			int minNameLength = Constant.MinNickNameLength;
//			int maxNameLength = Constant.MaxNickNameLength;
//
//			switch (type)
//			{
//			case TextLimitType.GuildName:
//				{
//					belowTextEnum = UITextEnum.POPUP_GUILDNAME_LENGTH_LIMIT_BELOW;
//					overTextEnum = UITextEnum.POPUP_GUILDNAME_LENGTH_LIMIT_OVER;
//					minNameLength = Constant.MinGuildNameLength;
//					maxNameLength = Constant.MaxGuildNameLength;
//				}
//				break;
//			}
//
//			if (Datatable.Inst.CheckProhibitedNickSpecial(name))
//			{
//				GlobalSystemRewardMsgHandler.instance.ShowMessage(UITextEnum.POPUP_ALERT_NO_SPECIAL_WORD);
//			}
//			else if(Datatable.Inst.CheckProhibitedNick(name))
//			{
//				GlobalSystemRewardMsgHandler.instance.ShowMessage(UITextEnum.COMMON_PROHIBITED_WORD_ALERT);
//			}
//			else
//			{
//				int length = Giant.Util.GetCharacterLength(name);
//
//				if(length < minNameLength)
//				{
//					GlobalSystemRewardMsgHandler.instance.ShowMessage(belowTextEnum);	
//				}
//				else if(length > maxNameLength)
//				{
//					GlobalSystemRewardMsgHandler.instance.ShowMessage(overTextEnum);	
//				}
//				else
//				{
//					success(true);
//					return;
//				}	
//			}
//			success(false);
//		}

		public static int GetCharacterLength(string str)
		{
			int length = 0;
			char[] charArr = str.ToCharArray();
			foreach(char c in charArr)
			{
				if(char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
				{
					length += 2;
				}
				else
				{
					length++;
				}
			}
			return length;
		}

		public static string GetOSVersion() 
		{
#if !UNITY_EDITOR && UNITY_ANDROID
			using (var version = new AndroidJavaClass("android.os.Build$VERSION")) {
				return version.GetStatic<string>("RELEASE");
			}
#elif !UNITY_EDITOR && UNITY_IOS
			return UnityEngine.iOS.Device.systemVersion;
#endif
			return string.Empty;
		}

		public static void Vibrate()
		{
#if !UNITY_EDITOR && UNITY_ANDROID
			Handheld.Vibrate();
#elif !UNITY_EDITOR && UNITY_IOS
			// check iPad
			if(UnityEngine.iOS.Device.generation.ToString().IndexOf("iPad") < 0)
			{
				Handheld.Vibrate();
			}
#endif
		}

#if UNITY_EDITOR
		public static void RemapShader(GameObject obj)
		{
			if (obj == null)
				return;
			Renderer [] renderers =  obj.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < renderers.Length; ++i)
			{
				Material [] materials = renderers[i].sharedMaterials;
				for (int j = 0; j < materials.Length; ++j)
				{
					if (materials[j] == null || materials[j].shader.Equals(Shader.Find(materials[j].shader.name)))
						continue;
					Debug.LogWarningFormat("RemapShader : {0} ({1} => {2})", materials[j].shader.name, materials[j].shader.GetInstanceID(), Shader.Find(materials[j].shader.name).GetInstanceID());
					materials[j].shader = Shader.Find(materials[j].shader.name);
				}
			}
		}

		public static void CopyFile(string _from, string _to)
		{
			if( File.Exists(_from) )
			{
				File.Copy(_from, _to, true);
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			}
		}

		public static void DeleteFile(string _path)
		{
			if( File.Exists(_path) )
			{
				File.Delete(_path);
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			}
		}

		public static void CreateDirectory(string _path)
		{
			if( !Directory.Exists(_path) )
			{
				Directory.CreateDirectory(_path);
				AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
			}
		}

		public static string GetGameObjectPath(GameObject gameObject)
		{
			StringBuilder sb = Giant.Util.GetStringBuilder();
			sb.AppendFormat("/{0}", gameObject.name);

			while (gameObject.transform.parent != null)
			{
				gameObject = gameObject.transform.parent.gameObject;
				sb.Insert(0, global::Util.StringFormat("/{0}", gameObject.name));
			}

			sb.Remove(0, 1);

			return sb.ToString();
		}
#region Enum Index
		public static int FindEnumIndexByValue(Type type, object enumType)
		{
			System.Array enumValues = System.Enum.GetValues(type);
			if(enumValues != null)
			{
				for(int i = 0; i < enumValues.Length; i++)
				{
					if((int)enumValues.GetValue(i) == (int)enumType)
						return i;
				}
			}
			return -1;
		}

		public static T GetEnumAtIndex<T>(int index)
		{
			System.Array spriteTypes = System.Enum.GetValues(typeof(T));
			if(spriteTypes != null && spriteTypes.Length > index)
				return (T)(spriteTypes.GetValue(index));
			return default(T);
		}
#endregion
#endif

#region RefHandler
		public class RefHandler<T>
		{
			public T value;
			private int refCount;
			private Action action;

			public void Init(T value, Action action)
			{
				this.value = value;
				this.action = action;
			}

			public void IncRef()
			{
				++refCount;
			}

			public void DecRef()
			{
				--refCount;

				if (refCount == 0 && action != null)
				{
					action();
					action = null;
				}
			}

			public int GetRefCount()
			{
				return refCount;
			}
		}
#endregion

#region StringBuilder
		private static StringBuilder stringBuilder = new StringBuilder();

		public static StringBuilder GetStringBuilder()
		{
			stringBuilder.Remove(0, stringBuilder.Length);
			return stringBuilder;
		}
#endregion

#region StatCalculator
//		private static StatCalculator statCalculator = new StatCalculator();
//
//		public static StatCalculator GetStatCalculator()
//		{
//			return statCalculator;
//		}
//
//		public static double GetTargetHeroCombatPoint(DeckInfo targetDeckInfo, int index)
//		{
//			GiantHeroData heroData = targetDeckInfo.GetHeroDataByIndex(index);
//
//			statCalculator.SetData(heroData, null, targetDeckInfo.core, targetDeckInfo.researchSkill, heroData.GetEquipCostumeID());
//			return statCalculator.GetCombatPoint(true);
//		}
//
//		public static double GetMyHeroCombatPoint(GiantHeroData heroData, float statMultiplier = 1f, List<Datatable.WorldBossOption> worldBossOptions = null)
//		{
//			statCalculator.SetData(heroData, null, AccountDataStore.instance.core.inventory, AccountDataStore.instance.researchSkill, heroData.GetEquipCostumeID(), statMultiplier, null, null, worldBossOptions);
//			return statCalculator.GetCombatPoint(true);
//		}
//
//		public static long GetGiantCombatPoint(GiantGiantData giantData, GiantDictionary<int, int> researchSkill, GiantDictionary<int, int> guildResearchSkill = null)
//		{
//			statCalculator.SetData(giantData, researchSkill, guildResearchSkill);
//			return (long)statCalculator.GetCombatPoint(true);
//		}
//
//		public static long GetGiantCombatPoint(GiantSlot giantSlot, GiantDictionary<int, int> guildResearchSkill)
//		{
//			statCalculator.SetData(giantSlot, giantSlot.researchSkill, guildResearchSkill);
//			return (long)statCalculator.GetCombatPoint(true);
//		}
//
//		public static long GetItemCombatPoint(int idItem)
//		{
//			statCalculator.SetItemData(idItem);
//			return (long)statCalculator.GetCombatPoint(true);
//		}
//
//		public static long GetCoreCombatPoint(GiantCoreData coreData)
//		{
//			statCalculator.SetCoreData(coreData);
//			return (long)statCalculator.GetCombatPoint(true);
//		}
//
//		public static long GetCoreCombatWeightPoint(GiantCoreData coreData, GiantHeroData heroData)
//		{
//			GiantCoreData modifiedCore = new GiantCoreData().Clone(coreData);
//			modifiedCore.useHero = heroData.idHero;
//			statCalculator.SetCoreData(modifiedCore);
//			return (long)statCalculator.GetCombatWeightPoint(heroData);
//		}

#endregion

#region File
		public static bool LoadJsonFile(string fileName, object data, bool isDictionary = false)
		{
			string filePath = AL.PersistentStore.GetPath(fileName);
			if (!File.Exists(filePath))
				return false;
			byte[] bytes = AL.Util.ReadFile(filePath);
			if (bytes == null || bytes.Length == 0)
				return false;
#if !UNITY_EDITOR
			bytes = AL.Util.ALEDecode(bytes);
#endif
			string json = AL.Util.Utf8Decode(bytes);
			object obj = AL.Util.JsonDecode(json);
			if (isDictionary)
				AL.Util.GetDictFields((IDictionary)data, obj);
			else
				AL.Util.GetFields(data, obj);
			return true;
		}

		public static void SaveJsonFile(string fileName, object data, bool isiCloudAndiTunesBackup = false)
		{
			string filePath = AL.PersistentStore.GetPath(fileName);
			string json = AL.Util.JsonEncode2(data);
			byte[] bytes =  AL.Util.Utf8Encode(json);
#if !UNITY_EDITOR
			bytes = AL.Util.ALEEncode(bytes);
#endif
			AL.Util.WriteFile(filePath, bytes, isiCloudAndiTunesBackup);
		}
#endregion

		public static string ConvertToPriceMark(string _str)
		{
			int iAddCount = ((_str.Length-1)/3);
			int iInterval = (_str.Length%3).Equals(0) ? 3 : (_str.Length%3);

			int index_ = 0;
			for( int i = 0; i < iAddCount; i++ )
			{
				_str = _str.Insert((iInterval)+(i*3)+index_,",");
				index_++;
			}

			return _str;
		}

//		public static int GetInscriptionSlotId(int slotIndex, int targetLevel)
//		{
//			var enumerator = Datatable.Inst.dtInscriptionSlot.GetEnumerator();
//			while(enumerator.MoveNext())
//			{
//				if(enumerator.Current.Value.SlotNum == slotIndex && enumerator.Current.Value.Lv == targetLevel)
//					return enumerator.Current.Value.Key;
//			}
//			return 0;
//		}
//
//		public static bool CheckInscriptionMaxLv(int inscriptionTargetLevel)
//		{
//			if(inscriptionTargetLevel > Datatable.Inst.settingData.InscriptionMaxLv)
//			{
//				GlobalSystemRewardMsgHandler.instance.ShowMessage(UITextEnum.NPC_INSCRIPTION_LEVEL_MAX_NOTICE);
//				return true;
//			}
//			return false;
//		}

//		private static IEnumerator m_CoreListNotSelectReasonEnumerator = null;
//		public static void CheckNotSelectEnum(int flag)
//		{
//			if(m_CoreListNotSelectReasonEnumerator == null)
//				m_CoreListNotSelectReasonEnumerator = System.Enum.GetValues(typeof(CoreListNotSelectReason)).GetEnumerator();
//			else
//				m_CoreListNotSelectReasonEnumerator.Reset();
//			
//			int index = 0;
//			while(m_CoreListNotSelectReasonEnumerator.MoveNext())
//			{
//				int tempInt = System.Convert.ToInt32(m_CoreListNotSelectReasonEnumerator.Current);
//				if((flag & tempInt) != 0)
//				{
//					GlobalSystemRewardMsgHandler.instance.ShowMessage(UITextEnum.CORE_NOT_SELECT_REASON_START + index);
//					return;
//				}
//				++index;
//			}
//		}
//
//		public static PlayerDataFieldType ConvertIDItemToPlayerDataFieldType(int idItem)
//		{
//			switch(idItem)
//			{
//			case SpecialItemID.Gold:
//				return PlayerDataFieldType.Gold;
//			case SpecialItemID.Cash:
//				return PlayerDataFieldType.Cash;
//			case SpecialItemID.Ether:
//				return PlayerDataFieldType.Ether;
//			case SpecialItemID.Ethereum:
//				return PlayerDataFieldType.Ethereum;
//			case SpecialItemID.DungeonToken:
//				return PlayerDataFieldType.DungeonToken;
//			case SpecialItemID.DarkEther:
//				return PlayerDataFieldType.DarkEther;
//			case SpecialItemID.Lumber:
//				return PlayerDataFieldType.Lumber;
//			case SpecialItemID.Cubic:
//				return PlayerDataFieldType.Cubic;
//			case SpecialItemID.ArenaCurrency:
//				return PlayerDataFieldType.ArenaCurrency;
//			case SpecialItemID.GuildPoint:
//				return PlayerDataFieldType.GuildPoint;
//			case SpecialItemID.GuildCoin:
//				return PlayerDataFieldType.GuildCoin;
//			}
//			return PlayerDataFieldType.None;
//		}
		
//		public static RESOURCE ConvertIDItemToResource(int idItem)
//		{
//			switch (idItem)
//			{
//			case SpecialItemID.DarkEther:
//				return RESOURCE.DARKETHER;
//			case SpecialItemID.Lumber:
//				return RESOURCE.LUMBER;
//			case SpecialItemID.Cubic:
//				return RESOURCE.CUBIC;
//			}
//			return RESOURCE.ENDINDEX;
//		}

		public static ColorType GetStatColorType(CHILD_STAT childStat)
		{
			switch (childStat)
			{
			case CHILD_STAT.PHYSICAL:
			case CHILD_STAT.PHYSICAL_DEF:
			case CHILD_STAT.PHYSICAL_CRITICAL_CHANCE:
				return ColorType.Physical;
			case CHILD_STAT.MAGICAL:
			case CHILD_STAT.MAGICAL_DEF:
			case CHILD_STAT.MAGICAL_CRITICAL_CHANCE:
				return ColorType.Magical;
			}
			return ColorType.Normal;
		}

		public static void OpenURL(string url)
		{
			Application.OpenURL(url);
		}

		public static PanelType ConvertStringToPanelType(string panelName)
		{
			string[] tempNames = Enum.GetNames(typeof(PanelType));
			for(int i = 0; i < tempNames.Length; ++i)
			{
				if(tempNames[i].Equals(panelName))
					return (PanelType)i;
			}
			Debug.LogWarning("invalid panel name : " + panelName);
			return PanelType.EndIndex;

		}

//		public static string GetCoreEnchantString(int enchant)
//		{
//			if(enchant == 0)
//				return string.Empty;
//			else if(enchant < Datatable.Inst.settingData.CoreMaxEnchantLevel)
//				return AL.Util.StringFormat(StringFormat.NUMBER_PLUS, enchant);
//			else
//				return AL.Util.StringFormat(StringFormat.COMMON_NAME_COLOR, GlobalDataStore.Inst.GetGameColorString(ColorType.Complete), Datatable.Inst.GetUIText(UITextEnum.COMMON_MAX));
//		}
//
//		public static int GetRankFrame(int rank)
//		{
//			if (rank <= Constant.HeroFrame1_RankMax)
//				return 1;
//			if (rank <= Constant.HeroFrame2_RankMax)
//				return 2;
//			if (rank <= Constant.HeroFrame3_RankMax)
//				return 3;
//			if (rank <= Constant.HeroFrame4_RankMax)
//				return 4;
//			if (rank <= Constant.HeroFrame5_RankMax)
//				return 5;
//			return 6;
//		}

		public static ColorType GetRankColor(int rank)
		{
			switch (rank)
			{
			case 1:
				return ColorType.Gold;
			case 2:
				return ColorType.Rank1;
			case 3:
				return ColorType.BaseExp;
			default:
				return ColorType.Highlight;
			}	
		}

//		public static string GetRankFormat(int rank)
//		{
//			if (rank < 1)
//				return Datatable.Inst.GetUIText(UITextEnum.COMMON_RANKING_FORMAT, Constant.NoneRanking);
//			return Datatable.Inst.GetUIText(UITextEnum.COMMON_RANKING_FORMAT, rank);
//		}
//
//		public static int GetRankValue(int rank)
//		{
//			if (rank <= Constant.HeroFrame1_RankMax)
//				return rank;
//			if (rank <= Constant.HeroFrame2_RankMax)
//				return rank - Constant.HeroFrame1_RankMax;
//			if (rank <= Constant.HeroFrame3_RankMax)
//				return rank - Constant.HeroFrame2_RankMax;
//			if (rank <= Constant.HeroFrame4_RankMax)
//				return rank - Constant.HeroFrame3_RankMax;
//			if (rank <= Constant.HeroFrame5_RankMax)
//				return rank - Constant.HeroFrame4_RankMax;
//			return rank - Constant.HeroFrame5_RankMax;
//		}
//
//		public static bool IsOnPurchaseTerms()
//		{
//#if !UNITY_EDITOR && !HIDDEN_LOGIN_TEST
//			return DeviceSaveManager.Inst.IsCurrentLanguageEqual(SystemLanguage.Korean) && HiveManager.Inst.ConnectCountry.Equals("KR");
//#else
//			return DeviceSaveManager.Inst.IsCurrentLanguageEqual(SystemLanguage.Korean);
//#endif
//		}

		public static char CheckValidateInput(string text, int charIndex, char addedChar) 
		{
			if(!char.IsLetterOrDigit(addedChar))
				addedChar = '\0';
			return addedChar;
		}

		public static void CheckPublicIp(Action<bool> cb)
		{
//			if (!string.IsNullOrEmpty(Datatable.Inst.settingData.PublicIpCheckUrl))
//			{
//				AL.Util.UnityWebRequestGetText(Datatable.Inst.settingData.PublicIpCheckUrl, (json) =>
//				{
//					if (json != null)
//					{
//						IDictionary dic = (IDictionary)AL.Util.JsonDecode(json);
//	
//						if (dic.Contains("ip") && dic["ip"] != null)
//							GiantHandler.Inst.publicIp = (string)dic["ip"];
//					}
//					if (cb != null)
//							cb(json != null);
//				});
//			}
//			else
//			{
//				if (cb != null)
//					cb(false);
//			}
		}

		public static void ApplyFpsGrade()
		{
//			Datatable.SettingData settingData = Datatable.Inst.settingData;
//			int fpsGrade = DeviceSaveManager.Inst.gameSettingData.fpsGrade;
//			int fps = settingData.FpsGrade2;
//			switch (fpsGrade)
//			{
//			case 1:		fps = settingData.FpsGrade1;		break;
//			case 3:		fps = settingData.FpsGrade3;		break;
//			default:	fps = settingData.FpsGrade2;		break;
//			}
//			Application.targetFrameRate = fps;
		}

//		public static string GetRewardTypeNameByIDItem(int idItem)
//		{
//			switch(idItem)
//			{
//			case SpecialItemID.Gold:
//				return RewardTypeName.Gold;
//			case SpecialItemID.Cash:
//				return RewardTypeName.Cash;
//			case SpecialItemID.Ether:
//				return RewardTypeName.Gold;
//			default:
//				return RewardTypeName.Item;	
//			}
//		}

		public static void GetEvoStar(int evo, out int smallStar, out int largeStar)
		{
			if (evo > 0)
			{
				smallStar = evo % 5;
				if (smallStar == 0)
					smallStar = 5;
				largeStar = (evo - 1) / 5;		
			}
			else
			{
				smallStar = 0;
				largeStar = 0;
			}
		}

//		public static List<Datatable.PvpWeeklyReward> GetPvpWeeklyRewardGroup()
//		{
//			List<Datatable.PvpWeeklyReward> groupList = new List<_Datatable.PvpWeeklyReward>();
//
//			var enumerator = Datatable.Inst.dtPvpWeeklyReward.GetEnumerator();
//			while (enumerator.MoveNext())
//				if (groupList.Find(item => item.RewardGroup == enumerator.Current.Value.RewardGroup) == null)
//					groupList.Add(enumerator.Current.Value);
//
//			groupList.Sort((item1, item2) =>
//			{ 
//				return item1.RewardGroup.CompareTo(item2.RewardGroup);
//			});
//			return groupList;
//		}
//
//		public static Datatable.PvpWeeklyReward GetPvpWeeklyRewardByRank(int rank)
//		{
//			List<Datatable.PvpWeeklyReward> groupList = Giant.Util.GetPvpWeeklyRewardGroup();
//			int myGroup = groupList.Max(item => item.RewardGroup);
//			if (Datatable.Inst.dtPvpWeeklyReward.ContainsKey(rank))
//				myGroup = Datatable.Inst.dtPvpWeeklyReward[rank].RewardGroup;
//
//			List<Datatable.PvpWeeklyReward> myGroupData = Datatable.Inst.dtPvpWeeklyReward.Values.Where(item => item.RewardGroup == myGroup).ToList();
//			return myGroupData.First();
//		}

//		public static int GetPvpGradeByPoint(int point)
//		{
//			if (Datatable.Inst.dtPvpGrade[Datatable.Inst.dtPvpGrade.Count].AccumPoint <= point)
//				return Datatable.Inst.dtPvpGrade.Count;
//		
//			for (int i = 1; i < Datatable.Inst.dtPvpGrade.Count; i++)
//			{
//				if (Datatable.Inst.dtPvpGrade[i].AccumPoint <= point && point < Datatable.Inst.dtPvpGrade[i + 1].AccumPoint)
//					return i;
//			}
//			return 1;
//		}
//
//		public static bool IsGuildRefreshError(object error)
//		{
//			if (error.Equals(ServerError.NOT_ENOUGH_GUILD_LEVEL) ||
//				error.Equals(ServerError.MAX_LEVEL_TARGET_GUILD_BUILDING) ||
//				error.Equals(ServerError.ALREADY_GUILD_BATTLE_PLACEMENT_STATUS) ||
//				error.Equals(ServerError.NOT_GIANT_OWNER_GUILD_MEMBER) ||
//				error.Equals(ServerError.SHORTAGE_GUILD_BATTLE_GIANT_COUNT) ||
//				error.Equals(ServerError.NOT_YET_GUILD_BATTLE_GIANT_ALL_DESTROY) ||
//				error.Equals(ServerError.NOT_ENOUGH_GUILD_COIN))
//			{
//				return true;
//			}
//			return false;
//		}

//		public static string GetGuildResourceUnit(int point)
//		{
//			return AL.Util.StringFormat(StringFormat.GUILD_RESOURCE_UNIT, point);
//		}

//		// 100 K 미만이면 수치 그대로, 100 K 이상이면 1000 으로 나눈 값에  " K" 를 붙인다.
//		public static string ConvertGuildUnitString(int value)
//		{
//			if (value < 100000)
//				return value.ToString();
//			return AL.Util.StringFormat(StringFormat.GUILD_UNIT_K, (int)(value / 1000));
//		}

		// 참여한 히어로만 반환한다.
		public static List<int> GetDeckList(int [] gameFormationIds)
		{
			List<int> deck = new List<int>();
			int count = Mathf.Min(gameFormationIds.Length);
			for (int i = 0; i < count; ++i)
			{
				if (gameFormationIds[i] <= 0)
					continue;
				deck.Add(gameFormationIds[i]);
			}
			return deck;
		}

        public static string ConvertCsvFileToJsonObject(string path)
        {

            if (!File.Exists(path))
            {
                Debug.Log(path);
                return null;
            }
            var csv = new List<string[]>();
            var lines = File.ReadAllLines(path);

            foreach (string line in lines)
                csv.Add(line.Split(','));

            var properties = lines[0].Split(',');

            var listObjResult = new List<Dictionary<string, string>>();

            for (int i = 1; i < lines.Length; i++)
            {
                var objResult = new Dictionary<string, string>();
                for (int j = 0; j < properties.Length; j++)
                    objResult.Add(properties[j], csv[i][j]);

                listObjResult.Add(objResult);
            }

            return JsonConvert.SerializeObject(listObjResult);
        }
    }

#if DONT_MUTE_GAMELOG
	public class GameLogger
	{
		private static string logFilePath;
		public static StreamWriter _LogFile;
		public static StreamWriter LogFile() {
			return _LogFile;
		}
		public static void Create() {
			Close();
			if (null == logFilePath)
				logFilePath = Application.persistentDataPath + "/gamelog.txt";
			Debug.Log("gamelog file path: " + logFilePath);
			FileStream f = new FileStream(logFilePath, FileMode.Create);
			_LogFile = new StreamWriter(f);
		}
		public static void Close() {
			if (null == _LogFile)
				return;
			_LogFile.Close();
			_LogFile = null;
		}
		public static string WriteObjects(params object[] args) {
			List<object> l = new List<object>(args);
			l.Insert(0, AL.Util.Millis);
			string s = AL.Util.JsonEncode(l);
			if (LogFile() != null) {
				LogFile().Write(s);
				OperatingSystem os = Environment.OSVersion;
				if (os.Platform != PlatformID.Unix && os.Platform != PlatformID.MacOSX)
					LogFile().Write("\r\n");
				else
					LogFile().Write("\n");
				LogFile().Flush();
			}
			return "Logger> " + s;
		}
		public static string Write(params object[] args) {
			return WriteObjects(args);
		}
	}
#endif
}
