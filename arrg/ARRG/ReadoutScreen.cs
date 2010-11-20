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
    class ReadoutScreen
    {
        private G2DPanel mainPanel;
        private G2DLabel[] output = new G2DLabel[Player.MAX_NUM_DIE];
        private Player player;

        public ReadoutScreen(Player player)
        {
            this.player = player;
            initialize();
        }

        private void initialize()
        {
            mainPanel = new G2DPanel();
            if (player.PlayerNum == 1)
            {
                mainPanel.Bounds = new Rectangle(0, 525, 400, 75);
                mainPanel.Texture = State.Content.Load<Texture2D>("Textures/readout/p1_bar");
            }
            else
            {
                mainPanel.Bounds = new Rectangle(400, 525, 400, 75);
                mainPanel.Texture = State.Content.Load<Texture2D>("Textures/readout/p2_bar");
            }
            mainPanel.Enabled = false;
            mainPanel.Visible = false;

            SpriteFont font = State.Content.Load<SpriteFont>("UIFont_Bold_Small");
            for (int i = 0; i < Player.MAX_NUM_DIE; i++)
            {
                output[i] = new G2DLabel();
                output[i].VerticalAlignment = GoblinEnums.VerticalAlignment.Bottom;
                output[i].TextFont = font;
                output[i].TextColor = Color.White;
                if (player.PlayerNum == 1)
                {
                    output[i].Bounds = new Rectangle(5 + (mainPanel.Bounds.Width / 3) * i, 0, mainPanel.Bounds.Width / 3, mainPanel.Bounds.Height - 5);
                }
                else
                {
                    output[i].Bounds = new Rectangle(10 + (mainPanel.Bounds.Width / 3) * i, 0, mainPanel.Bounds.Width / 3, mainPanel.Bounds.Height - 5);
                }
                mainPanel.AddChild(output[i]);
            }

            GlobalScene.scene.UIRenderer.Add2DComponent(mainPanel);

            update();
        }

        public void update()
        {
            Die[] dice = player.Die;
            for (int d = 0; d < dice.Length; d++)
                if (dice[d].CurrentMonster != null)
                    output[d].Text = dice[d].CurrentMonster.ToString();
                else
                    output[d].Text = "No Monster";
        }

        public bool isDisplaying()
        {
            return mainPanel.Visible;
        }

        public void toggleDisplay()
        {
            mainPanel.Visible = !mainPanel.Visible;
            mainPanel.Enabled = !mainPanel.Enabled;
        }
    }
}
