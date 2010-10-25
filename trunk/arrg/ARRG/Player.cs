using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{
    class Player
    {
        private static int MAX_NUM_DIE = 3;
        private static int MAX_CARDS_IN_DECK = 20;      //SUbject to change
        private static int MAX_CREATURES_ON_DIE = 12;   //Subject to change
        
        private int health;
        private int mana;
        private int gold;

        private Die[] die;
        private int numDie;

        private Monster[] selectedMonsters;
        private int numMonsters;

        private Card[] deck;
        private int numCards;   //May not be necessary

        private List<Monster> monsters;     //List for inventory purposes
        private List<Card> availableCards;  //List for inventory purposes

        public Player()
        {
            health = 20;
            mana = 10;
            gold = 100;
            die = new Die[MAX_NUM_DIE];
            selectedMonsters = new Monster[MAX_CREATURES_ON_DIE];
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

        public bool addMonster(Monster m)
        {
            if (numMonsters < MAX_CREATURES_ON_DIE)
            {
                selectedMonsters[numMonsters++] = m;
                return true;
            }
            return false;
        }
        



    }
}
