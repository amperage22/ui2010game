using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using GoblinXNA;
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Graphics.ParticleEffects;
using GoblinXNA.Device.Generic;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;


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

        private Die nearestEnemy;



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
                                upMarker.RemoveChild(currentMonster.TransNode);

                            upMarker = side;
                            addMonster(m.createMonster());
                            return true;
                        }
                    }
                    else if (d > 70)
                    {
                        if (side.Equals(upMarker))
                        {
                            upMarker.RemoveChild(currentMonster.TransNode);
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
            }
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
            currentMonster = null;
            upMarker = null;
            nearestEnemy = null;
        }

        public void setNearestEnemy(Die[] die)
        {
            float prev = 1000, curr;
            Die closest;
            foreach (Die d in die)
            {
                if (d != null && d.CurrentMonster != null)
                {
                    curr = Vector3.Distance(UpMarker.WorldTransformation.Translation, d.UpMarker.WorldTransformation.Translation);
                    if (curr < prev)
                    {
                        prev = curr;
                        closest = d;
                    }
                }

                if (d != null && d.CurrentMonster != null && currentMonster != null)
                {
                    nearestEnemy = d;
                    currentMonster.NearestEnemy = d.currentMonster;
                    currentMonster.applyLine();
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




    }


}
