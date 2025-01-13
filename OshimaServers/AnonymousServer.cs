using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules;

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
            if (data.Count > 0)
            {
                await Task.Delay(1);
            }
            result.Add("msg", "匿名服务器已经收到消息了");

            return result;
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
