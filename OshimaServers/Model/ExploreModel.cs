using System.Text;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaServers.Service;

namespace Oshima.FunGame.OshimaServers.Model
{
    public class ExploreModel()
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public long RegionId { get; set; } = 0;
        public IEnumerable<long> CharacterIds { get; set; } = [];
        public DateTime? StartTime { get; set; } = null;
        public ExploreResult Result { get; set; } = ExploreResult.Nothing;
        public string String { get; set; } = "";
        public double CreditsAward { get; set; } = 0;
        public double MaterialsAward { get; set; } = 0;
        public Dictionary<string, int> Awards { get; set; } = [];
        public bool FightWin { get; set; } = false;
        public double[] AfterFightHPs { get; set; } = [];

        public string GetExploreInfo(IEnumerable<Character> inventoryCharacters, IEnumerable<Region> regions)
        {
            StringBuilder sb = new();

            if (CharacterIds.Any())
            {
                if (regions.FirstOrDefault(r => r.Id == RegionId) is Region region)
                {
                    sb.AppendLine($"☆--- 正在探索 {RegionId} 号地区：{region.Name} ---☆");
                    if (StartTime != null)
                    {
                        sb.AppendLine($"探索时间：{StartTime.Value.ToString(General.GeneralDateTimeFormatChinese)}");
                    }
                    sb.AppendLine($"探索角色：{FunGameService.GetCharacterGroupInfoByInventorySequence(inventoryCharacters, CharacterIds, "，")}");
                }
            }

            return sb.ToString().Trim();
        }
    }
}
