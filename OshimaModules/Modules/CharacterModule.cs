using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Oshima.FunGame.OshimaModules.Characters;

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

        protected override Factory.EntityFactoryDelegate<Character> EntityFactory()
        {
            return (id, name, args) =>
            {
                return id switch
                {
                    1 => new OshimaShiya(),
                    2 => new XinYin(),
                    3 => new Yang(),
                    4 => new NanGanYu(),
                    5 => new NiuNan(),
                    6 => new DokyoMayor(),
                    7 => new MagicalGirl(),
                    8 => new QingXiang(),
                    9 => new QWQAQW(),
                    10 => new ColdBlue(),
                    11 => new dddovo(),
                    12 => new Quduoduo(),
                    _ => null,
                };
            };
        }
    }
}
