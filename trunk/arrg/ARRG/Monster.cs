using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ARRG_Game
{

    class Monster
    {

        private int health, power;
        private string name;

        public Monster(string name, int health, int power, Player player, Die die)
        {
            this.name = name;
            this.health = health;
            this.power = power;
        }

        public int Health
        {
          get { return health; }
          set { health = value; }
        }

        public int Power
        {
          get { return power; }
          set { power = value; }
        }

        public String Name
        {
          get { return name; }
          set { name = value; }
        }

        public void adjustPower(int mod)
        {
            power = power + mod;
        }

        public void adjustHealth(int mod)
        {
            health = health + mod;
        }

    }
}
