using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics.ParticleEffects;

namespace ARRG_Game
{
    class ParticleLineGenerator
    {
        Vector3 source, target;
        ParticleNode lineEffectNode;

        public ParticleNode addParticle(String name)
        {
            ParticleEffect lineParticles = new FireParticleEffect();
            lineParticles.TextureName = name;
            lineParticles.MaxHorizontalVelocity = lineParticles.MinHorizontalVelocity;
            lineParticles.MaxVerticalVelocity = lineParticles.MinVerticalVelocity;
            lineParticles.MinStartSize = 2f;
            lineParticles.MaxStartSize = 2f;
            lineParticles.MaxEndSize = lineParticles.MaxStartSize;
            lineParticles.MinEndSize = lineParticles.MinStartSize;
            lineParticles.EndVelocity = 0;
            lineParticles.MinColor = new Color(0, 0, 0, 255);
            lineParticles.MaxColor = new Color(255, 255, 255, 255);
            lineEffectNode = new ParticleNode();
            lineEffectNode.ParticleEffects.Add(lineParticles);

            lineParticles = new FireParticleEffect();
            lineParticles.TextureName = name;
            lineParticles.MaxHorizontalVelocity = lineParticles.MinHorizontalVelocity;
            lineParticles.MaxVerticalVelocity = lineParticles.MinVerticalVelocity;
            lineParticles.MinStartSize = 2f;
            lineParticles.MaxStartSize = 2f;
            lineParticles.MaxEndSize = lineParticles.MaxStartSize;
            lineParticles.MinEndSize = lineParticles.MinStartSize;
            lineParticles.EndVelocity = 0;
            lineParticles.MinColor = new Color(0, 0, 0, 255);
            lineParticles.MaxColor = new Color(255, 255, 255, 255);
            lineEffectNode.ParticleEffects.Add(lineParticles);

            lineParticles = new FireParticleEffect();
            lineParticles.TextureName = name;
            lineParticles.MaxHorizontalVelocity = lineParticles.MinHorizontalVelocity;
            lineParticles.MaxVerticalVelocity = lineParticles.MinVerticalVelocity;
            lineParticles.MinStartSize = 2f;
            lineParticles.MaxStartSize = 2f;
            lineParticles.MaxEndSize = lineParticles.MaxStartSize;
            lineParticles.MinEndSize = lineParticles.MinStartSize;
            lineParticles.EndVelocity = 0;
            lineParticles.MinColor = new Color(0, 0, 0, 255);
            lineParticles.MaxColor = new Color(255, 255, 255, 255);
            lineEffectNode.ParticleEffects.Add(lineParticles);

            lineEffectNode.UpdateHandler += new ParticleUpdateHandler(UpdateLine);
            lineEffectNode.Enabled = false;

            return lineEffectNode;

        }
        public void update(Vector3 source, Vector3 target)
        {
            this.source = source;
            this.target = target;
            if (!lineEffectNode.Enabled)
                lineEffectNode.Enabled = true;
        }
        public void disable()
        {
            lineEffectNode.Enabled = false;
        }
        public void enable()
        {
            lineEffectNode.Enabled = true;
        }
        private void UpdateLine(Matrix worldTransform, List<ParticleEffect> particleEffects)
        {
            if (!lineEffectNode.Enabled)
                lineEffectNode.Enabled = true;
            if (Vector3.Zero.Equals(target))
                return;
            Vector3 vel = target - source;
            foreach (ParticleEffect particle in particleEffects)
            {

                // Add 10 fire particles every frame
                for (int k = 0; k < 20; k++)
                {
                    if (!Vector3.Zero.Equals(worldTransform.Translation))
                        particle.AddParticle(source, vel);
                }
            }
        }

    }

    class FireGenerator
    {
        Vector3 source, target;
        ParticleNode fireNode;

