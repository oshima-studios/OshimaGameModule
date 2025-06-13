using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public class DynamicsEffect : Effect
    {
        public override long Id => (long)EffectID.DynamicsEffect;
        public override string Name { get; set; } = "动态扩展特效";
        public override string Description => string.Join("", Descriptions) + (Source != null && (Skill.Character != Source || Skill is not OpenSkill) ? $"来自：[ {Source} ]" + (Skill.Item != null ? $" 的 [ {Skill.Item.Name} ]" : (Skill is OpenSkill ? "" : $" 的 [ {Skill.Name} ]")) : "");
        public HashSet<string> Descriptions { get; } = [];
        public Dictionary<string, double> RealDynamicsValues { get; } = [];

        public override void OnEffectGained(Character character)
        {
            if (Durative && RemainDuration == 0)
            {
                RemainDuration = Duration;
            }
            else if (RemainDurationTurn == 0)
            {
                RemainDurationTurn = DurationTurn;
            }
            Resolve(character);
        }

        public override void OnEffectLost(Character character)
        {
            Resolve(character, true);
        }

        public override void OnAttributeChanged(Character character)
        {
            // 刷新加成
            OnEffectLost(character);
            OnEffectGained(character);
        }

        private void Resolve(Character character, bool remove = false)
        {
            foreach (string key in Values.Keys)
            {
                string value = Values[key].ToString() ?? "";
                switch (key.ToLower())
                {
                    case "exatk":
                        if (double.TryParse(value, out double exATK))
                        {
                            if (!remove)
                            {
                                character.ExATK2 += exATK;
                            }
                            else if (RealDynamicsValues.TryGetValue("exatk", out double current))
                            {
                                character.ExATK2 -= current;
                            }
                            RealDynamicsValues["exatk"] = exATK;
                            Descriptions.Add($"{(exATK >= 0 ? "增加" : "减少")}角色 {Math.Abs(exATK):0.##} 点攻击力。");
                        }
                        break;
                    case "exdef":
                        if (double.TryParse(value, out double exDEF))
                        {
                            if (!remove)
                            {
                                character.ExDEF2 += exDEF;
                            }
                            else if (RealDynamicsValues.TryGetValue("exdef", out double current))
                            {
                                character.ExDEF2 -= current;
                            }
                            RealDynamicsValues["exdef"] = exDEF;
                            Descriptions.Add($"{(exDEF >= 0 ? "增加" : "减少")}角色 {Math.Abs(exDEF):0.##} 点物理护甲。");
                        }
                        break;
                    case "exstr":
                        if (double.TryParse(value, out double exSTR))
                        {
                            if (!remove)
                            {
                                character.ExSTR += exSTR;
                            }
                            else if (RealDynamicsValues.TryGetValue("exstr", out double current))
                            {
                                character.ExSTR -= current;
                            }
                            RealDynamicsValues["exstr"] = exSTR;
                            Descriptions.Add($"{(exSTR >= 0 ? "增加" : "减少")}角色 {Math.Abs(exSTR):0.##} 点力量。");
                        }
                        break;
                    case "exagi":
                        if (double.TryParse(value, out double exAGI))
                        {
                            if (!remove)
                            {
                                character.ExAGI += exAGI;
                            }
                            else if (RealDynamicsValues.TryGetValue("exagi", out double current))
                            {
                                character.ExAGI -= current;
                            }
                            RealDynamicsValues["exagi"] = exAGI;
                            Descriptions.Add($"{(exAGI >= 0 ? "增加" : "减少")}角色 {Math.Abs(exAGI):0.##} 点敏捷。");
                        }
                        break;
                    case "exint":
                        if (double.TryParse(value, out double exINT))
                        {
                            if (!remove)
                            {
                                character.ExINT += exINT;
                            }
                            else if (RealDynamicsValues.TryGetValue("exint", out double current))
                            {
                                character.ExINT -= current;
                            }
                            RealDynamicsValues["exint"] = exINT;
                            Descriptions.Add($"{(exINT >= 0 ? "增加" : "减少")}角色 {Math.Abs(exINT):0.##} 点智力。");
                        }
                        break;
                    case "shtr":
                        if (double.TryParse(value, out double shtr))
                        {
                            if (!remove)
                            {
                                foreach (Skill s in character.Skills)
                                {
                                    s.HardnessTime -= shtr;
                                }
                                foreach (Skill? s in character.Items.Select(i => i.Skills.Active))
                                {
                                    if (s != null)
                                        s.HardnessTime -= shtr;
                                }
                            }
                            else if (RealDynamicsValues.TryGetValue("shtr", out double current))
                            {
                                foreach (Skill s in character.Skills)
                                {
                                    s.HardnessTime += current;
                                }
                                foreach (Skill? s in character.Items.Select(i => i.Skills.Active))
                                {
                                    if (s != null)
                                        s.HardnessTime += current;
                                }
                            }
                            RealDynamicsValues["shtr"] = shtr;
                            Descriptions.Add($"减少角色的所有主动技能 {shtr:0.##} {GameplayEquilibriumConstant.InGameTime}硬直时间。");
                        }
                        break;
                    case "nahtr":
                        if (double.TryParse(value, out double nahtr))
                        {
                            if (!remove)
                            {
                                character.NormalAttack.HardnessTime -= nahtr;
                            }
                            else if (RealDynamicsValues.TryGetValue("nahtr", out double current))
                            {
                                character.NormalAttack.HardnessTime += current;
                            }
                            RealDynamicsValues["nahtr"] = nahtr;
                            Descriptions.Add($"减少角色的普通攻击 {nahtr:0.##} {GameplayEquilibriumConstant.InGameTime}硬直时间。");
                        }
                        break;
                    case "shtr2":
                        if (double.TryParse(value, out double shtr2))
                        {
                            if (!remove)
                            {
                                foreach (Skill s in character.Skills)
                                {
                                    s.HardnessTime -= s.HardnessTime * shtr2;
                                }
                                foreach (Skill? s in character.Items.Select(i => i.Skills.Active))
                                {
                                    if (s != null)
                                        s.HardnessTime -= s.HardnessTime * shtr2;
                                }
                            }
                            else if (RealDynamicsValues.TryGetValue("shtr2", out double current))
                            {
                                foreach (Skill s in character.Skills)
                                {
                                    s.HardnessTime += s.HardnessTime * current;
                                }
                                foreach (Skill? s in character.Items.Select(i => i.Skills.Active))
                                {
                                    if (s != null)
                                        s.HardnessTime += s.HardnessTime * current;
                                }
                            }
                            RealDynamicsValues["shtr2"] = shtr2;
                            Descriptions.Add($"减少角色的所有主动技能 {shtr2 * 100:0.##}% {GameplayEquilibriumConstant.InGameTime}硬直时间。");
                        }
                        break;
                    case "nahtr2":
                        if (double.TryParse(value, out double nahtr2))
                        {
                            if (!remove)
                            {
                                character.NormalAttack.HardnessTime -= character.NormalAttack.HardnessTime * nahtr2;
                            }
                            else if (RealDynamicsValues.TryGetValue("nahtr2", out double current))
                            {
                                character.NormalAttack.HardnessTime += character.NormalAttack.HardnessTime * current;
                            }
                            RealDynamicsValues["nahtr2"] = nahtr2;
                            Descriptions.Add($"减少角色的普通攻击 {nahtr2 * 100:0.##}% {GameplayEquilibriumConstant.InGameTime}硬直时间。");
                        }
                        break;
                    case "exacc":
                        if (double.TryParse(value, out double exacc))
                        {
                            if (!remove)
                            {
                                character.ExAccelerationCoefficient += exacc;
                            }
                            else if (RealDynamicsValues.TryGetValue("exacc", out double current))
                            {
                                character.ExAccelerationCoefficient -= current;
                            }
                            RealDynamicsValues["exacc"] = exacc;
                            Descriptions.Add($"{(exacc >= 0 ? "增加" : "减少")}角色 {Math.Abs(exacc) * 100:0.##}% 加速系数。");
                        }
                        break;
                    case "exspd":
                        if (double.TryParse(value, out double exspd))
                        {
                            if (!remove)
                            {
                                character.ExSPD += exspd;
                            }
                            else if (RealDynamicsValues.TryGetValue("exspd", out double current))
                            {
                                character.ExSPD -= current;
                            }
                            RealDynamicsValues["exspd"] = exspd;
                            Descriptions.Add($"{(exspd >= 0 ? "增加" : "减少")}角色 {Math.Abs(exspd):0.##} 点行动速度。");
                        }
                        break;
                    case "exac":
                        if (double.TryParse(value, out double exac))
                        {
                            if (!remove)
                            {
                                character.ExActionCoefficient += exac;
                            }
                            else if (RealDynamicsValues.TryGetValue("exac", out double current))
                            {
                                character.ExActionCoefficient -= current;
                            }
                            RealDynamicsValues["exac"] = exac;
                            Descriptions.Add($"{(exac >= 0 ? "增加" : "减少")}角色 {Math.Abs(exac) * 100:0.##}% 行动系数。");
                        }
                        break;
                    case "excdr":
                        if (double.TryParse(value, out double excdr))
                        {
                            if (!remove)
                            {
                                character.ExCDR += excdr;
                            }
                            else if (RealDynamicsValues.TryGetValue("excdr", out double current))
                            {
                                character.ExCDR -= current;
                            }
                            RealDynamicsValues["excdr"] = excdr;
                            Descriptions.Add($"{(excdr >= 0 ? "增加" : "减少")}角色 {Math.Abs(excdr) * 100:0.##}% 冷却缩减。");
                        }
                        break;
                    case "exhp":
                        if (double.TryParse(value, out double exhp))
                        {
                            if (!remove)
                            {
                                character.ExHP2 += exhp;
                            }
                            else if (RealDynamicsValues.TryGetValue("exhp", out double current))
                            {
                                character.ExHP2 -= current;
                            }
                            RealDynamicsValues["exhp"] = exhp;
                            Descriptions.Add($"{(exhp >= 0 ? "增加" : "减少")}角色 {Math.Abs(exhp):0.##} 点最大生命值。");
                        }
                        break;
                    case "exmp":
                        if (double.TryParse(value, out double exmp))
                        {
                            if (!remove)
                            {
                                character.ExMP2 += exmp;
                            }
                            else if (RealDynamicsValues.TryGetValue("exmp", out double current))
                            {
                                character.ExMP2 -= current;
                            }
                            RealDynamicsValues["exmp"] = exmp;
                            Descriptions.Add($"{(exmp >= 0 ? "增加" : "减少")}角色 {Math.Abs(exmp):0.##} 点最大魔法值。");
                        }
                        break;
                    case "excr":
                        if (double.TryParse(value, out double excr))
                        {
                            if (!remove)
                            {
                                character.ExCritRate += excr;
                            }
                            else if (RealDynamicsValues.TryGetValue("excr", out double current))
                            {
                                character.ExCritRate -= current;
                            }
                            RealDynamicsValues["excr"] = excr;
                            Descriptions.Add($"{(excr >= 0 ? "增加" : "减少")}角色 {Math.Abs(excr) * 100:0.##}% 暴击率。");
                        }
                        break;
                    case "excrd":
                        if (double.TryParse(value, out double excrd))
                        {
                            if (!remove)
                            {
                                character.ExCritDMG += excrd;
                            }
                            else if (RealDynamicsValues.TryGetValue("excrd", out double current))
                            {
                                character.ExCritDMG -= current;
                            }
                            RealDynamicsValues["excrd"] = excrd;
                            Descriptions.Add($"{(excrd >= 0 ? "增加" : "减少")}角色 {Math.Abs(excrd) * 100:0.##}% 暴击伤害。");
                        }
                        break;
                    case "exer":
                        if (double.TryParse(value, out double exer))
                        {
                            if (!remove)
                            {
                                character.ExEvadeRate += exer;
                            }
                            else if (RealDynamicsValues.TryGetValue("exer", out double current))
                            {
                                character.ExEvadeRate -= current;
                            }
                            RealDynamicsValues["exer"] = exer;
                            Descriptions.Add($"{(exer >= 0 ? "增加" : "减少")}角色 {Math.Abs(exer) * 100:0.##}% 闪避率。");
                        }
                        break;
                    case "exppt":
                        if (double.TryParse(value, out double exppt))
                        {
                            if (!remove)
                            {
                                character.PhysicalPenetration += exppt;
                            }
                            else if (RealDynamicsValues.TryGetValue("exppt", out double current))
                            {
                                character.PhysicalPenetration -= current;
                            }
                            RealDynamicsValues["exppt"] = exppt;
                            Descriptions.Add($"{(exppt >= 0 ? "增加" : "减少")}角色 {Math.Abs(exppt) * 100:0.##}% 物理穿透。");
                        }
                        break;
                    case "exmpt":
                        if (double.TryParse(value, out double exmpt))
                        {
                            if (!remove)
                            {
                                character.MagicalPenetration += exmpt;
                            }
                            else if (RealDynamicsValues.TryGetValue("exmpt", out double current))
                            {
                                character.MagicalPenetration -= current;
                            }
                            RealDynamicsValues["exmpt"] = exmpt;
                            Descriptions.Add($"{(exmpt >= 0 ? "增加" : "减少")}角色 {Math.Abs(exmpt) * 100:0.##}% 魔法穿透。");
                        }
                        break;
                    case "expdr":
                        if (double.TryParse(value, out double expdr))
                        {
                            if (!remove)
                            {
                                character.ExPDR += expdr;
                            }
                            else if (RealDynamicsValues.TryGetValue("expdr", out double current))
                            {
                                character.ExPDR -= current;
                            }
                            RealDynamicsValues["expdr"] = expdr;
                            Descriptions.Add($"{(expdr >= 0 ? "增加" : "减少")}角色 {Math.Abs(expdr) * 100:0.##}% 物理伤害减免。");
                        }
                        break;
                    case "exhr":
                        if (double.TryParse(value, out double exhr))
                        {
                            if (!remove)
                            {
                                character.ExHR += exhr;
                            }
                            else if (RealDynamicsValues.TryGetValue("exhr", out double current))
                            {
                                character.ExHR -= current;
                            }
                            RealDynamicsValues["exhr"] = exhr;
                            Descriptions.Add($"{(exhr >= 0 ? "增加" : "减少")}角色 {Math.Abs(exhr):0.##} 点生命回复。");
                        }
                        break;
                    case "exmr":
                        if (double.TryParse(value, out double exmr))
                        {
                            if (!remove)
                            {
                                character.ExMR += exmr;
                            }
                            else if (RealDynamicsValues.TryGetValue("exmr", out double current))
                            {
                                character.ExMR -= current;
                            }
                            RealDynamicsValues["exmr"] = exmr;
                            Descriptions.Add($"{(exmr >= 0 ? "增加" : "减少")}角色 {Math.Abs(exmr):0.##} 点魔法回复。");
                        }
                        break;
                    case "exatk2":
                        if (double.TryParse(value, out double exATK2))
                        {
                            double real = 0;
                            if (!remove)
                            {
                                real = character.BaseATK * exATK2;
                                character.ExATKPercentage += exATK2;
                            }
                            else if (RealDynamicsValues.TryGetValue("exatk2", out double percentage))
                            {
                                character.ExATKPercentage -= percentage;
                            }
                            RealDynamicsValues["exatk2"] = exATK2;
                            Descriptions.Add($"{(real >= 0 ? "增加" : "减少")}角色 {Math.Abs(exATK2) * 100:0.##}% [ {Math.Abs(real):0.##} ] 点攻击力。");
                        }
                        break;
                    case "exdef2":
                        if (double.TryParse(value, out double exDEF2))
                        {
                            double real = 0;
                            if (!remove)
                            {
                                real = character.BaseDEF * exDEF2;
                                character.ExDEFPercentage += exDEF2;
                            }
                            else if (RealDynamicsValues.TryGetValue("exdef2", out double percentage))
                            {
                                character.ExDEFPercentage -= percentage;
                            }
                            RealDynamicsValues["exdef2"] = exDEF2;
                            Descriptions.Add($"{(real >= 0 ? "增加" : "减少")}角色 {Math.Abs(exDEF2) * 100:0.##}% [ {Math.Abs(real):0.##} ] 点物理护甲。");
                        }
                        break;
                    case "exstr2":
                        if (double.TryParse(value, out double exSTR2))
                        {
                            double real = 0;
                            if (!remove)
                            {
                                real = character.BaseSTR * exSTR2;
                                character.ExSTRPercentage += exSTR2;
                            }
                            else if (RealDynamicsValues.TryGetValue("exstr2", out double percentage))
                            {
                                character.ExSTRPercentage -= percentage;
                            }
                            RealDynamicsValues["exstr2"] = exSTR2;
                            Descriptions.Add($"{(real >= 0 ? "增加" : "减少")}角色 {Math.Abs(exSTR2) * 100:0.##}% [ {Math.Abs(real):0.##} ] 点力量。");
                        }
                        break;
                    case "exagi2":
                        if (double.TryParse(value, out double exAGI2))
                        {
                            double real = 0;
                            if (!remove)
                            {
                                real = character.BaseAGI * exAGI2;
                                character.ExAGIPercentage += exAGI2;
                            }
                            else if (RealDynamicsValues.TryGetValue("exagi2", out double percentage))
                            {
                                character.ExAGIPercentage -= percentage;
                            }
                            RealDynamicsValues["exagi2"] = exAGI2;
                            Descriptions.Add($"{(real >= 0 ? "增加" : "减少")}角色 {Math.Abs(exAGI2) * 100:0.##}% [ {Math.Abs(real):0.##} ] 点敏捷。");
                        }
                        break;
                    case "exint2":
                        if (double.TryParse(value, out double exINT2))
                        {
                            double real = 0;
                            if (!remove)
                            {
                                real = character.BaseINT * exINT2;
                                character.ExINTPercentage += exINT2;
                            }
                            else if (RealDynamicsValues.TryGetValue("exint2", out double percentage))
                            {
                                character.ExINTPercentage -= percentage;
                            }
                            RealDynamicsValues["exint2"] = exINT2;
                            Descriptions.Add($"{(real >= 0 ? "增加" : "减少")}角色 {Math.Abs(exINT2) * 100:0.##}% [ {Math.Abs(real):0.##} ] 点智力。");
                        }
                        break;
                    case "exhp2":
                        if (double.TryParse(value, out double exhp2))
                        {
                            double real = 0;
                            if (!remove)
                            {
                                real = character.MaxHP * exhp2;
                                character.ExHPPercentage += exhp2;
                            }
                            else if (RealDynamicsValues.TryGetValue("exhp2", out double percentage))
                            {
                                character.ExHPPercentage -= percentage;
                            }
                            RealDynamicsValues["exhp2"] = exhp2;
                            Descriptions.Add($"{(real >= 0 ? "增加" : "减少")}角色 {Math.Abs(exhp2) * 100:0.##}% [ {Math.Abs(real):0.##} ] 点最大生命值。");
                        }
                        break;
                    case "exmp2":
                        if (double.TryParse(value, out double exmp2))
                        {
                            double real = 0;
                            if (!remove)
                            {
                                real = character.MaxMP * exmp2;
                                character.ExMPPercentage += exmp2;
                            }
                            else if (RealDynamicsValues.TryGetValue("exmp2", out double percentage))
                            {
                                character.ExMPPercentage -= percentage;
                            }
                            RealDynamicsValues["exmp2"] = exmp2;
                            Descriptions.Add($"{(real >= 0 ? "增加" : "减少")}角色 {Math.Abs(exmp2) * 100:0.##}% [ {Math.Abs(real):0.##} ] 点最大魔法值。");
                        }
                        break;
                    case "mdftype":
                        if (int.TryParse(value, out int mdftype))
                        {
                            double mdfValue = 0;
                            MagicType magicType = MagicType.None;
                            if (!remove)
                            {
                                magicType = (MagicType)mdftype;
                                string mdfvalueKey = Values.Keys.FirstOrDefault(s => s.Equals("mdfvalue", StringComparison.CurrentCultureIgnoreCase)) ?? "";
                                if (mdfvalueKey.Length > 0 && double.TryParse(Values[mdfvalueKey].ToString(), out mdfValue))
                                {
                                    if (magicType == MagicType.None)
                                    {
                                        character.MDF.AddAllValue(mdfValue);
                                    }
                                    else
                                    {
                                        character.MDF[magicType] += mdfValue;
                                    }
                                }
                            }
                            else if (RealDynamicsValues.TryGetValue("mdftype", out double currentType))
                            {
                                magicType = (MagicType)(int)currentType;
                                if (RealDynamicsValues.TryGetValue("mdfvalue", out mdfValue))
                                {
                                    if (magicType == MagicType.None)
                                    {
                                        character.MDF.AddAllValue(-mdfValue);
                                    }
                                    else
                                    {
                                        character.MDF[magicType] -= mdfValue;
                                    }
                                }
                            }
                            RealDynamicsValues["mdftype"] = mdftype;
                            RealDynamicsValues["mdfvalue"] = mdfValue;
                            Descriptions.Add($"{(mdfValue >= 0 ? "增加" : "减少")}角色 {Math.Abs(mdfValue) * 100:0.##}% {CharacterSet.GetMagicResistanceName(magicType)}。");
                        }
                        break;
                }
            }
        }

        public DynamicsEffect(Skill skill, Dictionary<string, object> args, Character? source = null) : base(skill, args)
        {
            EffectType = EffectType.Item;
            GamingQueue = skill.GamingQueue;
            Source = source;
        }
    }
}
