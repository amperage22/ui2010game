using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    public class Robots : Monster
    {
        public Robots(string name, String model, int health, int power, bool useInternal)
            : base(name, model, health, power, useInternal)
        {
            Hit = 85;
            Dodge = 25;
            Crit = 30;
        }
    }
}