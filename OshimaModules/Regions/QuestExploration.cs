using Oshima.Core.Constant;

namespace Oshima.FunGame.OshimaModules.Regions
{
    public class QuestExploration(string description, string character = "", string item = "", ExploreResult result = ExploreResult.General)
    {
        public ExploreResult ExploreResult { get; set; } = result;
        public string Description { get; set; } = description;
        public string Character { get; set; } = character;
        public string Item { get; set; } = item;
    }
}
