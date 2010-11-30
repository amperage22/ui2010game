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
        private const int SIZEOFTARGETLABEL = 15; //pixels tall

        private G2DPanel identityPanel, readoutPanel;
        private G2DLabel[] identity = new G2DLabel[Player.MAX_NUM_DIE];
        private G2DLabel[] output = new G2DLabel[Player.MAX_NUM_DIE];
        private G2DLabel[] target = new G2DLabel[Player.MAX_NUM_DIE];
        private Player player;

        public ReadoutScreen(Player player)
        {
            this.player = player;
            initialize();
        }

        private void initialize()
        {
            identityPanel = new G2DPanel();
            readoutPanel = new G2DPanel();
            if (player.PlayerNum == 1)
            {
                identityPanel.Bounds = new Rectangle(0, 493, 400, 32);
                readoutPanel.Bounds = new Rectangle(0, 525, 400, 75);
                readoutPanel.Texture = State.Content.Load<Texture2D>("Textures/readout/p1_bar");
            }
            else
            {
                identityPanel.Bounds = new Rectangle(400, 493, 400, 32);
                readoutPanel.Bounds = new Rectangle(400, 525, 400, 75);
                readoutPanel.Texture = State.Content.Load<Texture2D>("Textures/readout/p2_bar");
            }

            identityPanel.DrawBackground = false;

            SpriteFont font = State.Content.Load<SpriteFont>("CooperBlack");
            for (int i = 0; i < Player.MAX_NUM_DIE; i++)
            {
                identity[i] = new G2DLabel();
                identity[i].BackgroundColor = new Color(Die.getDiscColor(player.Die[i].DieNum));
                identity[i].DrawBackground = true;
                identity[i].BorderColor = Color.Black;
                identity[i].DrawBorder = true;

                output[i] = new G2DLabel();
                output[i].VerticalAlignment = GoblinEnums.VerticalAlignment.Top;
                output[i].TextFont = font;
                output[i].TextColor = Color.White;

                target[i] = new G2DLabel();
                target[i].TextFont = font;
                
                if (player.PlayerNum == 1)
                {
                    identity[i].Bounds = new Rectangle(5 + (identityPanel.Bounds.Width / 3) * i, 0, identityPanel.Bounds.Height, identityPanel.Bounds.Height);

                    Rectangle r = new Rectangle(5 + (readoutPanel.Bounds.Width / 3) * i, 0, readoutPanel.Bounds.Width / 3, readoutPanel.Bounds.Height - SIZEOFTARGETLABEL - 5);
                    output[i].Bounds = r;
                    
                    r.Y = r.Height;
                    r.Height = SIZEOFTARGETLABEL;
                    target[i].Bounds = r;

                    target[i].TextColor = new Color(0, 100, 255);
                }
                else
                {
                    identity[i].Bounds = new Rectangle(10 + (identityPanel.Bounds.Width / 3) * i, 0, identityPanel.Bounds.Height, identityPanel.Bounds.Height);

                    Rectangle r = new Rectangle(10 + (readoutPanel.Bounds.Width / 3) * i, 0, readoutPanel.Bounds.Width / 3, readoutPanel.Bounds.Height - SIZEOFTARGETLABEL - 5);
                    output[i].Bounds = r;

                    r.Y = r.Height;
                    r.Height = SIZEOFTARGETLABEL;
                    target[i].Bounds = r;

                    target[i].TextColor = new Color(247, 0, 0);
                }

                identityPanel.AddChild(identity[i]);
                readoutPanel.AddChild(output[i]);
                readoutPanel.AddChild(target[i]);
            }

            identityPanel.Enabled = false;
            identityPanel.Visible = false;
            readoutPanel.Enabled = false;
            readoutPanel.Visible = false;

            GlobalScene.scene.UIRenderer.Add2DComponent(identityPanel);
            GlobalScene.scene.UIRenderer.Add2DComponent(readoutPanel);
            
            update();
        }

        public void update()
        {
            Die[] dice = player.Die;
            for (int d = 0; d < dice.Length; d++)
                if (dice[d].CurrentMonster != null) {
                    output[d].Text = dice[d].CurrentMonster.ToString();
                    Monster nearestEnemy = dice[d].CurrentMonster.NearestEnemy;
                    if (nearestEnemy != null)
                    {
                        target[d].Text = nearestEnemy.Name;
                        target[d].TextColor = new Color(Die.getDiscColor(nearestEnemy.DieNum));
                    } 
                    else
                        target[d].Text = "";
                } else {
                    output[d].Text = "No Monster";
                    target[d].Text = "";
                }
        }

        public bool isDisplaying()
        {
            return readoutPanel.Visible;
        }

        public void toggleDisplay()
        {
            identityPanel.Visible = !identityPanel.Visible;
            identityPanel.Enabled = !identityPanel.Enabled;
            readoutPanel.Visible = !readoutPanel.Visible;
            readoutPanel.Enabled = !readoutPanel.Enabled;
        }
    }
}
