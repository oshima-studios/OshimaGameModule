using System.Text;
using Milimoe.FunGame.Core.Entity;

namespace Oshima.FunGame.OshimaServers.Model
{
    public class Horse(User user)
    {
        public long Id => user.Id;
        public string Name => user.Username;

        // 基础属性，技能会在此基础上进行增减
        private int _step = 1;
        private int _hr = 1;
        private int _hp = 3;

        public int MaxHP { get; set; } = 3;

        /// <summary>
        /// 每回合行动的步数
        /// </summary>
        public int Step
        {
            get
            {
                return _step;
            }
            set
            {
                _step = Math.Max(0, value);
            }
        }

        /// <summary>
        /// 当前生命值
        /// </summary>
        public int HP
        {
            get
            {
                return _hp;
            }
            set
            {
                _hp = Math.Min(MaxHP, Math.Max(0, value));
            }
        }

        /// <summary>
        /// 每回合恢复的HP值
        /// </summary>
        public int HPRecovery
        {
            get
            {
                return _hr;
            }
            set
            {
                _hr = Math.Max(0, value);
            }
        }

        /// <summary>
        /// 马匹当前在赛道上的位置
        /// </summary>
        public int CurrentPosition { get; set; } = 0;

        /// <summary>
        /// 马匹拥有的永久技能
        /// </summary>
        public HashSet<HorseSkill> Skills { get; set; } = [];

        /// <summary>
        /// 马匹当前正在生效的技能效果列表
        /// </summary>
        public List<ActiveSkillEffect> ActiveEffects { get; set; } = [];
        
        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// 技能定义
    /// </summary>
    public class HorseSkill(Horse? horse = null)
    {
        public Horse? Horse { get; set; } = horse;
        public string Name { get; set; } = "";
        public bool ToEnemy { get; set; } = false;
        public int AddStep { get; set; } = 0;
        public int ReduceStep { get; set; } = 0;
        public int AddHP { get; set; } = 0;
        public int ReduceHP { get; set; } = 0;
        public int AddHR { get; set; } = 0;
        public int ReduceHR { get; set; } = 0;
        public int ChangePosition { get; set; } = 0;
        /// <summary>
        /// 技能发动概率，1表示每回合都发动
        /// </summary>
        public double CastProbability { get; set; } = 1;
        /// <summary>
        /// 技能持续回合数，默认1回合（即立即生效并结束）
        /// </summary>
        public int Duration { get; set; } = 1;

        public override string ToString()
        {
            StringBuilder builder = new();

            if (AddStep > 0) builder.Append($"每回合将额外移动 {AddStep} 步！");
            if (ReduceStep > 0) builder.Append($"每回合将少移动 {ReduceStep} 步！");
            if (AddHP > 0) builder.AppendLine($"恢复了 {AddHP} 点生命值！");
            if (ReduceHP > 0) builder.Append($"受到了 {ReduceHP} 点伤害！");
            if (AddHR > 0) builder.Append($"每回合将额外恢复 {AddHR} 点生命值！");
            if (ReduceHR > 0) builder.Append($"每回合将少恢复 {ReduceHR} 点生命值！");
            if (ChangePosition != 0) builder.Append($"{(ChangePosition > 0 ? "前进" : "后退")}了 {Math.Abs(ChangePosition)} 步！");

            return builder.ToString().Trim();
        }
    }

    /// <summary>
    /// 用于追踪马匹身上正在生效的技能效果
    /// </summary>
    public class ActiveSkillEffect(HorseSkill skill)
    {
        public HorseSkill Skill { get; } = skill;
        public int RemainDuration { get; set; } = skill.Duration;
    }
}
