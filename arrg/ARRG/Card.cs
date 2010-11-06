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

        public Card(Scene s, int markerNum, int mod, ModifierType modType, CreatureType againstCreatureType)
        {
            type = CardType.STAT_MOD;
            buff = new Buff(modType, againstCreatureType, mod);
            marker = new MarkerNode(s.MarkerTracker, markerNum, 30d);
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

            s.RootNode.AddChild(marker);
        }
        public void getNearestCreature(Die[] d1, Die[] d2)
        {
            double distance = 1000;

            foreach (Die d in d1)
            {
                if (d.UpMarker == null && d.CurrentMonster == null) break;

                double ds = Vector3.Distance(marker.WorldTransformation.Translation, d.UpMarker.WorldTransformation.Translation);

                if (ds < distance)
                    nearest = d.CurrentMonster;
            }
            foreach (Die d in d2)
            {
                if (d.UpMarker == null && d.CurrentMonster == null) break;

                double ds = Vector3.Distance(marker.WorldTransformation.Translation, d.UpMarker.WorldTransformation.Translation);

                if (ds < distance)
                    nearest = d.CurrentMonster;
            }
        }
        public void castSpell()
        {
            if (nearest == null)
                return;
            switch (type)
            {
                case CardType.STAT_MOD: nearest.addBuff(buff); break;
                case CardType.DMG_DONE: nearest.dealDirectDmg(dmgDone); break;
                case CardType.DMG_PREVENT: nearest.preventDmg(dmgPrevent); break;
            }
        }
        //    public void update()
        //    {
        //        if (marker.MarkerFound && !particleSet)
        //        {
        //            SmokePlumeParticleEffect smokeParticles = new SmokePlumeParticleEffect();
        //            FireParticleEffect fireParticles = new FireParticleEffect();
        //            //fireParticles.TextureName = "particles";
        //            smokeParticles.DrawOrder = 200;
        //            fireParticles.DrawOrder = 300;
        //            fireRingEffectNode = new ParticleNode();
        //            fireRingEffectNode.ParticleEffects.Add(smokeParticles);
        //            fireRingEffectNode.ParticleEffects.Add(fireParticles);
        //            fireRingEffectNode.UpdateHandler += new ParticleUpdateHandler(UpdateRingOfFire);
        //            fireRingEffectNode.Enabled = true;

        //            node.AddChild(fireRingEffectNode);
        //            marker.AddChild(node);
        //            particleSet = true;

        //        }

        //    }
        //    private void UpdateRingOfFire(Matrix worldTransform, List<ParticleEffect> particleEffects)
        //    {
        //        foreach (ParticleEffect particle in particleEffects)
        //        {
        //            if (particle is FireParticleEffect)
        //            {
        //                // Add 10 fire particles every frame
        //                for (int k = 0; k < 30; k++)
        //                {
        //                    if (!Vector3.Zero.Equals(worldTransform.Translation))
        //                        particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), new Vector3(2, 2, 2));
        //                }
        //            }
        //            else if (!Vector3.Zero.Equals(worldTransform.Translation))
        //                // Add 1 smoke particle every frame
        //                particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), new Vector3(2 + 1, 2 + 1, 2 + 1));
        //        }
        //    }
        //    private Vector3 RandomPointOnCircle(Vector3 pos)
        //    {

        //        if(frameCount++ >= 500 && radius <= 40)
        //        {
        //            frameCount = 0;
        //            radius += .001f;
        //        }
        //        if (radius > 40)
        //            return Vector3.Zero;

        //        double theta = random.NextDouble() * Math.PI;
        //        double angle = random.NextDouble() * Math.PI * 2;

        //        float x = radius * (float)Math.Sin(theta) * (float)Math.Cos(angle);
        //        float y = radius * (float)Math.Sin(theta) * (float)Math.Sin(angle);
        //        Random rand = new Random();
        //        float z = radius * (float)Math.Cos(theta);

        //        return new Vector3(x * radius + pos.X, y * radius + pos.Y, z + pos.Z * 2);
        //    }
    }
}