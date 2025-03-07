﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Graphics;
using Microsoft.Xna.Framework.Graphics;
using GoblinXNA.Helpers;

namespace ARRG_Game
{
    class Robots : Monster
    {
        private TransformNode origin;
        private LaserLineGenerator laser = new LaserLineGenerator();
        Scene scene;
        private Boolean showAttack;
        public Robots(string name, String model, int health, int power, bool useInternal, int dieNum)
            : base(name, model, health, power, useInternal, dieNum)
        {
            hit = 85;
            dodge = 25;
            crit = 30;
            Type = CreatureType.ROBOTS;
            showAttack = false;
            this.scene = GlobalScene.scene;
            origin = transNode;
            transNode.AddChild(laser.addParticle());
        }
        public override void applyLine(Vector3 source, Vector3 target)
        {
            base.applyLine(source, target);
            laser.update(source, target);
            laser.disable();
        }
        public override void startAttackAnimation()
        {

            if (!isSpecialAttack && !isNormalAttack && RandomHelper.GetRandomInt(100) + 1 <= lightningAttack)
            {
                Vector3 center = monsterNode.BoundingVolume.Center;
                laser.setSource(center + new Vector3(center.X,center.Y,center.Z + monsterNode.BoundingVolume.Radius*2));
                isSpecialAttack = true;
                laser.enable();
            }
            if (attackTimer-- > 0 && !isSpecialAttack)
            {
                isNormalAttack = true;
                float modifier = 2.0f;
                if (attackTimer < 25)
                    transNode.Translation += new Vector3(modifier, 0, 0);
                else
                    transNode.Translation -= new Vector3(modifier, 0, 0);
                //Console.WriteLine(transNode.Translation.X);
            }

        }

        public override void endAttackAnimation()
        {
            //transNode = origin;
        }


        private void createDonut()
        {

            GeometryNode torusNode = new GeometryNode("Torus");
            torusNode.Model = new Torus(0.7f, 1.5f, 30, 30);

            Material torusMat = new Material();
            torusMat.Diffuse = Color.Yellow.ToVector4();
            torusMat.Specular = Color.White.ToVector4();
            torusMat.SpecularPower = 10;

            torusNode.Material = torusMat;

            TransformNode torusTransNode = new TransformNode();
            torusTransNode.Translation = new Vector3(-2, 0, 3);

            // Since GoblinXNA does not have a predefined shape type
            // of torus, we define the shape to be ConvexHull,
            // which is applicable to any shape, but more computationally
            // expensive compared to the other primitive shape types we have
            // used thus far
            torusNode.Physics.Shape = GoblinXNA.Physics.ShapeType.ConvexHull;
            torusNode.Physics.Pickable = true;
            torusNode.AddToPhysicsEngine = true;

            scene.RootNode.AddChild(torusTransNode);
            torusTransNode.AddChild(torusNode);
        }


    }
}