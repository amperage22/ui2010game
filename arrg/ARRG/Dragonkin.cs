using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    class Dragonkin : Monster
    {
        public Dragonkin(string name, String model, int health, int power, bool useInternal)
            : base(name, model, health, power, useInternal)
        {
            hit = 80;
            dodge = 20;
            crit = 40;
            type = CreatureType.DRAGONKIN;
        }

        new public void endAttackAnimation()
        {
            //Should end simple "animation" of Monsters atack
        }
        //********End Dice-Monster Interactions********************
    }
}