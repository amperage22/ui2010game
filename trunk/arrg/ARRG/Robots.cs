using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ARRG_Game
{
    class Robots : Monster
    {
      private Boolean showAttack;
        public Robots(string name, String model, int health, int power, bool useInternal)
            : base(name, model, health, power, useInternal)
        {
            hit = 85;
            dodge = 25;
            crit = 30;
            type = CreatureType.ROBOTS;
            showAttack = false;
        }

        new public void startAttackAnimation()
        {

          float modifier;
          //Should create simple "animation" of Monsters atack
          origin = transNode.Translation;
          //Console.Write(DateTime.Now.Second);
          modifier = 0.01f;
          Console.WriteLine("inloop");


          transNode.Translation += new Vector3(modifier, 0, 0);

        }


    }
}