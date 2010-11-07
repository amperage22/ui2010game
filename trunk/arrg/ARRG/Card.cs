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
        Monster nearest;
        int dmgDone, dmgPrevent;
        CardType type;
        Buff buff;
        //test
        bool particleSet;
        ParticleNode fireRingEffectNode;
        float radius;
        TransformNode node;
        int frameCount;
        Random random = new Random();

        public Card(Scene s, int markerNum, int mod, ModifierType modType, CreatureType againstCreatureType)
        {
            type = CardType.STAT_MOD;
            buff = new Buff(modType, againstCreatureType, mod);
            marker = new MarkerNode(s.MarkerTracker, markerNum, 30d);
            node = new TransformNode();
            s.RootNode.AddChild(marker);
        }
        public Card(Scene s, CardType type, int markerNum, int mod)
        {
            this.type = type;
            marker = new MarkerNode(s.MarkerTracker, markerNum, 30d);
            switch (type)
            {
                case CardType.DMG_DONE: dmgDone = mod; dmgPrevent = 0; break;
                case CardType.DMG_PREVENT: dmgDone = 0; dmgPrevent = mod; break;
            }
            node = new TransformNode();
            s.RootNode.AddChild(marker);
        }
        public void getNearestCreature(Die[] d1, Die[] d2)
        {
            double distance = 1000;

            foreach (Die d in d1)
            {
                double ds;
                if (d.UpMarker != null && d.CurrentMonster != null)
                {
                    ds = Vector3.Distance(marker.WorldTransformation.Translation, d.UpMarker.WorldTransformation.Translation);

                    if (ds < distance)
                        nearest = d.CurrentMonster;
                }
            }
            foreach (Die d in d2)
            {
                double ds;
                if (d.UpMarker != null && d.CurrentMonster != null)
                {
                    ds = Vector3.Distance(marker.WorldTransformation.Translation, d.UpMarker.WorldTransformation.Translation);

                    if (ds < distance)
                        nearest = d.CurrentMonster;
                }
            }
        }
        public void castSpell()
        {
            if (nearest == null || Vector3.Zero.Equals(marker.WorldTransformation.Translation))
                return;
            switch (type)
            {
                case CardType.STAT_MOD: nearest.addBuff(buff); break;
                case CardType.DMG_DONE: nearest.dealDirectDmg(dmgDone); break;
                case CardType.DMG_PREVENT: nearest.preventDmg(dmgPrevent); break;
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
                fireRingEffectNode.ParticleEffects.Add(new FireParticleEffect());
                fireRingEffectNode.ParticleEffects.Add(new FireParticleEffect());
                fireRingEffectNode.UpdateHandler += new ParticleUpdateHandler(UpdateRingOfFire);

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
                            particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), new Vector3(20, 100, 20));
                    }
                }
                //else if (!Vector3.Zero.Equals(worldTransform.Translation))
                // Add 1 smoke particle every frame
                particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), Vector3.Zero);
            }
        }
        private Vector3 RandomPointOnCircle(Vector3 pos)
        {
            const float radius = 15f;

            double angle = random.NextDouble() * Math.PI * 2;

            float x = (float)Math.Cos(angle);
            float y = (float)Math.Sin(angle);
            float z = (float)random.NextDouble() * 50;

            return new Vector3(x * radius + pos.X, y * radius + pos.Y, z + pos.Z);
        }
    }
}