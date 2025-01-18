using Milimoe.FunGame.Core.Api.Transmittal;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Interface;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Common.Event;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Model;
using Oshima.Core.Constant;

namespace Oshima.FunGame.OshimaModes
{
    public class FastAuto : GameModule, IGamingUpdateInfoEvent
    {
        public override string Name => OshimaGameModuleConstant.FastAuto;
        public override string Description => OshimaGameModuleConstant.Description;
        public override string Version => OshimaGameModuleConstant.Version;
        public override string Author => OshimaGameModuleConstant.Author;
        public override string DefaultMap => OshimaGameModuleConstant.FastAutoMap;
        public override GameModuleDepend GameModuleDepend => OshimaGameModuleConstant.GameModuleDepend;
        public override RoomType RoomType => RoomType.FastAuto;
        public override bool HideMain => false;
        public override int MaxUsers => 12;

        public override void StartGame(Gaming instance, params object[] args)
        {
            try
            {
                DataRequest request = Controller.NewDataRequest(GamingType.Connect);
                request.AddRequestData("un", Session.LoginUserName);
                request.SendRequest();
            }
            catch (Exception e)
            {
                TXTHelper.AppendErrorLog(e.ToString());
            }
        }

        public override void StartUI(params object[] args)
        {
            if (Application.MessageLoop)
            {
                FastAutoUI f = new();
                f.Invoke(f.Show);
            }
            else
            {
                ApplicationConfiguration.Initialize();
                Application.Run(new FastAutoUI());
            }
        }


        public void GamingUpdateInfoEvent(object sender, GamingEventArgs e, Dictionary<string, object> data)
        {
            try
            {
                string msg = (DataRequest.GetDictionaryJsonObject<string>(data, "msg") ?? "").Trim();
                if (msg != "") Controller.WriteLine(msg);
            }
            catch (Exception ex)
            {
                TXTHelper.AppendErrorLog(ex.ToString());
            }
        }
    }
}
