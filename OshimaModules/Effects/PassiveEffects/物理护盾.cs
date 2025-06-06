﻿using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Effects.PassiveEffects
{
    public class 物理护盾 : Effect
    {
        public override long Id => 4106;
        public override string Name => "物理护盾";
        public override string Description => $"此角色拥有物理护盾。来自：[ {Source} ] 的 [ {Skill.Name} ]";
        public override EffectType EffectType => EffectType.Shield;
        public override DispelledType DispelledType => DispelledType.CannotBeDispelled;
        public override bool DurativeWithoutDuration => true;
        public override bool IsDebuff => false;
        public override Character Source => _sourceCharacter;

        private readonly Character _sourceCharacter;
        private readonly double _shield;

        public 物理护盾(Skill skill, Character sourceCharacter, double shield) : base(skill)
        {
            GamingQueue = skill.GamingQueue;
            _sourceCharacter = sourceCharacter;
            _shield = shield;
        }

        public override void OnEffectGained(Character character)
        {
            character.Shield.Physical += _shield;
        }

        public override bool OnShieldBroken(Character character, Character attacker, bool isMagic, MagicType magicType, double damage, double shield, double overFlowing)
        {
            Effect[] effects = [.. character.Effects.Where(e => e is 物理护盾)];
            foreach (Effect effect in effects)
            {
                character.Effects.Remove(effect);
            }
            return true;
        }
    }
}
