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
        public const int MAX_NUM_DIE = 3;
        public const int MAX_CREATURES_INGAME = 6;
        private healthBar healthBar;

        //*****Player stats****
        private int health;
        private int mana;
        private int gold;
        private Scene scene;
        private int playerNum;
        //***End player stats***

        private MarkerNode ground;

        private Die[] die;


        internal Die[] Die
        {
            get { return die; }
            set { die = value; }
        }

        private List<MonsterBuilder> selectedMonsters;
        public List<MonsterBuilder> SelectedMonsters
        {
            get { return selectedMonsters; }
            set { selectedMonsters = value; }
        }
        private List<MonsterBuilder> purchasedMonsters;     //List for inventory purposes
        internal List<MonsterBuilder> PurchasedMonsters
        {
            get { return purchasedMonsters; }
            set { purchasedMonsters = value; }
        }
        private List<Buff> buffs;  //Technically the talents of the player

        /**
         * Sets up the player object.
         * s The scene object...
         * firstDieNum Which die is being created?  Die1, 2, 3?
         * numDice The amount of dice this player will get
         */
        public Player(Scene s, int playerNum, MarkerNode ground)
        {
            health = 20;
            buffs = new List<Buff>();
            mana = 10;
            gold = 100;
            die = new Die[MAX_NUM_DIE];
            this.ground = ground;
            selectedMonsters = new List<MonsterBuilder>();
            //healthBar = new healthBar(s, playerNum, health, mana);
            this.scene = s;
            this.playerNum = playerNum;

            /* Create the player's dice */
            for (int i = 0; i < MAX_NUM_DIE; i++)
                die[i] = new Die(s, i + (playerNum - 1) * MAX_NUM_DIE, ground);

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
        public List<Buff> Buffs
        {
            get { return buffs; }
            set { buffs = value; }
        }
        public void updateDraw()
        {
            foreach (Die d in die)
                d.reset();
        }

        public void updateSummon()
        {
            if (purchasedMonsters.Count == 0) return;
            foreach (Die d in die)
            {
                Random r = new Random();
                d.setTopMarker(purchasedMonsters[r.Next(purchasedMonsters.Count)]);  //Randomly attach monster to die
            }
        }
        public void updateAttack(Die[] die2)
        {
            foreach (Die d in die)
            {
                if (d != null && d.CurrentMonster != null)
                {
                    d.setNearestEnemy(die2);
                }
            }
        }
        public void updateDamage()
        {
            foreach (Die d in die)
            {
                if (d != null && d.CurrentMonster != null)
                {
                    d.CurrentMonster.startAttackAnimation();
                }
            }
        }

        public void applyHealthMods()
        {
            foreach (Die d in die)
            {
                if (d != null)
                {
                    d.addBuffs(buffs);
                    d.applyHealthMod();
                }
            }
        }

        public void applyMonsterDmg()
        {
            foreach (Die d in die)
            {
                if (d != null)
                    d.applyMonsterDmg();
            }
        }
        public void damageResolution()
        {
            foreach (Die d in die)
            {
                if (die != null)
                    d.resloveDamage();
            }
            if (healthBar != null)
            {
                healthBar.adjustHealth(health);
                healthBar.adjustMana(mana);
            }
        }
        public void applyPlayerDamage(Player other)
        {
            if (other == null)
                return;

            int myCount = 0, theirCount = 0;

            foreach (Die d in die)
                if (d.CurrentMonster != null && !d.CurrentMonster.IsDead)
                    myCount++;

            foreach (Die d in other.Die)
                if (d.CurrentMonster != null && !d.CurrentMonster.IsDead)
                    theirCount++;
            if (myCount < theirCount)
            {
                //healthBar.adjustHealth(-1);
                health--;
            }
        }

        public void showHealth()
        {
            healthBar = new healthBar(scene, playerNum, health, mana);
        }
    }
}
