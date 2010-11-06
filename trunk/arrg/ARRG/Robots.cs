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
            baseHit = 85;
            baseDodge = 25;
            baseCrit = 30;
        }
    }
}