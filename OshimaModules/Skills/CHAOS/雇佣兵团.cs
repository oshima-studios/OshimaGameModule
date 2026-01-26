using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Library.Constant;
using Oshima.FunGame.OshimaModules.Models;

namespace Oshima.FunGame.OshimaModules.Skills
{
    public class 雇佣兵团 : Skill
    {
        public override long Id => (long)PassiveID.雇佣兵团;
        public override string Name => "雇佣兵团";
        public override string Description => Effects.Count > 0 ? Effects.First().Description : "";

        public 雇佣兵团(Character? character = null) : base(SkillType.Passive, character)
        {
            Effects.Add(new 雇佣兵团特效(this));
        }

        public override IEnumerable<Effect> AddPassiveEffectToCharacter()
        {
            return Effects;
        }
    }

    public class 雇佣兵团特效(Skill skill) : Effect(skill)
    {
        public override long Id => Skill.Id;
        public override string Name => Skill.Name;
        public override string Description => $"{Skill.SkillOwner()}在场上时，会召唤数名雇佣兵协助战斗，初始数量为 {最小数量} 名，雇佣兵具有独立的回合，生命值为{Skill.SkillOwner()}的 {生命值比例 * 100:0.##}% [ {Skill.Character?.MaxHP * 生命值比例:0.##} ]，攻击力为{Skill.SkillOwner()}的 {攻击力比例 * 100:0.##}% 基础攻击力 [ {Skill.Character?.BaseATK * 攻击力比例:0.##} ]，" +
            $"完整继承其他能力值（暴击率、闪避率等），雇佣兵每{GameplayEquilibriumConstant.InGameTime}流失 {生命流失 * 100:0.##}% 当前生命值。当{Skill.SkillOwner()}参与击杀时，便会临时产生一名额外的雇佣兵。场上最多可以存在 {最大数量} 名雇佣兵，达到数量后不再产生新的雇佣兵；当不足 {最小数量} 名雇佣兵时，{补充间隔} {GameplayEquilibriumConstant.InGameTime}后会重新补充一名雇佣兵。" +
            (雇佣兵团.Count < 最小数量 && Skill.CurrentCD > 0 ? $"（下次补充：{Skill.CurrentCD:0.##} {GameplayEquilibriumConstant.InGameTime}后）" : "");

        public List<雇佣兵> 雇佣兵团 { get; } = [];
        public const int 最小数量 = 1;
        public const int 最大数量 = 5;
        public const int 补充间隔 = 30;
        public const double 生命值比例 = 0.1;
        public const double 攻击力比例 = 0.6;
        public const double 生命流失 = 0.09;

        private bool 激活 = false;

        public override void OnGameStart()
        {
            if (!激活)
            {
                激活 = true;
                if (Skill.Character != null)
                {
                    保底补充(Skill.Character);
                }
            }
        }

        public override void AfterDeathCalculation(Character death, bool hasMaster, Character? killer, Dictionary<Character, int> continuousKilling, Dictionary<Character, int> earnedMoney, Character[] assists)
        {
            if (death is 雇佣兵 gyb)
            {
                WriteLine($"[ {killer} ] 杀死了 [ {death} ]！");
                雇佣兵团.Remove(gyb);
                if (GamingQueue is Milimoe.FunGame.Core.Model.GamingQueue queue)
                {
                    if (queue.Map != null) queue.RemoveCharacterFromMap(gyb);
                    else queue.RemoveCharacterFromQueue(gyb);
                }
                if (雇佣兵团.Count < 最小数量 && Skill.CurrentCD == 0)
                {
                    Skill.CurrentCD = 补充间隔;
                    Skill.Enable = false;
                }
            }

            if (death == Skill.Character)
            {
                OnEffectLost(death);
            }

            if (Skill.Character != null && death != Skill.Character && (killer == Skill.Character || assists.Contains(Skill.Character)) && 雇佣兵团.Count < 最大数量)
            {
                新增雇佣兵(Skill.Character);
            }
        }

        public override void OnTimeElapsed(Character character, double elapsed)
        {
            if (character == Skill.Character)
            {
                保底补充(character);
            }
            foreach (雇佣兵 gyb in 雇佣兵团)
            {
                if (gyb.HP > 0)
                {
                    double lost = gyb.HP * 生命流失 * elapsed;
                    gyb.HP -= lost;
                    if (gyb.HP <= 0)
                    {
                        gyb.HP = 1;
                    }
                }
            }
        }

        public override void OnEffectLost(Character character)
        {
            if (GamingQueue != null)
            {
                if (GamingQueue is Milimoe.FunGame.Core.Model.GamingQueue queue)
                {
                    if (queue.Map != null) queue.RemoveCharacterFromMap(雇佣兵团);
                    else queue.RemoveCharacterFromQueue(雇佣兵团);
                }
                雇佣兵团.Clear();
                WriteLine($"[ {character} ] 的雇佣兵团已消散！");
            }
        }

        public void 保底补充(Character character)
        {
            if (!激活) return;
            int count = 雇佣兵团.Count;
            if (count < 最小数量 && Skill.Enable)
            {
                do
                {
                    count = 新增雇佣兵(character);
                }
                while (count < 最小数量);
            }
            if (GamingQueue != null)
            {
                foreach (雇佣兵 gyb in 雇佣兵团.Where(g => !GamingQueue.Queue.Contains(g)))
                {
                    if (gyb.HP <= 0) gyb.HP = 1;
                    添加到地图(character, gyb);
                }
            }
        }

        public int 新增雇佣兵(Character character)
        {
            雇佣兵 gyb = new(character, FunGameConstant.GenerateRandomChineseUserName())
            {
                Level = 1,
                InitialHP = character.MaxHP * 生命值比例,
                InitialATK = character.BaseATK * 攻击力比例,
                ExCritRate = character.CritRate - GameplayEquilibriumConstant.CritRate,
                ExCritDMG = character.CritDMG - GameplayEquilibriumConstant.CritDMG,
                ExEvadeRate = character.EvadeRate - GameplayEquilibriumConstant.EvadeRate,
                InitialSPD = character.SPD,
                InitialDEF = character.DEF,
                InitialHR = character.HR,
                InitialMR = character.MR,
                Lifesteal = character.Lifesteal,
                ExPDR = character.ExPDR,
                PhysicalPenetration = character.PhysicalPenetration,
                MagicalPenetration = character.MagicalPenetration,
                ExMOV = character.MOV,
                ExATR = character.ATR - 1
            };
            gyb.MDF.AddAllValue(character.MDF.Avg);
            gyb.Recovery();
            雇佣兵团.Add(gyb);

            添加到地图(character, gyb);
            WriteLine($"[ {character} ] 召唤了{gyb}！");

            return 雇佣兵团.Count;
        }

        public void 添加到地图(Character character, 雇佣兵 gyb)
        {
            // 添加到地图/队列
            if (GamingQueue != null)
            {
                if (GamingQueue.Map is GameMap map)
                {
                    Grid? current = map.GetCharacterCurrentGrid(character);
                    if (current != null)
                    {
                        List<Grid> nearbyGrids = map.GetGridsByRange(current, 5, false);
                        Grid? target = nearbyGrids.OrderBy(g => GameMap.CalculateManhattanDistance(g, current)).FirstOrDefault();
                        if (target != null)
                        {
                            map.SetCharacterCurrentGrid(gyb, target);
                        }
                    }
                }
                GamingQueue.Queue.Add(gyb);
                GamingQueue.ChangeCharacterHardnessTime(gyb, 5, false, false);
            }
        }
    }
}
