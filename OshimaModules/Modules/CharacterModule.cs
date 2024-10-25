using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;

namespace Oshima.FunGame.OshimaModules
{
    public class CharacterModule : Milimoe.FunGame.Core.Library.Common.Addon.CharacterModule
    {
        public override string Name => OshimaGameModuleConstant.Character;
        public override string Description => OshimaGameModuleConstant.Description;
        public override string Version => OshimaGameModuleConstant.Version;
        public override string Author => OshimaGameModuleConstant.Author;

        public override Dictionary<string, Character> Characters
        {
            get
            {
                EntityModuleConfig<Character> config = new(OshimaGameModuleConstant.General, OshimaGameModuleConstant.Character);
                config.LoadConfig();
                return config;
            }
        }
    }
}
