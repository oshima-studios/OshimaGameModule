using System.Data;
using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.Core;
using Oshima.FunGame.OshimaModules;
using Oshima.FunGame.OshimaServers.Service;

namespace Oshima.FunGame.OshimaServers
{
    public class AnonymousServer : GameModuleServer
    {
        public override string Name => OshimaGameModuleConstant.Anonymous;

        public override string Description => OshimaGameModuleConstant.Description;

        public override string Version => OshimaGameModuleConstant.Version;

        public override string Author => OshimaGameModuleConstant.Author;

        public override string DefaultMap => OshimaGameModuleConstant.AnonymousMap;

        public override GameModuleDepend GameModuleDepend => OshimaGameModuleConstant.GameModuleDepend;

        public override bool IsAnonymous => true;

        public static HashSet<AnonymousServer> Instances { get; } = [];

        /// <summary>
        /// 向客户端推送事件
        /// </summary>
        /// <param name="msg"></param>
        public static async Task PushMessageToClients(long qq, string msg)
        {
            AnonymousServer[] servers = [.. Instances];
            foreach (AnonymousServer anonymous in servers)
            {
                try
                {
                    await anonymous.PushMessage(qq, msg);
                }
                catch (Exception e)
                {
                    anonymous.Controller.Error(e);
                    Instances.Remove(anonymous);
                }
            }
        }

        protected IServerModel? _clientModel;

        /// <summary>
        /// 启动匿名服务器
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override bool StartAnonymousServer(IServerModel model)
        {
            // 添加当前单例
            Instances.Add(this);
            Controller.WriteLine($"{model.GetClientName()} 连接至匿名服务器", LogLevel.Info);
            // 接收连接匿名服务器的客户端
            _clientModel = model;
            return true;
        }

        /// <summary>
        /// 关闭匿名服务器
        /// </summary>
        /// <param name="model"></param>
        public override void CloseAnonymousServer(IServerModel model)
        {
            // 移除当前单例
            Instances.Remove(this);
            Controller.WriteLine($"{model.GetClientName()} 从匿名服务器断开", LogLevel.Info);
        }

        public override void AfterLoad(params object[] args)
        {
            Controller.NewSQLHelper();
            Controller.NewMailSender();
            OSMCore.InitOSMCore();
            FunGameService.InitFunGame();
            FunGameSimulation.InitFunGameSimulation();
        }

        /// <summary>
        /// 向客户端推送事件
        /// </summary>
        /// <param name="msg"></param>
        public async Task PushMessage(long qq, string msg)
        {
            if (_clientModel != null)
            {
                Dictionary<string, object> data = [];
                data.Add(nameof(qq), qq);
                data.Add(nameof(msg), msg);
                Controller.WriteLine("向客户端推送事件", LogLevel.Debug);
                await SendAnonymousGameServerMessage([_clientModel], data);
            }
        }

        /// <summary>
        /// 接收并处理匿名服务器消息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override async Task<Dictionary<string, object>> AnonymousGameServerHandler(Dictionary<string, object> data)
        {
            Dictionary<string, object> result = [];
            Controller.WriteLine("接收匿名服务器消息", LogLevel.Debug);

            // 根据服务器和客户端的数据传输约定，自行处理 data，并返回。
            string msg = "";
            if (data.Count > 0)
            {
                string command = NetworkUtility.JsonDeserializeFromDictionary<string>(data, "command") ?? "";
                switch (command.Trim().ToLower())
                {
                    case "scadd":
                        msg = SCAdd(data);
                        break;
                    case "sclist":
                        msg = SCList();
                        break;
                    case "att":
                        break;
                    default:
                        msg = "匿名服务器已经收到消息了";
                        break;
                }
                await Task.Delay(1);
            }
            if (msg.Trim() != "")
            {
                result.Add("msg", msg);
            }

            return result;
        }

        public string SCAdd(Dictionary<string, object> data)
        {
            string result = "";

            SQLHelper? sql = Factory.OpenFactory.GetSQLHelper();
            if (sql != null)
            {
                using (sql)
                {
                    try
                    {
                        long qq = NetworkUtility.JsonDeserializeFromDictionary<long>(data, "qq");
                        double sc = NetworkUtility.JsonDeserializeFromDictionary<double>(data, "sc");
                        sql.NewTransaction();
                        sql.Script = "select * from saints where qq = @qq";
                        sql.Parameters.Add("qq", qq);
                        sql.ExecuteDataSet();
                        sql.Parameters.Add("sc", sc);
                        sql.Parameters.Add("qq", qq);
                        if (sql.Success)
                        {
                            sql.Script = "update saints set sc = sc + @sc where qq = @qq";
                        }
                        else
                        {
                            sql.Script = "insert into saints(qq, sc) values(@qq, @sc)";
                        }
                        sql.Execute();
                        if (sql.Success)
                        {
                            Controller.WriteLine($"用户 {qq} 的圣人点数增加了 {sc}", LogLevel.Debug);
                            sql.Commit();
                        }
                        else
                        {
                            sql.Rollback();
                        }
                    }
                    catch (Exception e)
                    {
                        result = e.ToString();
                        sql.Rollback();
                    }
                    finally
                    {
                        sql.Close();
                    }
                }
            }
            else result = "无法调用此接口：SQL 服务不可用。";

            return result;
        }

        public string SCList()
        {
            string result = "☆--- OSMTV 圣人排行榜 TOP10 ---☆\r\n统计时间：" + DateTime.Now.ToString(General.GeneralDateTimeFormatChinese);

            SQLHelper? sql = Controller.SQLHelper;
            if (sql != null)
            {
                sql.Script = "select * from saints order by sc desc";
                sql.ExecuteDataSet();
                if (sql.Success && sql.DataSet.Tables.Count > 0)
                {
                    int count = 0;
                    foreach (DataRow dr in sql.DataSet.Tables[0].Rows)
                    {
                        count++;
                        if (count > 10) break;
                        long qq = Convert.ToInt64(dr["qq"]);
                        double sc = Convert.ToDouble(dr["sc"]);
                        result += $"{count}. 用户：{qq}，圣人点数：{sc} 分\r\n";
                    }
                }
            }
            else result = "无法调用此接口：SQL 服务不可用。";

            return result.Trim();
        }

        public override Task<Dictionary<string, object>> GamingMessageHandler(string username, GamingType type, Dictionary<string, object> data)
        {
            throw new NotImplementedException();
        }

        public override bool StartServer(string GameModule, Room Room, List<User> Users, IServerModel RoomMasterServerModel, Dictionary<string, IServerModel> ServerModels, params object[] Args)
        {
            throw new NotImplementedException();
        }
    }
}
