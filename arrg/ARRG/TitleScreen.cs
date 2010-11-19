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
using GoblinXNA.UI;
using GoblinXNA.UI.UI2D;
using GoblinXNA.SceneGraph;
using GoblinXNA.Graphics;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;
using GoblinXNA.Device.Generic;

namespace ARRG_Game
{
    class TitleScreen
    {
        CreatureType specialization;
        private Texture2D titleScreen;
        private Vector2 point; //Point to draw titleScreen at (top left)
        private G2DPanel frame;
        private G2DButton[] creature = new G2DButton[3];

        public TitleScreen()
        {
            titleScreen = State.Content.Load<Texture2D>("Textures/title/title_screen");
            point = new Vector2(0, 0);
            specialization = CreatureType.NONE;
            createButtons();
            
        }

        private void createButtons()
        {
            frame = new G2DPanel();
            frame.DrawBackground = false;
            frame.DrawBorder = false;
            frame.Bounds = new Rectangle(8, 300, 784, 256);

            for (int i = 0; i < 3; i++)
            {
                creature[i] = new G2DButton();
                creature[i].Bounds = new Rectangle(i * 264, 0, 256, 256);
                creature[i].BorderColor = Color.Black;
                creature[i].HighlightColor = Color.White;
                creature[i].DrawBorder = true;
                creature[i].DrawBackground = false;
                creature[i].ActionPerformedEvent += new ActionPerformed(HandleSpecialization);
                switch (i)
                {
                    case 0: creature[i].Texture = State.Content.Load<Texture2D>("Textures/title/beasts"); break;
                    case 1: creature[i].Texture = State.Content.Load<Texture2D>("Textures/title/dragonkin"); break;
                    case 2: creature[i].Texture = State.Content.Load<Texture2D>("Textures/title/robots"); break;
                }
                creature[i].TextureColor = Color.White;
                frame.AddChild(creature[i]);
            }

            GlobalScene.scene.UIRenderer.Add2DComponent(frame);
        }

        public void Draw(SpriteBatch sb)
        {
            sb.Begin(SpriteBlendMode.AlphaBlend);
            sb.Draw(titleScreen, point, Color.White);
            sb.End();
        }

        private void HandleSpecialization(object source)
        {
            if (source == creature[0])
                specialization = CreatureType.BEASTS;
            else if (source == creature[1])
                specialization = CreatureType.DRAGONKIN;
            else if (source == creature[2])
                specialization = CreatureType.ROBOTS;
        }

        public CreatureType playerChoice()
        {
            return specialization;
        }

        public void Kill()
        {
            GlobalScene.scene.UIRenderer.Remove2DComponent(frame);
        }
    }
}
