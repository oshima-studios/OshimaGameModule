using Oshima.Core.Constant;

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
    }
}
