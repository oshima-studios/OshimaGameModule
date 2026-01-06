using Milimoe.FunGame.Core.Interface.Base;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Model;
using Oshima.Core.Constant;

namespace Oshima.FunGame.OshimaMaps
{
    public class FastAutoMap : GameMap
    {
        public override string Name => OshimaGameModuleConstant.FastAutoMap;

        public override string Description => OshimaGameModuleConstant.Description;

        public override string Version => OshimaGameModuleConstant.Version;

        public override string Author => OshimaGameModuleConstant.Author;

        public override int Length => 9;

        public override int Width => 9;

        public override int Height => 1;

        public override float Size => 6;

        public override GameMap InitGamingQueue(IGamingQueue queue)
        {
            GameMap map = new FastAutoMap();
            map.Load();

            if (queue is GamingQueue gq)
            {
                gq.WriteLine($"地图 {map.Name} 已加载。");
            }

            return map;
        }
    }
}
