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
    class Dragonkin : Monster
    {
        public Dragonkin(string name, String model, int health, int power, bool useInternal)
            : base(name, model, health, power, useInternal)
        {
            hit = 80;
            dodge = 20;
            crit = 40;
            type = CreatureType.DRAGONKIN;
        }
        public override void startAttackAnimation()
        {
            transNode.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, MathHelper.ToRadians(10));
        }
        public override void endAttackAnimation()
        {
            //Should end simple "animation" of Monsters atack
        }
        //********End Dice-Monster Interactions********************
    }
}