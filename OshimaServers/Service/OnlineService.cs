using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Models;
using Oshima.FunGame.OshimaServers.Model;

namespace Oshima.FunGame.OshimaServers.Service
{
    public class OnlineService
    {
        public static SemaphoreSlim RoomSemaphoreSlim { get; } = new(1, 1);
        public static SemaphoreSlim HorseRacingSettleSemaphoreSlim { get; } = new(1, 1);
        public static SemaphoreSlim CooperativeSettleSemaphoreSlim { get; } = new(1, 1);
        public static Dictionary<string, bool> GroupsHasHorseRacing { get; } = [];

        public static void GetRoomSemaphoreSlim()
        {
            RoomSemaphoreSlim.Wait(FunGameConstant.SemaphoreSlimTimeout);
        }

        public static void ReleaseRoomSemaphoreSlim()
        {
            if (RoomSemaphoreSlim.CurrentCount == 0)
            {
                RoomSemaphoreSlim.Release();
            }
        }

        public static void GetHorseRacingSettleSemaphoreSlim()
        {
            HorseRacingSettleSemaphoreSlim.Wait(FunGameConstant.SemaphoreSlimTimeout);
        }

        public static void ReleaseHorseRacingSettleSemaphoreSlim()
        {
            if (HorseRacingSettleSemaphoreSlim.CurrentCount == 0)
            {
                HorseRacingSettleSemaphoreSlim.Release();
            }
        }

        public static void GetCooperativeSettleSemaphoreSlim()
        {
            CooperativeSettleSemaphoreSlim.Wait(FunGameConstant.SemaphoreSlimTimeout);
        }

        public static void ReleaseCooperativeSettleSemaphoreSlim()
        {
            if (CooperativeSettleSemaphoreSlim.CurrentCount == 0)
            {
                CooperativeSettleSemaphoreSlim.Release();
            }
        }

        public static Room CreateRoom(User user, string roomType, string password, string groupId, out string msg)
        {
            try
            {
                GetRoomSemaphoreSlim();
                long id = FunGameConstant.Rooms.Count > 0 ? FunGameConstant.Rooms.Values.Max(r => r.Id) + 1 : 1;
                Room room = Factory.GetRoom(id, password: password);
                msg = "";
                if (FunGameConstant.UsersInRoom.TryGetValue(user.Id, out Room? room2) && room2 != null)
                {
                    msg = $"你已经在{room2.Name} [ {room2.Roomid} ] 中了，请先离开房间后再创建房间。";
                }
                else
                {
                    switch (roomType)
                    {
                        case "horseracing":
                            if (GroupsHasHorseRacing.TryGetValue(groupId, out bool has) && has)
                            {
                                msg = $"本群已经存在一个赛马房间！空闲房间会在 {FunGameConstant.RoomExpireTime} 分钟后自动解散，请先等待该房间完成比赛或自动解散。";
                            }
                            else
                            {
                                room.RoomType = RoomType.Custom;
                                room.Name = "赛马房间";
                                room.GameModule = "horseracing";
                                room.GameMap = groupId;
                                room.MaxUsers = 8;
                            }
                            break;
                        case "mix":
                            room.RoomType = RoomType.Mix;
                            room.Name = "混战房间";
                            room.GameModule = "mix";
                            room.GameMap = groupId;
                            room.MaxUsers = 10;
                            break;
                        case "team":
                            room.RoomType = RoomType.Team;
                            room.Name = "团队死斗房间";
                            room.GameModule = "team";
                            room.GameMap = groupId;
                            room.MaxUsers = 8;
                            break;
                        case "cooperative":
                            room.RoomType = RoomType.Custom;
                            room.Name = "共斗房间";
                            room.GameModule = "cooperative";
                            room.GameMap = groupId;
                            room.MaxUsers = 4;
                            break;
                        default:
                            msg = "不支持的房间类型。";
                            break;
                    }
                }
                if (msg == "")
                {
                    room.Roomid = Verification.CreateVerifyCode(VerifyCodeType.MixVerifyCode, 7);
                    while (true)
                    {
                        if (!FunGameConstant.Rooms.ContainsKey(room.Roomid))
                        {
                            break;
                        }
                        room.Roomid = Verification.CreateVerifyCode(VerifyCodeType.MixVerifyCode, 7);
                    }
                    msg = $"房间创建成功，房间号为：{room.Roomid}\r\n注意：房间若在 {FunGameConstant.RoomExpireTime} 分钟后仍处于空闲状态，将自动解散。";
                    if (room.GameModule == "horseracing")
                    {
                        msg += "\r\n在赛马房间内的所有玩家无论是否是房主都可以开始游戏。";
                    }
                    room.RoomMaster = user;
                    room.CreateTime = DateTime.Now;
                }
                if (room.Roomid != "-1")
                {
                    FunGameConstant.Rooms[room.Roomid] = room;
                    if (room.GameModule == "horseracing")
                    {
                        GroupsHasHorseRacing[room.GameMap] = true;
                    }
                }
                return room;
            }
            catch (Exception e)
            {
                msg = $"创建房间失败，错误信息：{e.Message}";
                return Factory.GetRoom();
            }
            finally
            {
                ReleaseRoomSemaphoreSlim();
            }
        }

