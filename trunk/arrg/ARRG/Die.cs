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
    public class Die
    {
        private const int firstDieID = 128;

        private MarkerNode[] sides;
        private Monster currentMonster;
        private MarkerNode ground;
        private MarkerNode upMarker;

        public Die(ref Scene s, int dieNum,ref MarkerNode ground)
        {
            //Set up the 6 sides of this die
            sides = new MarkerNode[6];
            int[] side_marker = new int[1];
            for (int i = 0; i < 6; i++)
            {
                side_marker[0] = (dieNum * 6) + firstDieID;
                String config_file = String.Format("Content/dice_markers/die{0}side{1}.txt", dieNum, i);
                sides[i] = new MarkerNode(s.MarkerTracker, config_file, side_marker);
                s.RootNode.AddChild(sides[i]);
            }

            this.ground = ground;
        }

        private bool markerSwitch(MarkerNode side)
        {
            if (side.Equals(upMarker))
            {
                return true;
            }
            return false;
        }

        public void setTopMarker()
        {
            Vector3 groundRightVector = ground.WorldTransformation.Right;
            if (groundRightVector.Equals(Vector3.Zero))
                return;
            foreach (MarkerNode side in sides)
            {
                if (side.MarkerFound)
                {
                    Vector3 sideUp = side.WorldTransformation.Forward;

                    double d = Math.Acos(Vector3.Dot(sideUp, groundRightVector));
                    d = MathHelper.ToDegrees((float)d);
                    if (d <= 92 && d >= 88)
                    {
                        if (!markerSwitch(side))
                        {
                            if(currentMonster != null)
                                upMarker.RemoveChild(currentMonster.TransNode);

                            upMarker = side;
                            addMonster(new Monster("Test", "test", 5, 5));
                            return;
                        }
                    }
                }
            }
        }
        public void addMonster(Monster m)
        {
            if (upMarker != null)
            {
                upMarker.AddChild(m.TransNode);
                currentMonster = m;
            }
        }
        public bool hasMonster() {
            return currentMonster != null;
        }

        public void Update()
        {
            //TODO: Make sure that if the die changes orientation, that the monster remains on the top face
        }
    }
}
