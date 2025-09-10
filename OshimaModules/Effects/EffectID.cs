namespace Oshima.FunGame.OshimaModules.Effects.OpenEffects
{
    public enum EffectID : long
    {
        /// <summary>
        /// 被动特效起点
        /// </summary>
        Passive_Start = 8000,

        /// <summary>
        /// 数值攻击力，参数：exatk
        /// </summary>
        ExATK = 8001,

        /// <summary>
        /// 数值物理护甲，参数：exdef
        /// </summary>
        ExDEF = 8002,

        /// <summary>
        /// 数值力量，参数：exstr
        /// </summary>
        ExSTR = 8003,

        /// <summary>
        /// 数值敏捷，参数：exagi
        /// </summary>
        ExAGI = 8004,

        /// <summary>
        /// 数值智力，参数：exint
        /// </summary>
        ExINT = 8005,

        /// <summary>
        /// 数值技能硬直时间减少，参数：shtr
        /// </summary>
        SkillHardTimeReduce = 8006,

        /// <summary>
        /// 数值普攻硬直时间减少，参数：nahtr
        /// </summary>
        NormalAttackHardTimeReduce = 8007,

        /// <summary>
        /// 加速系数%，参数：exacc
        /// </summary>
        AccelerationCoefficient = 8008,

        /// <summary>
        /// 数值行动速度，参数：exspd
        /// </summary>
        ExSPD = 8009,

        /// <summary>
        /// 行动系数%，参考：exac
        /// </summary>
        ExActionCoefficient = 8010,

        /// <summary>
        /// 冷却缩减%，参数：excdr
        /// </summary>
        ExCDR = 8011,

        /// <summary>
        /// 数值生命值，参数：exhp
        /// </summary>
        ExMaxHP = 8012,

        /// <summary>
        /// 数值魔法值，参数：exmp
        /// </summary>
        ExMaxMP = 8013,

        /// <summary>
        /// 暴击率%，参数：excr
        /// </summary>
        ExCritRate = 8014,

        /// <summary>
        /// 暴击伤害%，参数：excrd
        /// </summary>
        ExCritDMG = 8015,

        /// <summary>
        /// 闪避率%，参数：exer
        /// </summary>
        ExEvadeRate = 8016,

        /// <summary>
        /// 物理穿透%，参数：exppt
        /// </summary>
        PhysicalPenetration = 8017,

        /// <summary>
        /// 魔法穿透%，参数：exmpt
        /// </summary>
        MagicalPenetration = 8018,

        /// <summary>
        /// 物理伤害减免%，参数：expdr
        /// </summary>
        ExPDR = 8019,

        /// <summary>
        /// 魔法抗性%<para/>
        /// 参数：<para/>
        /// 魔法类型（对应MagicType，0为所有）：mdftype<para/>
        /// 魔法抗性%：mdfvalue
        /// </summary>
        ExMDF = 8020,

        /// <summary>
        /// 数值生命回复，参数：exhr
        /// </summary>
        ExHR = 8021,

        /// <summary>
        /// 数值魔法回复，参数：exmr
        /// </summary>
        ExMR = 8022,

        /// <summary>
        /// 攻击力%，参数：exatk
        /// </summary>
        ExATK2 = 8023,

        /// <summary>
        /// 物理护甲%，参数：exdef
        /// </summary>
        ExDEF2 = 8024,

        /// <summary>
        /// 力量%，参数：exstr
        /// </summary>
        ExSTR2 = 8025,

        /// <summary>
        /// 敏捷%，参数：exagi
        /// </summary>
        ExAGI2 = 8026,

        /// <summary>
        /// 智力%，参数：exint
        /// </summary>
        ExINT2 = 8027,

        /// <summary>
        /// 技能硬直时间减少%，参数：shtr
        /// </summary>
        SkillHardTimeReduce2 = 8028,

        /// <summary>
        /// 普攻硬直时间减少%，参数：nahtr
        /// </summary>
        NormalAttackHardTimeReduce2 = 8029,

        /// <summary>
        /// 最大生命值%，参数：exhp
        /// </summary>
        ExMaxHP2 = 8030,

        /// <summary>
        /// 最大魔法值%，参数：exmp
        /// </summary>
        ExMaxMP2 = 8031,

        /// <summary>
        /// 动态扩展特效
        /// </summary>
        DynamicsEffect = 8032,

        /// <summary>
        /// 无视闪避率，参数：p
        /// </summary>
        IgnoreEvade = 8033,

        /// <summary>
        /// 数值攻击距离，参数：exatr
        /// </summary>
        ExATR = 8034,
        
        /// <summary>
        /// 数值移动距离，参数：exmov
        /// </summary>
        ExMOV = 8035,
        
        /// <summary>
        /// 生命偷取%，参数：exls
        /// </summary>
        ExLifesteal = 8038,

        /// <summary>
        /// 被动特效终点
        /// </summary>
        Passive_End = 8699,

        /// <summary>
        /// 主动特效起点
        /// </summary>
        Active_Start = 8700,

        /// <summary>
        /// 立即回复生命值，参数：hp
        /// </summary>
        RecoverHP = 8701,

        /// <summary>
        /// 立即回复魔法值，参数：mp
        /// </summary>
        RecoverMP = 8702,

        /// <summary>
        /// 立即回复生命值%，参数：hp
        /// </summary>
        RecoverHP2 = 8703,

        /// <summary>
        /// 立即回复魔法值%，参数：mp
        /// </summary>
        RecoverMP2 = 8704,

        /// <summary>
        /// 立即获得能量值，参数：ep
        /// </summary>
        GetEP = 8705,

        /// <summary>
        /// 立即获得经验值，参数：exp
        /// </summary>
        GetEXP = 8706,

        /// <summary>
        /// 主动特效终点
        /// </summary>
        Active_End = 8999
    }

    public enum PassiveEffectID : long
    {
        完全行动不能 = 4101,
        行动受限 = 4102,
        战斗不能 = 4103,
        封技 = 4104,
        缴械 = 4105,
        标记 = 4106,
        眩晕 = 4107,
        石化 = 4108,
        冻结 = 4109,
        烧伤 = 4110,
        中毒 = 4111,
        诅咒 = 4112,
        愤怒 = 4113,
        混乱 = 4114,
        气绝 = 4115,
        虚弱 = 4116,
        迟滞 = 4117,
        易伤 = 4118,
        物理护盾 = 4119,
        魔法护盾 = 4120,
        物理免疫 = 4121,
        魔法免疫 = 4122,
        技能免疫 = 4123,
        完全免疫 = 4124,
        持续性弱驱散 = 4125,
        持续性强驱散 = 4126,
        累积之压标记 = 4127
    }
}
