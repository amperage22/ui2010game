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
            hit = 80;
            dodge = 20;
            crit = 40;
        }

        public void attack()
        {
        }

        public void defend()
        {
        }
    }
}