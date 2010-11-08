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
    class Monster
    {
        /*
        DAMAGE, CRIT, HIT, DODGE, HP, PARRY,
        ADDITIONAL_ATTACK_CHANCE, FIREBREATH_ATTACK_CHANCE, LIGHTNING_ATTACK_CHANCE
         */
        protected TransformNode transNode;  //Instantiated through Monster
        protected int health, power;        //Instantiated through Monster
        protected double hit, dodge, crit, extraAttack, fireBreath, lightningAttack;
        protected double parry;
        protected List<Buff> buffs;
        protected List<int> dmgTaken;       //Instantiated through Monster
        protected List<int> dmgPrevented;   //Instantiated through Monster
        protected string name;              //Instantiated through Monster
        protected Monster nearestEnemy;
        protected CreatureType type;
        protected Vector3 origin;
        protected Boolean animating;

        public Monster NearestEnemy
        {
            get { return nearestEnemy; }
            set { nearestEnemy = value; }
        }

        private bool isDead;

        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }

        public Monster(string name, String model, int health, int power, bool useInternal)
        {
            this.name = name;
            this.health = health;
            this.power = power;
            Random rand = new Random();

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
            transNode.Scale *= 0.15f;
            transNode.Translation += new Vector3(10, 0, 20);
            buffs = new List<Buff>();
        }


        //********* Selectors and Mutators****************

        public TransformNode TransNode
        {
            get { return transNode; }
            set { transNode = value; }
        }

        //*********End Selectors and Mutators*************

        //*********Card-Monster Interactions**************************
        public void addBuff(Buff buff)
        {
            buffs.Add(buff);
        }
        public void addBuffs(List<Buff> buffs)
        {
            if (buffs != null)
                this.buffs.AddRange(buffs);
        }
        public void dealDirectDmg(int dmg)
        {
            dmgTaken.Add(dmg);
        }
        public void preventDmg(int health)
        {
            dmgPrevented.Add(health);
        }
        //*****End Card-Monster Interactions**************************

        //***********Dice-Monster Interactions************************
        public void applyMods()
        {
            foreach (Buff b in buffs)
            {
                if (b.AffectedCreature == CreatureType.ALL || b.AffectedCreature == type)
                    switch (b.Modifier)
                    {
                        case ModifierType.POWER: power += b.Amount;
                            break;
                        case ModifierType.DAMAGE_MOD:
                            break;
                        case ModifierType.CRIT: crit += b.Amount;
                            break;
                        case ModifierType.HIT: hit += b.Amount;
                            break;
                        case ModifierType.DODGE: dodge += b.Amount;
                            break;
                        case ModifierType.HP: health += b.Amount;
                            break;
                        case ModifierType.HP_MOD:
                            break;
                        case ModifierType.PARRY: parry += b.Amount;
                            break;
                        //Type specific attacks
                        case ModifierType.ADDITIONAL_ATTACK_CHANCE: extraAttack += b.Amount;
                            break;
                        case ModifierType.FIREBREATH_ATTACK_CHANCE: fireBreath += b.Amount;
                            break;
                        case ModifierType.LIGHTNING_ATTACK_CHANCE: lightningAttack += b.Amount;
                            break;
                        //End Type specific attacks
                        default:
                            break;
                    }
            }

            if (health <= 0)
                isDead = true;
        }

        public void defend(int dmg, Monster attacker)
        {
            dealDirectDmg(dmg);
            if (RandomHelper.GetRandomInt(100) <= parry)
                attacker.dealDirectDmg(power);
        }
        public void dealAttackDmg()
        {
            if (isDead || nearestEnemy == null)
                return;

            //Needs to be finished
            double chanceToHit = hit - nearestEnemy.dodge;

            if (RandomHelper.GetRandomInt(100) <= chanceToHit)
            {
                if (RandomHelper.GetRandomInt(100) <= crit)
                    nearestEnemy.defend((int)Math.Floor(1.5 * power),this);
                else nearestEnemy.defend(power,this);
            }
            if (RandomHelper.GetRandomInt(100) <= extraAttack || 
                RandomHelper.GetRandomInt(100) <= fireBreath || RandomHelper.GetRandomInt(100) <= lightningAttack)
            {
                if (RandomHelper.GetRandomInt(100) <= crit)
                    nearestEnemy.defend((int)Math.Floor(1.5 * power), this);
                else nearestEnemy.defend(power, this);
            }

        }

        public void damageResolution()
        {
            if (isDead)
                return;
            int dmgRecieved = 0, preventedDmg = 0;

            foreach (int d in dmgTaken)
                dmgRecieved += d;

            foreach (int p in dmgPrevented)
                preventedDmg += p;

            dmgRecieved = dmgRecieved - preventedDmg;

            if (dmgRecieved >= health)
                isDead = true;
        }

        public virtual void startAttackAnimation()
        {

          float modifier;
            //Should create simple "animation" of Monsters atack
          origin = transNode.Translation;
          //Console.Write(DateTime.Now.Second);
          modifier = 0.01f;
          Console.WriteLine("inloop");


            transNode.Translation += new Vector3(modifier, 0, 0);
          /*
          while (DateTime.Now.Millisecond < endMilli)
          {

            transNode.Translation += new Vector3(modifier, 0, 0);
            startMilli += 1;
            
          }
          Console.WriteLine(DateTime.Now.Second);

          *///transNode.Translation = origin;
          transNode.Translation += new Vector3(modifier, 0, 0);
          
        }

        public virtual void endAttackAnimation()
        {
            //Should end simple "animation" of Monsters atack
          transNode.Translation = origin;
        }
        //********End Dice-Monster Interactions********************

        //********Monster-Monster Interactions********************
        private void resetNearest()
        {
            if (nearestEnemy.IsDead)
                nearestEnemy = null;
        }
        //*****End Monster-Monster Interactions********************

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
        int goldCost, manaCost;
        bool useInternal;
        public MonsterBuilder(CreatureID id, CreatureType type, String name, String model, Texture2D inv_texture, int power, int health, bool useInternal, int goldCost, int manaCost)
        {
            this.id = id;
            this.name = name;
            this.health = health;
            this.power = power;
            this.type = type;
            this.model = model;
            this.useInternal = useInternal;
            this.inv_texture = inv_texture;
            this.goldCost = goldCost;
            this.manaCost = manaCost;
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
        public int getGoldCost()
        {
            return goldCost;
        }
        public int getManaCost()
        {
            return manaCost;
        }
        public CreatureType getType()
        {
            return type;
        }
    }

}
