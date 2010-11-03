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
        private const int firstCardMarker = 265;
        MarkerNode marker;

        private int healthMod, dmgMod, dmgDone, dmgPrevent;
        private CardType type;

        public Card(Scene s,CardType type, int markerNum, int dmg, int health)
        {
            this.type = type;

            //Set up the 6 sides of this die
            marker = new MarkerNode();
            int[] side_marker = new int[1];
            side_marker[0] = markerNum;
            String config_file = String.Format("Content/dice_markers/card{0}.txt", markerNum - firstCardMarker);
            marker = new MarkerNode(s.MarkerTracker, config_file, side_marker);
            s.RootNode.AddChild(marker);

            switch (type)
            {
                case CardType.STAT_MOD: dmgMod = dmg; dmgDone = 0; dmgPrevent = 0; healthMod = health; break;
                case CardType.DMG_DONE: dmgMod = 0; dmgDone = dmg; dmgPrevent = 0; healthMod = 0; break;
                case CardType.DMG_PREVENT: dmgMod = 0; dmgDone = 0; dmgPrevent = health; healthMod = 0; break;
            }
        }
        public Card(Scene s,CardType type, int markerNum, int mod)
        {
            this.type = type;

            //Set up the 6 sides of this die
            marker = new MarkerNode();
            int[] side_marker = new int[1];
            side_marker[0] = markerNum;
            String config_file = String.Format("Content/dice_markers/card{0}.txt", markerNum + firstCardMarker);
            marker = new MarkerNode(s.MarkerTracker, config_file, side_marker);
            s.RootNode.AddChild(marker);

            switch (type)
            {
                case CardType.DMG_DONE: dmgMod = 0; dmgDone = mod; dmgPrevent = 0; healthMod = 0; break;
                case CardType.DMG_PREVENT: dmgMod = 0; dmgDone = 0; dmgPrevent = mod; healthMod = 0; break;
            }
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
    }
    class CardBuilder
    {
        Scene s;
        CardType type;
        int markerNum;
        int dmg, health, mod;

        public CardBuilder(Scene s,CardType type, int markerNum, int dmg, int health)
        {
            this.s = s;
            this.type = type;
            this.markerNum = markerNum;
            this.dmg = dmg;
            this.health = health;
        }

        public CardBuilder(Scene s,CardType type, int markerNum, int mod)
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
                case CardType.STAT_MOD: return new Card(s, type, markerNum, dmg, health);
                case CardType.DMG_DONE:
                case CardType.DMG_PREVENT: return new Card(s, type, markerNum, mod);
            }
            return null;

        }

    }
}