        public static bool IntoRoom(User user, string roomid, string password, out string msg)
        {
            try
            {
                GetRoomSemaphoreSlim();
                msg = "";
                if (FunGameConstant.Rooms.TryGetValue(roomid, out Room? room) && room != null)
                {
                    if (password == room.Password)
                    {
                        if (FunGameConstant.UsersInRoom.TryGetValue(user.Id, out Room? room2) && room2 != null)
                        {
                            msg = $"你已经在{room2.Name} [ {room2.Roomid} ] 中了，请先退出房间后再加入房间。";
                            return false;
                        }
                        if (room.UserAndIsReady.Count >= room.MaxUsers)
                        {
                            msg = "房间人数已满，无法加入。";
                            return false;
                        }
                        if (room.RoomState != RoomState.Created)
                        {
                            msg = "房间状态异常，无法加入。";
                            return false;
                        }
                        FunGameConstant.UsersInRoom[user.Id] = room;
                        room.UserAndIsReady[user] = true;
                        user.OnlineState = OnlineState.InRoom;
                        msg = $"成功加入{room.Name}：{room.Roomid}\r\n房间人数：{room.UserAndIsReady.Count} / {room.MaxUsers}";
                        return true;
                    }
                    else
                    {
                        msg = "密码错误，无法加入房间。";
                        return false;
                    }
                }
                else
                {
                    msg = "房间不存在，无法加入。";
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = $"加入房间失败，错误信息：{e.Message}";
                return false;
            }
            finally
            {
                ReleaseRoomSemaphoreSlim();
            }
        }

        public static bool QuitRoom(User user, out string msg)
        {
            try
            {
                GetRoomSemaphoreSlim();
                msg = "";
                if (FunGameConstant.UsersInRoom.TryGetValue(user.Id, out Room? room) && room != null)
                {
                    if (room.RoomState != RoomState.Created)
                    {
                        msg = "房间状态异常，无法退出。";
                        return false;
                    }
                    else
                    {
                        FunGameConstant.UsersInRoom.Remove(user.Id);
                        msg = $"成功退出{room.Name}：{room.Roomid}";
                        User[] users = [.. room.UserAndIsReady.Keys.Where(u => u.Id == user.Id)];
                        foreach (User userTemp in users)
                        {
                            room.UserAndIsReady.Remove(userTemp);
                        }
                        if (room.UserAndIsReady.Count == 0)
                        {
                            FunGameConstant.Rooms.Remove(room.Roomid);
                            msg += "，该房间人数为零，已解散该房间。";
                            if (room.GameModule == "horseracing")
                            {
                                GroupsHasHorseRacing[room.GameMap] = false;
                            }
                        }
                        else if (room.RoomMaster.Id == user.Id)
                        {
                            User newRoomMaster = room.UserAndIsReady.Keys.First();
                            room.RoomMaster = newRoomMaster;
                            string newRoomMasterName = newRoomMaster.Username;
                            if (FunGameConstant.UserIdAndUsername.TryGetValue(newRoomMaster.Id, out User? temp) && temp != null)
                            {
                                newRoomMasterName = temp.Username;
                            }
                            msg += $"，新房主为：{newRoomMasterName}。";
                        }
                        return true;
                    }
                }
                else
                {
                    msg = "你当前不在任何房间中。";
                    return false;
                }
            }
            catch (Exception e)
            {
                msg = $"退出房间失败，错误信息：{e.Message}";
                return false;
            }
            finally
            {
                ReleaseRoomSemaphoreSlim();
            }
        }

        public static string RoomInfo(User user)
        {
            string msg = "";
            if (FunGameConstant.UsersInRoom.TryGetValue(user.Id, out Room? room) && room != null)
            {
                string username = "";
                if (FunGameConstant.UserIdAndUsername.TryGetValue(room.RoomMaster.Id, out User? value) && value != null)
                {
                    username = value.Username;
                }
                List<string> users = [];
                foreach (long uid in room.UserAndIsReady.Keys.Select(u => u.Id))
                {
                    if (FunGameConstant.UserIdAndUsername.TryGetValue(uid, out value) && value != null)
                    {
                        users.Add(value.Username);
                    }
                }
                msg += $"☆--- [ {room.Roomid} ] ---☆\r\n房间类型：{room.Name}\r\n创建时间：{room.CreateTime.ToString(General.GeneralDateTimeFormatChinese)}\r\n房主：{username}\r\n" +
                    $"人数：{room.UserAndIsReady.Count} / {room.MaxUsers}\r\n在线玩家：{string.Join("、", users)}\r\n该房间将于 {room.CreateTime.AddMinutes(FunGameConstant.RoomExpireTime).ToString(General.GeneralDateTimeFormatChinese)} 后自动解散，请尽快开局";
            }
            else
            {
                msg = "你当前不在任何房间中。";
            }
            return msg;
        }

        public static string RoomInfo(Room room)
        {
            string username = "";
            if (FunGameConstant.UserIdAndUsername.TryGetValue(room.RoomMaster.Id, out User? value) && value != null)
            {
                username = value.Username;
            }
            string msg = $"房间号：{room.Roomid}\r\n房间类型：{room.Name}\r\n创建时间：{room.CreateTime.ToString(General.GeneralDateTimeFormatChinese)}\r\n房主：{username}\r\n" +
                $"人数：{room.UserAndIsReady.Count} / {room.MaxUsers}";
            return msg;
        }

        public static void RoomsAutoDisband()
        {
            try
            {
                GetRoomSemaphoreSlim();
                Room[] rooms = [.. FunGameConstant.Rooms.Values];
                foreach (Room room in rooms)
                {
                    if (room.RoomState == RoomState.Created && room.CreateTime.AddMinutes(FunGameConstant.RoomExpireTime) < DateTime.Now)
                    {
                        foreach (User user in room.UserAndIsReady.Keys)
                        {
                            FunGameConstant.UsersInRoom.Remove(user.Id);
                        }
                        FunGameConstant.Rooms.Remove(room.Roomid);
                        if (room.GameModule == "horseracing")
                        {
                            GroupsHasHorseRacing[room.GameMap] = false;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                ReleaseRoomSemaphoreSlim();
            }
        }

        public static void ReSetRoomState(string roomid)
        {
            try
            {
                GetRoomSemaphoreSlim();
                if (FunGameConstant.Rooms.TryGetValue(roomid, out Room? room) && room != null)
                {
                    room.CreateTime = DateTime.Now;
                    room.RoomState = RoomState.Created;
                }
            }
            catch { }
            finally
            {
                ReleaseRoomSemaphoreSlim();
            }
        }

        public static async Task<(Room, List<string>)> RunGameAsync(User user)
        {
            Room room = General.HallInstance;
            try
            {
                GetRoomSemaphoreSlim();
                List<string> msgs = [];
                if (FunGameConstant.UsersInRoom.TryGetValue(user.Id, out Room? value) && value != null)
                {
                    room = value;
                    if (room.GameModule != "horseracing" && room.RoomMaster.Id != user.Id)
                    {
                        msgs.Add("你不是房主，无法开始游戏。");
                    }
                    else if (room.RoomState != RoomState.Created)
                    {
                        msgs.Add("房间状态异常，无法开始游戏，已自动解散该房间。");
                        foreach (User userTemp in room.UserAndIsReady.Keys)
                        {
                            FunGameConstant.UsersInRoom.Remove(userTemp.Id);
                        }
                        FunGameConstant.Rooms.Remove(room.Roomid);
                        if (room.GameModule == "horseracing")
                        {
                            GroupsHasHorseRacing[room.GameMap] = false;
                        }
                    }
                    else if (room.UserAndIsReady.Count < 2)
                    {
                        msgs.Add("房间人数不足，无法开始游戏。");
                    }
                    else
                    {
                        room.RoomState = RoomState.Gaming;
                        switch (room.GameModule)
                        {
                            case "horseracing":
                                try
                                {
                                    GetHorseRacingSettleSemaphoreSlim();
                                    Dictionary<long, int> userPoints = HorseRacing.RunHorseRacing(msgs, room);
                                    PluginConfig pc = new("horseracing", room.GameMap);
                                    pc.LoadConfig();
                                    Dictionary<long, int> waitforsettle = pc.Get<Dictionary<long, int>>("points") ?? [];
                                    foreach (long uid in waitforsettle.Keys)
                                    {
                                        int points = waitforsettle[uid];
                                        if (userPoints.ContainsKey(uid))
                                        {
                                            userPoints[uid] += points;
                                        }
                                        else
                                        {
                                            userPoints[uid] = points;
                                        }
                                    }
                                    pc.Add("points", userPoints);
                                    pc.SaveConfig();
                                }
                                catch (Exception e2)
                                {
                                    msgs.Add("Error: " + e2.Message);
                                }
                                finally
                                {
                                    ReleaseHorseRacingSettleSemaphoreSlim();
                                }
                                break;
                            case "cooperative":
                                try
                                {
                                    GetCooperativeSettleSemaphoreSlim();
                                    await Cooperative.RunCooperativeGame(msgs, room);
                                }
                                catch (Exception e2)
                                {
                                    msgs.Add("Error: " + e2.Message);
                                }
                                finally
                                {
                                    ReleaseCooperativeSettleSemaphoreSlim();
                                }
                                room.CreateTime = DateTime.Now;
                                break;
                            default:
                                msgs.Add("游戏已开始！");
                                await Task.Delay(5000);
                                msgs.Add("游戏已结束！");
                                break;
                        }
                    }
                }
                else
                {
                    msgs.Add("你当前不在任何房间中。");
                }
                return (room, msgs);
            }
            catch (Exception e)
            {
                return (room, [$"游戏遇到错误，错误信息：{e.Message}"]);
            }
            finally
            {
                ReleaseRoomSemaphoreSlim();
            }
        }
    }
}
