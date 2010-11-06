using System;
using System.Collections.Generic;
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
using Model = GoblinXNA.Graphics.Model;
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
    struct Buff
    {
        public ModifierType modifier;
        public CreatureType affectedCreature;
        public int mod;
        public Buff(ModifierType m, CreatureType c, int mod)
        {
            modifier = m;
            affectedCreature = c;
            this.mod = mod;
        }
        public ModifierType Modifier
        {
            get { return modifier; }
            set { modifier = value; }
        }
        public CreatureType AffectedCreature
        {
            get { return affectedCreature; }
            set { affectedCreature = value; }
        }
        public int Amount
        {
            get { return mod; }
            set { mod = value; }
        }

    }
}