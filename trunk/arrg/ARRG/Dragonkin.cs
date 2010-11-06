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
            baseHit = 80;
            baseDodge = 20;
            baseCrit = 40;
        }

        public void endAttackAnimation()
        {
            //Should end simple "animation" of Monsters atack
        }
        //********End Dice-Monster Interactions********************

        //********Monster-Monster Interactions********************
        private void resetNearest()
        {
            if (nearestEnemy.IsDead)
                nearestEnemy = null;
        }
        //*****End Monster-Monster Interactions********************
    }
}