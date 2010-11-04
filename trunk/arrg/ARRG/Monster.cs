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

using Model = GoblinXNA.Graphics.Model;
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
        protected TransformNode transNode;  //Instantiated through Monster
        protected int health, power;        //Instantiated through Monster
        protected double hit, dodge, crit;  //Instantiated through Child classes
        protected List<int> dmgMods;        //Instantiated through Monster
        protected List<int> healthMods;     //Instantiated through Monster
        protected List<int> dmgTaken;       //Instantiated through Monster
        protected List<int> dmgPrevented;   //Instantiated through Monster
        protected string name;              //Instantiated through Monster
        protected Monster nearestEnemy;

        public Monster NearestEnemy
        {
            get { return nearestEnemy; }
            set { nearestEnemy = value; }
        }

        private bool isDead;

        protected bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }

        public Monster(string name, String model, int health, int power, bool useInternal)
        {
            this.name = name;
            this.health = health;
            this.power = power;

            dmgMods = new List<int>();
            healthMods = new List<int>();
            dmgTaken = new List<int>();
            dmgPrevented = new List<int>();
            ModelLoader loader = new ModelLoader();
            Model monsterModel = (Model)loader.Load("Models/", model);
            GeometryNode monsterNode = new GeometryNode("Robot");
            monsterNode.Model = monsterModel;
            if (!useInternal)
            {
                Material monsterMaterial = new Material();
                monsterMaterial.Diffuse = Color.Green.ToVector4();
                monsterMaterial.Specular = Color.White.ToVector4();
                monsterMaterial.SpecularPower = 2;
                monsterMaterial.Emissive = Color.Black.ToVector4();
                monsterNode.Material = monsterMaterial;

            }
            else
                monsterNode.Model.UseInternalMaterials = true;

            transNode = new TransformNode();
            transNode.AddChild(monsterNode);
            transNode.Scale *= 0.10f;
            transNode.Translation += new Vector3(10, 0, 20);
        }


        //********* Selectors and Mutators****************
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
        public TransformNode TransNode
        {
            get { return transNode; }
            set { transNode = value; }
        }
        //*********End Selectors and Mutators*************

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

        private void damageResolution()
        {
            if (isDead)
                return;
            int dmg = 0, prevent = 0;

            foreach (int d in dmgTaken)
                dmg += d;

            foreach (int p in dmgPrevented)
                prevent += p;

            dmg = dmg - prevent;

            if (dmg >= health)
                isDead = true;

            dmgTaken.Clear();
            dmgMods.Clear();
            healthMods.Clear();
            dmgPrevented.Clear();
        }
        private void healthModResolution()
        {
            int hMod =0;

            foreach (int hp in healthMods)
                hMod += hp;

            hMod += health;

            if (hMod <= 0)
                isDead = true;
            

        }

        public void dealAttackDmg()
        {
            if (isDead)
                return;
            int dmgMod = 0;
            foreach (int dmg in dmgMods)
                dmgMod+= dmg;
            nearestEnemy.dealDirectDmg(power + dmgMod);

        }
        public void startAttackAnimation()
        {
            //Should create simple "animation" of Monsters atack
        }

        public void endAttackAnimation()
        {
            //Should end simple "animation" of Monsters atack
        }

    }

    class MonsterBuilder
    {
        CreatureID id;
        CreatureType type;
        protected String name;
        protected String model;
        protected Texture2D inv_texture; //Texture that shows in the market and inventory
        int health;
        int power;
        int cost;
        bool useInternal;
        public MonsterBuilder(CreatureID id, CreatureType type, String name, String model, Texture2D inv_texture, int health, int power, bool useInternal, int cost)
        {
            this.id = id;
            this.name = name;
            this.health = health;
            this.power = power;
            this.type = type;
            this.model = model;
            this.useInternal = useInternal;
            this.inv_texture = inv_texture;
            this.cost = cost;
        }
        public Monster createMonster()
        {
            switch (type)
            {
                case CreatureType.BEASTS: return new Beasts(name, model, health, power, useInternal); 
                case CreatureType.DRAGONKIN: return new Dragonkin(name, model, health, power, useInternal);
                case CreatureType.ROBOTS: return new Robots(name, model, health, power, useInternal);
            }
            return new Monster(name, model, health, power, useInternal);
        }
        public int getID()
        {
            return (int)id;
        }
        public Texture2D getInvTexture()
        {
            return inv_texture;
        }
        public String getName()
        {
            return name;
        }
        public int getCost()
        {
            return cost;
        }
        public CreatureType getType()
        {
            return type;
        }
    }

}
