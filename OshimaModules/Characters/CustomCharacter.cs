using System;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Core.Library.Constant;

namespace Oshima.FunGame.OshimaModules.Characters
{
    public class CustomCharacter : Character
    {
        public CustomCharacter(long user_id, string name, string firstname = "", string nickname = "") : base()
        {
            Id = user_id;
            Name = name;
            FirstName = firstname;
            NickName = nickname;
            PrimaryAttribute = (PrimaryAttribute)Random.Shared.Next(1, 4);
            InitialATK = Random.Shared.Next(15, 26);
            InitialHP = Random.Shared.Next(40, 86);
            InitialMP = Random.Shared.Next(20, 56);

            int value = 31;
            int valueGrowth = 31;
            for (int i = 0; i < 3; i++)
            {
                if (value == 0) break;
                int attribute = i < 2 ? Random.Shared.Next(value) : (value - 1);
                int growth = i < 2 ? Random.Shared.Next(0, valueGrowth) : (valueGrowth - 1);
                switch (i)
                {
                    case 1:
                        InitialAGI = attribute;
                        AGIGrowth = Calculation.Round(Convert.ToDouble(growth) / 10, 2);
                        break;
                    case 2:
                        InitialINT = attribute;
                        INTGrowth = Calculation.Round(Convert.ToDouble(growth) / 10, 2);
                        break;
                    case 0:
                    default:
                        InitialSTR = attribute;
                        STRGrowth = Calculation.Round(Convert.ToDouble(growth) / 10, 2);
                        break;
                }
                value -= attribute;
                valueGrowth -= growth;
            }
            InitialSPD = Random.Shared.Next(220, 291);
            InitialHR = Random.Shared.Next(1, 6);
            InitialMR = Random.Shared.Next(1, 6);
        }

        public void SetPrimaryAttribute(PrimaryAttribute? value = null)
        {
            if (value != null && value.HasValue)
            {
                PrimaryAttribute = value.Value;
            }
            else
            {
                double max = Math.Max(Math.Max(STR, AGI), INT);
                if (max == STR)
                {
                    PrimaryAttribute = PrimaryAttribute.STR;
                }
                else if (max == AGI)
                {
                    PrimaryAttribute = PrimaryAttribute.AGI;
                }
                else if (max == INT)
                {
                    PrimaryAttribute = PrimaryAttribute.INT;
                }
            }
        }
    }
}