        public ParticleNode addParticle()
        {
            ParticleEffect fireParticle = new FireParticleEffect();
            SmokePlumeParticleEffect smoke = new SmokePlumeParticleEffect();

            fireNode = new ParticleNode();
            fireNode.ParticleEffects.Add(fireParticle);

            fireParticle = new FireParticleEffect();
            fireNode.ParticleEffects.Add(fireParticle);

            fireParticle = new FireParticleEffect();
            fireNode.ParticleEffects.Add(fireParticle);

            fireNode.ParticleEffects.Add(smoke);

            fireNode.UpdateHandler += new ParticleUpdateHandler(UpdateLine);
            fireNode.Enabled = false;

            return fireNode;

        }
        public void update(Vector3 source, Vector3 target)
        {
            this.source = source;
            this.target = target;
        }
        public void disable()
        {
            fireNode.Enabled = false;
        }
        public void enable()
        {
            fireNode.Enabled = true;
        }
        public void setSource(Vector3 source)
        {
            this.source = source;
        }
        private void UpdateLine(Matrix worldTransform, List<ParticleEffect> particleEffects)
        {
            if (Vector3.Zero.Equals(target))
                return;
            Vector3 vel = target - source;
            foreach (ParticleEffect particle in particleEffects)
            {

                if (particle is SmokePlumeParticleEffect)
                    particle.AddParticle(source + vel, Vector3.Zero);
                else
                {
                    for (int k = 0; k < 20; k++)
                    {
                        if (!Vector3.Zero.Equals(worldTransform.Translation))
                            particle.AddParticle(source, vel);
                    }
                }
            }
        }

    }

    class LaserLineGenerator
    {
        Vector3 source, target;
        ParticleNode laserNode;

        public ParticleNode addParticle()
        {
            ParticleEffect laserParticle = new FireParticleEffect();
            laserParticle.TextureName = "particleLine2";
            laserParticle.MaxHorizontalVelocity = laserParticle.MinHorizontalVelocity;
            laserParticle.MaxVerticalVelocity = laserParticle.MinVerticalVelocity;
            laserParticle.MinStartSize = 8f;
            laserParticle.MaxStartSize = 8f;
            laserParticle.MaxEndSize = laserParticle.MaxStartSize;
            laserParticle.MinEndSize = laserParticle.MinStartSize;
            laserParticle.EndVelocity = 0;
            laserParticle.MinColor = new Color(0, 0, 0, 127);
            laserParticle.MaxColor = new Color(255, 255, 255, 255);
            laserNode = new ParticleNode();
            laserNode.ParticleEffects.Add(laserParticle);

            laserParticle = new FireParticleEffect();
            laserParticle.TextureName = "particleLine2";
            laserParticle.MaxHorizontalVelocity = laserParticle.MinHorizontalVelocity;
            laserParticle.MaxVerticalVelocity = laserParticle.MinVerticalVelocity;
            laserParticle.MinStartSize = 8f;
            laserParticle.MaxStartSize = 8f;
            laserParticle.MaxEndSize = laserParticle.MaxStartSize;
            laserParticle.MinEndSize = laserParticle.MinStartSize;
            laserParticle.EndVelocity = 0;
            laserParticle.MinColor = new Color(0, 0, 0, 127);
            laserParticle.MaxColor = new Color(255, 255, 255, 255);
            laserNode.ParticleEffects.Add(laserParticle);

            

            laserNode.UpdateHandler += new ParticleUpdateHandler(UpdateLine);
            laserNode.Enabled = false;

            return laserNode;

        }
        public void update(Vector3 source, Vector3 target)
        {
            this.source = source;
            this.target = target;
        }
        public void disable()
        {
            laserNode.Enabled = false;
        }
        public void enable()
        {
            laserNode.Enabled = true;
        }
        public void setSource(Vector3 source)
        {
            this.source = source;
        }
        private void UpdateLine(Matrix worldTransform, List<ParticleEffect> particleEffects)
        {
            if (!laserNode.Enabled)
                laserNode.Enabled = true;
            if (Vector3.Zero.Equals(target))
                return;
            Vector3 vel = target - source;
            foreach (ParticleEffect particle in particleEffects)
            {

                // Add 10 fire particles every frame
                for (int k = 0; k < 20; k++)
                {
                    if (!Vector3.Zero.Equals(worldTransform.Translation))
                        particle.AddParticle(source, vel);
                }
            }
        }

    }
}
