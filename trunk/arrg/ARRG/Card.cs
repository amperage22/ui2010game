using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
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
        private MarkerNode marker;
        private Monster nearestMonster;
        private Die nearestDice;
        private int dmgDone, dmgPrevent, manaCost;
        private CardType type;
        private Buff buff;
        public static double BUFF_SCALE = .15;
        private TransformNode node = new TransformNode();
        private TransformNode buffModel;
        protected ParticleLineGenerator line = new ParticleLineGenerator();
        //test
        //bool particleSet;
        //ParticleNode fireRingEffectNode;
        //float radius;
        //TransformNode node;
        //int frameCount;
        //Random random = new Random();

        public Card(int markerNum, int manaCost, int mod, ModifierType modType, CreatureType creatureType)
        {
            this.manaCost = manaCost;
            type = CardType.STAT_MOD;
            buff = new Buff(modType, creatureType, mod);
            marker = new MarkerNode(GlobalScene.scene.MarkerTracker, markerNum, 30d);
            GlobalScene.scene.RootNode.AddChild(marker);
            marker.AddChild(node);
            marker.AddChild(line.addParticle("particleLine2"));
            loadModel();
        }
        public Card(CardType type, int markerNum, int manaCost, int mod)
        {
            this.manaCost = manaCost;
            this.type = type;
            marker = new MarkerNode(GlobalScene.scene.MarkerTracker, markerNum, 30d);
            switch (type)
            {
                case CardType.DMG_DONE: dmgDone = mod; dmgPrevent = 0; break;
                case CardType.DMG_PREVENT: dmgDone = 0; dmgPrevent = mod; break;
            }
            GlobalScene.scene.RootNode.AddChild(marker);
            marker.AddChild(node);
            marker.AddChild(line.addParticle("particleLine2"));
            loadModel();

        }
        private void loadModel()
        {
            buffModel = new TransformNode();
            buffModel.Translation += new Vector3(0, 0, 15);
            marker.AddChild(buffModel);

            String modelName = "";
            if (type == CardType.STAT_MOD)
            {
                if (buff.mod < 0)
                    modelName = "pumpkin";
                else modelName = "sword";

            }
            else if (type == CardType.DMG_PREVENT)
                modelName = "shield";
            else
                modelName = "fireflower";


            buffModel.Scale *= new Vector3(0.05f);

            GeometryNode GeodudeNode = new GeometryNode(modelName);
            GeodudeNode.Model = (Model)(new ModelLoader()).Load("Models/", modelName);
            GeodudeNode.Model.UseInternalMaterials = true;

            buffModel.AddChild(GeodudeNode);
        }
        public void getNearestCreature(Die[] d1, Die[] d2)
        {

            if (Vector3.Zero.Equals(marker.WorldTransformation.Translation))
                return;
            double distance = 100000;

            foreach (Die d in d1)
            {
                double ds;
                if (d.UpMarker != null && d.CurrentMonster != null)
                {
                    Vector3 tar = d.UpMarker.WorldTransformation.Translation;
                    tar.Z = 0;
                    ds = Vector3.Distance(marker.WorldTransformation.Translation, tar);

                    if (ds < distance)
                    {
                        nearestMonster = d.CurrentMonster;
                        nearestDice = d;
                        distance = ds;
                    }
                }
            }
            foreach (Die d in d2)
            {
                double ds;
                if (d.UpMarker != null && d.CurrentMonster != null)
                {
                    Vector3 tar = d.UpMarker.WorldTransformation.Translation;
                    tar.Z = 0;
                    ds = Vector3.Distance(marker.WorldTransformation.Translation, tar);

                    if (ds < distance)
                    {
                        nearestDice = d;
                        nearestMonster = d.CurrentMonster;
                        distance = ds;
                    }
                }
            }
            if (nearestMonster != null && !Vector3.Zero.Equals(marker.WorldTransformation.Translation))
                applyLine(marker.WorldTransformation.Translation, nearestDice.UpMarker.WorldTransformation.Translation);
        }
        public void castSpell()
        {
            if (nearestMonster == null || Vector3.Zero.Equals(marker.WorldTransformation.Translation))
                return;
            switch (type)
            {
                case CardType.STAT_MOD: nearestMonster.addBuff(buff);
                    if (buff.Amount > 0)
                        nearestMonster.TransNode.Scale /= (float)(buff.Amount * BUFF_SCALE);
                    else if (buff.Amount < 0) nearestMonster.TransNode.Scale *= (float)(-buff.Amount * BUFF_SCALE);
                    break;
                case CardType.DMG_DONE: nearestMonster.dealDirectDmg(dmgDone); break;
                case CardType.DMG_PREVENT: nearestMonster.preventDmg(dmgPrevent); break;
            }
        }
        public void Update(GameTime gameTime)
        {
            if (buffModel == null) return;
            buffModel.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, gameTime.ElapsedRealTime.Milliseconds * MathHelper.ToRadians(.125f));
        }
        public void applyLine(Vector3 source, Vector3 target)
        {
            line.update(source, target);
        }
        //public void update()
        //{
        //    if (marker.MarkerFound && !particleSet)
        //    {
        //        SmokePlumeParticleEffect smokeParticles = new SmokePlumeParticleEffect();
        //        FireParticleEffect fireParticles = new FireParticleEffect();
        //        //fireParticles.TextureName = "particles";
        //        smokeParticles.DrawOrder = 200;
        //        fireParticles.DrawOrder = 300;
        //        fireRingEffectNode = new ParticleNode();
        //        fireRingEffectNode.ParticleEffects.Add(smokeParticles);
        //        fireRingEffectNode.ParticleEffects.Add(fireParticles);
        //        fireRingEffectNode.ParticleEffects.Add(new FireParticleEffect());
        //        fireRingEffectNode.ParticleEffects.Add(new FireParticleEffect());
        //        fireRingEffectNode.UpdateHandler += new ParticleUpdateHandler(UpdateRingOfFire);

        //        node.AddChild(fireRingEffectNode);
        //        marker.AddChild(node);
        //        particleSet = true;

        //    }

        //}
        //private void UpdateRingOfFire(Matrix worldTransform, List<ParticleEffect> particleEffects)
        //{
        //    foreach (ParticleEffect particle in particleEffects)
        //    {
        //        if (particle is FireParticleEffect)
        //        {
        //            // Add 10 fire particles every frame
        //            for (int k = 0; k < 20; k++)
        //            {
        //                if (!Vector3.Zero.Equals(worldTransform.Translation))
        //                    particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), new Vector3(20, 100, 20));
        //            }
        //        }
        //        //else if (!Vector3.Zero.Equals(worldTransform.Translation))
        //        // Add 1 smoke particle every frame
        //        particle.AddParticle(RandomPointOnCircle(worldTransform.Translation), Vector3.Zero);
        //    }
        //}
        //private Vector3 RandomPointOnCircle(Vector3 pos)
        //{
        //    const float radius = 15f;

        //    double angle = random.NextDouble() * Math.PI * 2;

        //    float x = (float)Math.Cos(angle);
        //    float y = (float)Math.Sin(angle);
        //    float z = (float)random.NextDouble() * 50;

        //    return new Vector3(x * radius + pos.X, y * radius + pos.Y, z + pos.Z);
        //}
    }
}