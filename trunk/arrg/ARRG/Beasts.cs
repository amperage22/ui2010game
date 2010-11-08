using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ARRG_Game
{
    //protected double hit, dodge, crit;  //Instantiated through Child classes
    class Beasts : Monster
    {
      
        public Beasts(string name, String model, int health, int power, bool useInternal)
            : base(name, model, health, power, useInternal)
        {
            hit = 70;
            dodge = 40;
            crit = 30;
            type = CreatureType.BEASTS;
        }

        public override void startAttackAnimation()
        {
          float modifier = 1.0f;
          if (attackTimer-- > 0)
          {
            if (attackTimer < 25)
            {
              transNode.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(-5));
              transNode.Translation += new Vector3(modifier, 0, 0);
            }
            else
            {
              transNode.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitY, MathHelper.ToRadians(5));
              transNode.Translation -= new Vector3(modifier, 0, 0);
            }
          }
          
        }
    }
}
