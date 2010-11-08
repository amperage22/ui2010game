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
    class ParticleLineGenerator
    {
        Vector3 source = Vector3.Zero
            , target = Vector3.Zero;

        public ParticleNode addParticle()
        {
            FireParticleEffect fireParticles = new FireParticleEffect();
            //fireParticles.TextureName = "particles";
            ParticleNode fireRingEffectNode = new ParticleNode();
            fireRingEffectNode.ParticleEffects.Add(fireParticles);
            fireRingEffectNode.ParticleEffects.Add(new FireParticleEffect());
            //fireRingEffectNode.ParticleEffects.Add(new FireParticleEffect());
            fireRingEffectNode.UpdateHandler += new ParticleUpdateHandler(UpdateLine);

            return fireRingEffectNode;

        }
        public void update(Vector3 source, Vector3 target)
        {
            this.source = source;
            this.target = target;
        }
        private void UpdateLine(Matrix worldTransform, List<ParticleEffect> particleEffects)
        {
            Vector3 vel = source - target;
            foreach (ParticleEffect particle in particleEffects)
            {
                if (particle is FireParticleEffect)
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
}
