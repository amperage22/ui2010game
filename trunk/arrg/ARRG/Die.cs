using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using GoblinXNA.Helpers;
using GoblinXNA.SceneGraph;


using Microsoft.Xna.Framework.Graphics;

using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics;


namespace ARRG_Game
{
    class Die
    {
        private const int firstDieID = 128;
        private const double PARALLEL_ANGLE = 0;
        private const double FLOOR_TOLERANCE = 20;

        private MarkerNode[] sides;
        private Monster currentMonster;
        private MarkerNode ground;
        private MarkerNode upMarker;
        private int dieNum;
        private Die nearestEnemy;
        protected GeometryNode discNode;
        protected TransformNode transNode;

        public static Vector4 getDiscColor(int die)
        {
            Vector4 color;
            switch (die)
            {
                case 0: color = Color.Red.ToVector4(); break;
                case 1: color = Color.Green.ToVector4(); break;
                case 2: color = Color.Cyan.ToVector4(); break;
                case 3: color = Color.White.ToVector4(); break;
                case 4: color = Color.Black.ToVector4(); break;
                case 5: color = Color.Blue.ToVector4(); break;
                default: color = Color.Blue.ToVector4(); break;
            }
            return color;

        }

        public MarkerNode UpMarker
        {
            get { return upMarker; }
            set { upMarker = value; }
        }
        Scene scene;

        public Die(Scene s, int dieNum, MarkerNode ground)
        {
            //Set up the 6 sides of this die
            sides = new MarkerNode[6];
            for (int i = 0; i < 6; i++)
            {
                sides[i] = new MarkerNode(s.MarkerTracker, (dieNum * 6) + firstDieID + i, 40d);
                s.RootNode.AddChild(sides[i]);
            }
            this.ground = ground;
            scene = s;
            this.dieNum = dieNum;
        }

        private bool markerSwitch(MarkerNode side)
        {
            if (side.Equals(upMarker))
            {
                return true;
            }
            return false;
        }

        public bool setTopMarker(MonsterBuilder m)
        {
            Vector3 groundForwardVectr = ground.WorldTransformation.Forward;
            if (groundForwardVectr.Equals(Vector3.Zero))
                return false;
            foreach (MarkerNode side in sides)
            {
                if (!Vector3.Zero.Equals(side.WorldTransformation.Translation))
                {
                    Vector3 sideUp = side.WorldTransformation.Forward;

                    double d = Math.Acos(Vector3.Dot(sideUp, groundForwardVectr));
                    d = MathHelper.ToDegrees((float)d);
                    if (d <= PARALLEL_ANGLE + FLOOR_TOLERANCE && d >= PARALLEL_ANGLE - FLOOR_TOLERANCE)
                    {
                        if (!markerSwitch(side))
                        {
                            if (currentMonster != null)
                            {
                                upMarker.RemoveChild(currentMonster.TransNode);
                                removeDisc();
                            }

                            upMarker = side;
                            addMonster(m.createMonster(dieNum));
                            return true;
                        }
                    }
                    else if (d > 70)
                    {
                        if (side.Equals(upMarker))
                        {
                            upMarker.RemoveChild(currentMonster.TransNode);
                            removeDisc();
                            currentMonster = null;
                            upMarker = null;
                        }
                    }
                }
            }
            return false;
        }
        private void addMonster(Monster m)
        {
            if (upMarker != null)
            {
                upMarker.AddChild(m.TransNode);
                currentMonster = m;
                addDisc();
            }
        }
        private void addDisc()
        {
            ModelLoader loader = new ModelLoader();
            Model discModel = (Model)loader.Load("Models/", "disc");
            discNode = new GeometryNode("Disc");
            discNode.Model = discModel;

            Material discMaterial = new Material();
            discMaterial.Diffuse = getDiscColor();
            discMaterial.Specular = Color.White.ToVector4();
            discMaterial.SpecularPower = 2;
            discMaterial.Emissive = Color.Black.ToVector4();
            discNode.Material = discMaterial;


            transNode = new TransformNode();
            transNode.AddChild(discNode);
            transNode.Scale *= 0.07f;
            transNode.Translation += new Vector3(-15, 0, 5);
            upMarker.AddChild(transNode);
        }
        private void removeDisc()
        {
            if (upMarker != null)
                upMarker.RemoveChild(transNode);
        }
        private Vector4 getDiscColor()
        {
            return getDiscColor(dieNum);
        }

        public Monster CurrentMonster
        {
            get { return currentMonster; }
            set { currentMonster = value; }
        }
        public void reset()
        {
            if (currentMonster != null)
                upMarker.RemoveChild(currentMonster.TransNode);
            removeDisc();
            currentMonster = null;
            upMarker = null;
            nearestEnemy = null;
        }

        public void setNearestEnemy(Die[] die)
        {
            float prev = 1000, curr;
            foreach (Die d in die)
            {
                if (d != null && d.CurrentMonster != null && currentMonster != null)
                {
                    curr = Vector3.Distance(UpMarker.WorldTransformation.Translation, d.UpMarker.WorldTransformation.Translation);
                    if (curr < prev)
                    {
                        prev = curr;
                        nearestEnemy = d;
                        currentMonster.NearestEnemy = d.currentMonster;
                        currentMonster.applyLine(upMarker.WorldTransformation.Translation, nearestEnemy.UpMarker.WorldTransformation.Translation);
                    }
                }
            }

        }
        public void setWeakestEnemy(Die[] die)
        {
            foreach (Die d in die)
            {
                if (d != null && d.CurrentMonster != null && currentMonster != null)
                {
                    if (d.currentMonster.Health <= currentMonster.Power)
                    {
                        if (nearestEnemy == null || d.currentMonster.Health > nearestEnemy.currentMonster.Health)
                        {
                            nearestEnemy = d;
                            currentMonster.NearestEnemy = d.currentMonster;
                            //currentMonster.applyLine(upMarker.WorldTransformation.Translation, nearestEnemy.UpMarker.WorldTransformation.Translation);
                        }
                    }
                }
            }
        }
        public void addBuffs(List<Buff> buffs)
        {
            if (currentMonster != null)
                currentMonster.addBuffs(buffs);
        }
        //*****Monster-Dice interactions*******
        public void applyHealthMod()
        {
            if (currentMonster == null)
                return;
            currentMonster.applyMods();
            if (currentMonster.IsDead)
            {
                upMarker.RemoveChild(currentMonster.TransNode);
                currentMonster = null;
            }
        }

        public void applyMonsterDmg()
        {
            if (currentMonster == null || currentMonster.IsDead)
                return;
            currentMonster.dealAttackDmg();
        }
        public void resloveDamage()
        {
            if (currentMonster == null || currentMonster.IsDead)
                return;
            currentMonster.damageResolution();
            if (currentMonster.IsDead)
            {
                upMarker.RemoveChild(currentMonster.TransNode);
                currentMonster = null;
            }
        }
        //*****Monster-Dice interactions*******


        public int DieNum
        {
            get { return dieNum; }
        }

    }


}
