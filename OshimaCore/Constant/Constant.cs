﻿using Milimoe.FunGame.Core.Library.Common.Addon;
using Milimoe.FunGame.Core.Model;

namespace Oshima.Core.Constant
{
    public class OshimaGameModuleConstant
    {
        public const string General = "oshima-studios";
        public const string WebAPI = "oshima.fungame.webapi";
        public const string FastAuto = "oshima.fungame.fastauto";
        public const string Character = "oshima.fungame.characters";
        public const string Skill = "oshima.fungame.skills";
        public const string Item = "oshima.fungame.items";
        public const string Description = "Oshima Studios Presents";
        public const string Version = "1.0.0";
        public const string Author = "Oshima Studios";
        public const string FastAutoMap = "oshima.fungame.fastauto.map";
        public const string Anonymous = "oshima.fungame.anonymous";
        public const string AnonymousMap = "oshima.fungame.anonymous.map";
        public const string Server = "oshima.fungame.server";

        private static readonly string[] Maps = [FastAutoMap];
        private static readonly string[] Characters = [Character];
        private static readonly string[] Skills = [Skill];
        private static readonly string[] Items = [Item];
        public static GameModuleDepend GameModuleDepend { get; } = new(Maps, Characters, Skills, Items);

        public static EquilibriumConstant GameplayEquilibriumConstant { get; } = new()
        {
            InGameTime = "秒"
        };
    }
}
