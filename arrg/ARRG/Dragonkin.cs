using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    public class Dragonkin : Monster
    {
        public Dragonkin(string name, String model, int health, int power, bool useInternal)
            : base(name, model, health, power, useInternal)
        {
            Hit = 80;
            Dodge = 20;
            Crit = 40;
        }

        new public void attack()
        {
        }

        new public void defend()
        {
        }
    }
}