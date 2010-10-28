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
    public class Monster
    {
        protected TransformNode transNode;
        protected int health, power;
        protected double hit, dodge, crit;
        protected List<int> dmgMods;
        protected List<int> healthMods;
        protected List<int> dmgTaken;
        protected List<int> dmgPrevented;
        protected string name;

        public Monster(string name,String model, int health, int power)
        {
            this.name = name;
            this.health = health;
            this.power = power;

            dmgMods = new List<int>();
            healthMods = new List<int>();
            dmgTaken = new List<int>();
            dmgPrevented = new List<int>();
        }
        public Monster() { }

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

        public void addMod(int dmg, int health)
        {
            dmgMods.Add(dmg);
            healthMods.Add(health);
        }
        public void dealDirectDmg(int dmg)
        {
            dmgTaken.Add(dmg);
        }
        public void preventDmg(int health)
        {
            dmgPrevented.Add(health);
        }

        public void newTurn()
        {
            dmgTaken.Clear();
            dmgMods.Clear();
            healthMods.Clear();
            dmgPrevented.Clear();
        }
    }

    class MonsterBuilder
    {
        CreatureType type;
        protected String name;
        protected String model;
        int health;
        int power;
        public MonsterBuilder(CreatureType type,String name,String model, int health, int power)
        {
            this.name = name;
            this.health = health; 
            this.power = power;
            this.type = type;
            this.model = model;
        }
        public Monster createMonster()
        {
            switch (type)
            {
                case CreatureType.BEATS: break;
                case CreatureType.DRAGONKIN: break;
                case CreatureType.ROBOT: break;
            }
            return new Monster(name,model, health, power);
        }
    }

}
