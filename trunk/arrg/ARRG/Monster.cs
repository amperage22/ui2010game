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

    public Monster(string name, int health, int power)
    {
      this.name = name;
      this.health = health;
      this.power = power;
    }

    public int getHealth()
    {
      return health;
    }

    public int getPower()
    {
      return power;
    }

    public string getName()
    {
      return name;
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
