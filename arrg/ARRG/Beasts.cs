using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    //protected double hit, dodge, crit;  //Instantiated through Child classes
    class Beasts : Monster
    {
        public Beasts(string name, String model, int health, int power, bool useInternal)
            : base(name, model, health, power, useInternal)
        {
            baseHit = 70;
            baseDodge = 40;
            baseCrit = 30;
        }
    }
}
