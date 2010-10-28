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
        private int dieNum;

        public Die(Scene s, int dieNum)
        {
            //Set up the 6 sides of this die
            this.dieNum = dieNum;
            sides = new MarkerNode[6];
            int[] side_marker = new int[1];
            for (int i = 0; i < 6; i++)
            {
                side_marker[0] = (dieNum * 6) + firstDieID;
                String config_file = String.Format("Content/dice_markers/die{0}side{1}.txt", dieNum, i);
                sides[i] = new MarkerNode(s.MarkerTracker, config_file, side_marker);
                s.RootNode.AddChild(sides[i]);
            }

            currentMonster = null;
        }

        public void assignMonster(TransformNode t)
        {
            //if (m == null)
              //  throw new Exception("Monster cannot be null");
            //currentMonster = m;
            sides[0].AddChild(t);
        }

        public void removeMonster()
        {
            currentMonster = null;

            //And clear the transnode for the side markers
            //TODO!
        }

        public bool isOnScreen(Vector3 upVector)
        {
            foreach (MarkerNode side in sides)
            {
                if (side.MarkerFound)
                {
                    Vector3 result, sideUp = side.WorldTransformation.Up;
                    Vector3.Cross(ref upVector, ref sideUp, out result);
                    Console.WriteLine(String.Format("{0} - {1}", dieNum, result.ToString()));
                }
            }
            return false;
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
