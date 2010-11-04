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

    class Card
    {
        MarkerNode marker;
        Random random = new Random();
        ParticleNode fireRingEffectNode;
        bool particleSet = false;
        
        TransformNode node;

        public TransformNode Node
        {
            get { return node; }
            set { node = value; }
        }

        private int healthMod, dmgMod, dmgDone, dmgPrevent;
        private CardType type;

        public Card(ref Scene s, CardType type, int markerNum, int dmg, int health)
        {
            this.type = type;


            node = new TransformNode();
            marker = new MarkerNode(s.MarkerTracker, markerNum, 30d);


            switch (type)
            {
                case CardType.STAT_MOD: dmgMod = dmg; dmgDone = 0; dmgPrevent = 0; healthMod = health; break;
                case CardType.DMG_DONE: dmgMod = 0; dmgDone = dmg; dmgPrevent = 0; healthMod = 0; break;
                case CardType.DMG_PREVENT: dmgMod = 0; dmgDone = 0; dmgPrevent = health; healthMod = 0; break;
            }


            s.RootNode.AddChild(marker);
        }
        public Card(ref Scene s, CardType type, int markerNum, int mod)
        {
            this.type = type;

            //Set up the 6 sides of this die
            node = new TransformNode();
            marker = new MarkerNode(s.MarkerTracker, markerNum, 30d);
            //test code

            TransformNode trans = new TransformNode();
            trans.Translation = new Vector3(0, 0, 10);
            marker.AddChild(trans);
            //End test

            switch (type)
            {
                case CardType.DMG_DONE: dmgMod = 0; dmgDone = mod; dmgPrevent = 0; healthMod = 0; break;
                case CardType.DMG_PREVENT: dmgMod = 0; dmgDone = 0; dmgPrevent = mod; healthMod = 0; break;
            }

            s.RootNode.AddChild(marker);
        }
        public void castSpell(Monster m)
        {
            switch (type)
            {
                case CardType.STAT_MOD: m.addMod(dmgMod, healthMod); break;
                case CardType.DMG_DONE: m.dealDirectDmg(dmgDone); break;
                case CardType.DMG_PREVENT: m.preventDmg(dmgPrevent); break;
            }
        }
        public void update()
        {
            if (marker.MarkerFound && !particleSet)
            {
                SmokePlumeParticleEffect smokeParticles = new SmokePlumeParticleEffect();
                FireParticleEffect fireParticles = new FireParticleEffect();
                //fireParticles.TextureName = "particles";
                smokeParticles.DrawOrder = 200;
                fireParticles.DrawOrder = 300;
                fireRingEffectNode = new ParticleNode();
                fireRingEffectNode.ParticleEffects.Add(smokeParticles);
                fireRingEffectNode.ParticleEffects.Add(fireParticles);
                fireRingEffectNode.UpdateHandler += new ParticleUpdateHandler(UpdateRingOfFire);
                fireRingEffectNode.Enabled = true;

                node.AddChild(fireRingEffectNode);
                marker.AddChild(node);
                particleSet = true;

            }

        }
        private void UpdateRingOfFire(Matrix worldTransform, List<ParticleEffect> particleEffects)
        {
            foreach (ParticleEffect particle in particleEffects)
            {
                if (particle is FireParticleEffect)
                {
                    // Add 10 fire particles every frame
                    for (int k = 0; k < 20; k++)
                    {
                        if (!Vector3.Zero.Equals(worldTransform.Translation))
                            particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), new Vector3(20,100,20));
                    }
                }
                else if (!Vector3.Zero.Equals(worldTransform.Translation))
                    // Add 1 smoke particle every frame
                    particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), new Vector3(50, 100, 50));
            }
        }
        private Vector3 RandomPointOnCircle(Vector3 pos)
        {
            const float radius = 12.5f;

            double angle = random.NextDouble() * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);

            return new Vector3(x * radius + pos.X, y * radius + pos.Y, pos.Z);
        }
    }
    class CardBuilder
    {
        Scene s;
        CardType type;
        int markerNum;
        int dmg, health, mod;

        public CardBuilder(Scene s, CardType type, int markerNum, int dmg, int health)
        {
            this.s = s;
            this.type = type;
            this.markerNum = markerNum;
            this.dmg = dmg;
            this.health = health;
        }

        public CardBuilder(Scene s, CardType type, int markerNum, int mod)
        {
            this.s = s;
            this.type = type;
            this.markerNum = markerNum;
            this.mod = mod;
        }
        public Card createCard()
        {
            switch (type)
            {
                case CardType.STAT_MOD: return new Card(ref s, type, markerNum, dmg, health);
                case CardType.DMG_DONE:
                case CardType.DMG_PREVENT: return new Card(ref s, type, markerNum, mod);
            }
            return null;

        }

    }
}
