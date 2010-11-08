using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ARRG_Game
{
    class Robots : Monster
    {
        private TransformNode origin;
        Scene scene;
        private Boolean showAttack;
        public Robots(string name, String model, int health, int power, bool useInternal, Scene scene)
            : base(name, model, health, power, useInternal)
        {
            hit = 85;
            dodge = 25;
            crit = 30;
            type = CreatureType.ROBOTS;
            showAttack = false;
            this.scene = scene;
            origin = transNode;
        }
        public override void startAttackAnimation()
        {
            float modifier = 2.0f;
            if (attackTimer-- > 0)
            {
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