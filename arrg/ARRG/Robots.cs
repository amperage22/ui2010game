using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    class Robots : Monster
    {
        public Robots(string name, String model, int health, int power, bool useInternal)
            : base(name, model, health, power, useInternal)
        {
            hit = 85;
            dodge = 25;
            crit = 30;
            type = CreatureType.ROBOTS;
        }
    }
}