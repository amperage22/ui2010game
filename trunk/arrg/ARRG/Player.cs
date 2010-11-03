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
    class Player
    {
        private static int MAX_NUM_DIE = 3;
        private static int MAX_CREATURES_INGAME = 6;

        //*****Player stats****
        private int health;
        private int mana;
        private int gold;
        //***End player stats***

        private MarkerNode ground;

        private Die[] die;
        private List<Card> cards;

        internal List<Card> Cards
        {
            get { return cards; }
            set { cards = value; }
        }

        internal Die[] Die
        {
            get { return die; }
            set { die = value; }
        }

        private Monster[] selectedMonsters;
        private int numMonsters;


        private List<MonsterBuilder> purchasedMonsters;     //List for inventory purposes

        internal List<MonsterBuilder> PurchasedMonsters
        {
            get { return purchasedMonsters; }
            set { purchasedMonsters = value; }
        }
        private List<Talent> talents;  //List for inventory purposes

        private Talent[] activeTalents;

        /**
         * Sets up the player object.
         * s The scene object...
         * firstDieNum Which die is being created?  Die1, 2, 3?
         * numDice The amount of dice this player will get
         */
        public Player(ref Scene s, int playerNum, ref MarkerNode ground)
        {
            health = 20;
            mana = 10;
            gold = 100;
            die = new Die[MAX_NUM_DIE];
            this.ground = ground;
            selectedMonsters = new Monster[MAX_CREATURES_INGAME];

            /* Create the player's dice */
            for (int i = 0; i < MAX_NUM_DIE; i++)
                die[i] = new Die(ref s, i + (playerNum - 1) * MAX_NUM_DIE, ref ground);

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
        public int Gold
        {
            get { return gold; }
            set { gold = value; }
        }
        public Monster[] SelectedMonsters
        {
            get { return selectedMonsters; }
            set { selectedMonsters = value; }
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

        public void updateDraw()
        {
            foreach (Die d in die)
                d.reset();
        }

        public void updateSummon()
        {
            foreach (Die d in die)
            {
                Random r = new Random();
                d.setTopMarker(purchasedMonsters[r.Next(purchasedMonsters.Count)]);  //Randomly attach monster to die
            }
        }
        public void updateAtack(Die[] die2)
        {
            foreach (Die d in die)
            {
                if (d != null && d.CurrentMonster != null)
                {
                    d.setNearestEnemy(die2);
                }
            }
            if (cards == null)
                return;
            foreach (Card c in cards)
                c.update();
        }

        public void updateDamage(Die[] die2)
        {
        }
    }
}
