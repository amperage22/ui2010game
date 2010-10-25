using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    class Player
    {
        private static int MAX_NUM_DIE = 3;
        private int health;
        private int mana;
        private int gold;

        
        private Die[] die;
        private int numDie;
        private List<Monster> monsters; //Ava
        private List<Card> availableCards;

        public Player()
        {
            health = 20;
            mana = 10;
            gold = 100;
            die = new Die[MAX_NUM_DIE];
            monsters = new List<Monster>();
            availableCards = new List<Card>();
            numDie = 0;
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

        public bool addDie(Die d)
        {
            if (numDie < 3)
            {
                die[numDie++] = d;
                return true;
            }
            return false;
        }



    }
}
