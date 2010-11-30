using System;
using Microsoft.Xna.Framework;

using GoblinXNA.SceneGraph;
using GoblinXNA.Helpers;

namespace ARRG_Game
{
    class Dragonkin : Monster
    {
        private FireGenerator fire = new FireGenerator();
        private TransformNode origin;
        

        public Dragonkin(string name, String model, int health, int power, bool useInternal, int dieNum)
            : base(name, model, health, power, useInternal, dieNum)
        {
            hit = 80;
            dodge = 20;
            crit = 40;
            Type = CreatureType.DRAGONKIN;
            origin = transNode;
            transNode.AddChild(fire.addParticle());
            
        }

        public override void applyLine(Vector3 source, Vector3 target)
        {
            base.applyLine(source, target);
            fire.update(source, target);
            fire.disable();
        }


        public override void startAttackAnimation()
        {

            if (!isSpecialAttack && !isNormalAttack && RandomHelper.GetRandomInt(100) + 1 <= fireBreath )
            {
                fire.setSource(monsterNode.BoundingVolume.Center);
                isSpecialAttack = true;
                fire.enable();
            }
            else if (attackTimer-- > 0 && !isSpecialAttack)
            {
                isNormalAttack = true;
                transNode.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(10));
            }

            //else transNode.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(0));
        }

        public override void endAttackAnimation()
        {
            //Should end simple "animation" of Monsters atack
            //transNode = origin;
        }
        //********End Dice-Monster Interactions********************
    }
}