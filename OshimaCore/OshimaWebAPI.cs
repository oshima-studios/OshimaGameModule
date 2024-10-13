using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Exception;
using Oshima.FunGame.OshimaModules;

namespace Oshima.Core.WebAPI
{
    public class OshimaWebAPI : WebAPIPlugin
    {
        public override string Name => OshimaGameModuleConstant.WebAPI;

        public override string Description => OshimaGameModuleConstant.Description;

        public override string Version => OshimaGameModuleConstant.Version;

        public override string Author => OshimaGameModuleConstant.Author;

        public override void AfterLoad(params object[] objs)
        {
            base.AfterLoad(objs);
        }

        protected override bool BeforeLoad(params object[] objs)
        {
            if (objs.Length > 0 && objs[0] is Dictionary<string, object> delegates)
            {
                if (delegates.TryGetValue("WriteLine", out object? value) && value is Action<string> a)
                {
                    WriteLine = a;
                }
                if (delegates.TryGetValue("Error", out value) && value is Action<Exception> e)
                {
                    Error = e;
                }
            }
            return true;
        }

        public Action<string> WriteLine { get; set; } = new Action<string>(Console.WriteLine);
        public Action<Exception> Error { get; set; } = new Action<Exception>(e => Console.WriteLine(e.GetErrorInfo()));
    }
}
