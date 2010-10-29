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
    public class Player
    {
        private static int MAX_NUM_DIE = 3;
        private static int MAX_CARDS_IN_DECK = 20;      //SUbject to change
        private static int MAX_CREATURES_INGAME = 8;
        
        private int health;
        private int mana;
        private int gold;

        private MarkerNode ground;

        private Die[] die;

        private Monster[] selectedMonsters;
        private int numMonsters;

        private int numCards;   //May not be necessary

        bool x = true;
        private List<Monster> monsters;     //List for inventory purposes
        private List<Card> availableCards;  //List for inventory purposes

        /**
         * Sets up the player object.
         * s The scene object...
         * firstDieNum Which die is being created?  Die1, 2, 3?
         * numDice The amount of dice this player will get
         */
        public Player(Scene s, int playerNum, MarkerNode ground)
        {
            health = 20;
            mana = 10;
            gold = 100;
            die = new Die[MAX_NUM_DIE];
            this.ground = ground;
            selectedMonsters = new Monster[MAX_CREATURES_INGAME];
            monsters = new List<Monster>();
            availableCards = new List<Card>();

            /* Create the player's dice */
            for (int i = 0; i < MAX_NUM_DIE; i++)
            {
                die[i] = new Die(ref s, i + (playerNum - 1) * MAX_NUM_DIE,ref ground);
            }
        }


        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        public int Mana
        {
            get { return mana; }
            set { mana = value; }
        }

        internal Die[] Die
        {
            get { return die; }
            set { die = value; }
        }

        internal List<Monster> Monsters
        {
            get { return monsters; }
            set { monsters = value; }
        }

        internal List<Card> AvailableCards
        {
            get { return availableCards; }
            set { availableCards = value; }
        }

        public int Gold
        {
            get { return gold; }
            set { gold = value; }
        }

        public bool addMonster(Monster m)
        {
            if (numMonsters < MAX_CREATURES_INGAME)
            {
                selectedMonsters[numMonsters++] = m;
                return true;
            }
            return false;
        }

        public void Update()
        {
            foreach (Die d in die)
            {
                d.setTopMarker();
                    ////Create mat for blue cylinder
                    //Material mat = new Material();
                    //mat.Diffuse = new Vector4(0, 1, 0, 1);
                    //mat.Specular = Color.White.ToVector4();
                    //mat.SpecularPower = 10;
                    //mat.Emissive = Color.Blue.ToVector4();

                    ////Create the blue cylinder for the die marker
                    //GeometryNode cylinderNode = new GeometryNode("Cylinder");
                    //cylinderNode.Model = new Cylinder(10, 10, 10, 10);
                    //cylinderNode.Material = mat;
                    //TransformNode cylinderTransNode = new TransformNode();
                    //cylinderTransNode.AddChild(cylinderNode);
                    //cylinderTransNode.Translation = new Vector3(0, 0, 25);

                    //d.assignMonster(cylinderTransNode);

                    //x = false;
                    

            }
        }


    }
}
