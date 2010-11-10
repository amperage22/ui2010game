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
    class Dialog
    {
        enum DialogState { READY, DISPLAYING, FINISHED };

        private G2DPanel frame;
        private G2DButton button;
        private G2DLabel message;

        private Scene scene;
        private ContentManager content;
        private SpriteFont font;
        private DialogState state;

        public Dialog(Scene scene, ContentManager content)
        {
            this.scene = scene;
            this.content = content;
            font = content.Load<SpriteFont>("UIFont_Bold");

            CreateFrame();

            state = DialogState.READY;
        }

        public void Display(String message)
        {
            if (state != DialogState.DISPLAYING)
            {
                configure(message);
                scene.UIRenderer.Add2DComponent(frame);
                frame.Enabled = true;
                state = DialogState.DISPLAYING;
            }
        }

        private void CreateFrame()
        {
            //Create the main panel which holds all other GUI components
            frame = new G2DPanel();
            frame.Border = GoblinEnums.BorderFactory.LineBorder;
            frame.BackgroundColor = Color.LightGray;
            frame.DrawBorder = true;

            message = new G2DLabel();
            message.TextFont = font;
            message.HorizontalAlignment = GoblinEnums.HorizontalAlignment.Center;
            frame.AddChild(message);

            button = new G2DButton("Okay!");
            button.TextFont = font;
            button.BorderColor = Color.Black;
            button.TextColor = Color.Black;
            button.DrawBorder = true;
            button.HighlightColor = Color.Black;
            button.HorizontalAlignment = GoblinEnums.HorizontalAlignment.Center;
            button.VerticalAlignment = GoblinEnums.VerticalAlignment.Center;
            button.ActionPerformedEvent += new ActionPerformed(HandleButton);
            frame.AddChild(button);

            frame.Enabled = false;
        }

        private void HandleButton(object source)
        {
            scene.UIRenderer.Remove2DComponent(frame);
            frame.Enabled = false;
            state = DialogState.FINISHED;
        }

        private void configure(String m)
        {
            Vector2 string_size = message.TextFont.MeasureString(m);
            message.Text = m;
            message.Bounds = new Rectangle(10, 10, (int)string_size.X, (int)string_size.Y);

            Vector2 frame_size = new Vector2(string_size.X + 20, string_size.Y + 60);
            frame.Bounds = new Rectangle((int)(800 - frame_size.X) / 2, (int)(600 - frame_size.Y) / 2, (int)frame_size.X, (int)frame_size.Y);

            button.Bounds = new Rectangle((int)(frame_size.X - 70) / 2, (int)frame_size.Y - 37, 70, 25);
        }

        public bool isDisplaying()
        {
            return state == DialogState.DISPLAYING;
        }
    }
}