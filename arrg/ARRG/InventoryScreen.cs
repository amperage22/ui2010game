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
    class InventoryScreen
    {
        enum InventoryState { READY, DISPLAYING, FINISHED };

        private const int ROWS = 3, COLS = 5, SELECTED = 0, LOCKED = 1;

        private G2DPanel mainFrame, itemFrame;
        private G2DButton[,] itemButton = new G2DButton[ROWS, COLS];
        private Texture2D[,] buttonTextures = new Texture2D[ROWS, COLS];
        private Texture2D lockedTexture;
        private bool[,,] itemButtonFlag = new bool[ROWS, COLS, 2];
        private G2DLabel tooltip;
        private G2DButton submit, clear;
        private Color disabledColor = new Color(80, 80, 80);
        private G2DLabel monstersSelectedText;
        private int numSelectedMonsters;

        private Scene scene;
        private ContentManager content;
        private SpriteFont font;
        private InventoryState state;

        private Player player;
        private MonsterBuilder[] monsters = new MonsterBuilder[15];

        /*
         * Makes the talent screen as per specifications.
         * s The scene to display the talent screen on
         * f The font to be used with within the talent screen being created
         */
        public InventoryScreen(Scene scene, ContentManager content, Player player, List<MonsterBuilder> monsters)
        {
            if (monsters.Count != 15)
                throw new Exception("I can't handle this case");
            foreach (MonsterBuilder m in monsters)
                this.monsters[m.getID()] = m;
            this.scene = scene;
            this.content = content;
            this.player = player;
            numSelectedMonsters = 0;
            font = content.Load<SpriteFont>("UIFont_Bold");

            allocateTextures();

            CreateFrame();

            Personalize();

            state = InventoryState.READY;
        }

        public void Display()
        {
            if (state != InventoryState.DISPLAYING)
            {
                scene.UIRenderer.Add2DComponent(mainFrame);
                state = InventoryState.DISPLAYING;
            }
        }

        /**
         * Responsible for initializing all of the market screen stuff with
         * consideration to the player it corresponds to */
        private void Personalize()
        {
            //Unlock purchased monsters
            foreach (MonsterBuilder m in player.PurchasedMonsters)
            {
                int i = m.getID() / COLS, j = m.getID() - i * COLS;
                itemButton[i, j].Texture = buttonTextures[i, j];
                itemButton[i, j].TextureColor = disabledColor;
                itemButtonFlag[i, j, LOCKED] = false;
            }
            HandleClear(null);
        }

        private void CreateFrame()
        {
            //Create the main panel which holds all other GUI components
            mainFrame = new G2DPanel();
            mainFrame.Bounds = new Rectangle(180, 148, 440, 304);
            mainFrame.Border = GoblinEnums.BorderFactory.LineBorder;
            mainFrame.Transparency = 1.0f;  // Ranges from 0 (fully transparent) to 1 (fully opaque)
            mainFrame.Texture = content.Load<Texture2D>("Textures/inventory/inventory_bg");
            mainFrame.DrawBorder = true;

            //Submit and clear buttons
            Texture2D market_button = content.Load<Texture2D>("Textures/inventory/inventory_button");
            submit = new G2DButton("Win");
            submit.TextFont = font;
            submit.Bounds = new Rectangle(280, 266, 70, 25);
            //submit.Texture = market_button;
            //submit.TextureColor = new Color(200, 200, 200);
            submit.BackgroundColor = Color.White;
            submit.DrawBackground = true;
            submit.BorderColor = Color.Black;
            submit.DrawBorder = true;
            submit.TextColor = Color.Black;
            submit.HighlightColor = Color.Black;
            submit.ActionPerformedEvent += new ActionPerformed(HandleSubmit);
            mainFrame.AddChild(submit);

            clear = new G2DButton("Reset");
            clear.TextFont = font;
            clear.Bounds = new Rectangle(360, 266, 70, 25);
            //clear.Texture = market_button;
            //clear.TextureColor = new Color(200, 200, 200);
            clear.BackgroundColor = Color.White;
            clear.DrawBackground = true;
            clear.BorderColor = Color.Black;
            clear.TextColor = Color.Black;
            clear.DrawBorder = true;
            clear.HighlightColor = Color.Black;
            clear.ActionPerformedEvent += new ActionPerformed(HandleClear);
            mainFrame.AddChild(clear);

            //Create all the slots for the items and stuff
            itemFrame = new G2DPanel();
            itemFrame.Bounds = new Rectangle(10, 10, 420, 244);
            itemFrame.Border = GoblinEnums.BorderFactory.LineBorder;
            itemFrame.Transparency = 1.0f;
            for (int i = 0; i < ROWS; i++)
                for (int j = 0; j < COLS; j++)
                {
                    itemButton[i, j] = new G2DButton();
                    itemButton[i, j].Bounds = new Rectangle(10 + (j * 88), 10 + (i * 88), 48, 48);
                    itemButton[i, j].Texture = lockedTexture;
                    itemButton[i, j].MouseReleasedEvent += new MouseReleased(HandleAlloc);
                    itemButton[i, j].BorderColor = Color.Black;
                    itemButton[i, j].DrawBorder = true;
                    switch (monsters[i * COLS + j].getType())
                    {
                        case CreatureType.BEASTS:
                            itemButton[i, j].HighlightColor = Color.Blue; break;
                        case CreatureType.DRAGONKIN:
                            itemButton[i, j].HighlightColor = Color.Red; break;
                        case CreatureType.ROBOTS:
                            itemButton[i, j].HighlightColor = Color.Green; break;
                        default:
                            itemButton[i, j].HighlightColor = Color.White; break;
                    }
                    itemFrame.AddChild(itemButton[i, j]);
                    itemButtonFlag[i, j, LOCKED] = true;
                }

            monstersSelectedText = new G2DLabel();
            monstersSelectedText.BackgroundColor = Color.Black;
            monstersSelectedText.DrawBackground = true;
            monstersSelectedText.TextFont = font;
            monstersSelectedText.TextColor = Color.White;
            monstersSelectedText.DrawBorder = true;
            monstersSelectedText.BorderColor = Color.White;
            monstersSelectedText.VerticalAlignment = GoblinEnums.VerticalAlignment.Center;
            updateSelectedMonstersText();
            mainFrame.AddChild(monstersSelectedText);

            //And the tooltip
            tooltip = new G2DLabel();
            tooltip.TextFont = font;
            tooltip.BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.1f);
            tooltip.TextColor = Color.White;
            tooltip.HorizontalAlignment = GoblinEnums.HorizontalAlignment.Center;
            tooltip.VerticalAlignment = GoblinEnums.VerticalAlignment.Center;
            tooltip.DrawBorder = false;
            tooltip.DrawBackground = false;
            itemFrame.AddChild(tooltip);
            mainFrame.AddChild(itemFrame);
        }

        private void updateSelectedMonstersText()
        {
            monstersSelectedText.Text = String.Format("Monsters: {0} / {1}", numSelectedMonsters, Player.MAX_CREATURES_INGAME);
            Vector2 v = monstersSelectedText.TextFont.MeasureString(monstersSelectedText.Text);
            monstersSelectedText.Bounds = new Rectangle(10, 266, (int)v.X + 10, 25);
            //I may add more code to this function for milestone 3
        }

        private void HandleSubmit(object source)
        {
            if (numSelectedMonsters == 0) return;
            scene.UIRenderer.Remove2DComponent(mainFrame);
            state = InventoryState.FINISHED;
        }

        private void HandleClear(object source)
        {
            for (int i = 0; i < ROWS; i++)
                for (int j = 0; j < COLS; j++)
                {
                    itemButtonFlag[i, j, SELECTED] = false;
                    itemButton[i, j].TextureColor = disabledColor;
                    itemButton[i, j].BorderColor = Color.Black;
                }

            //Select selected monsters...duuuurrr
            foreach (MonsterBuilder m in player.SelectedMonsters)
            {
                int i = m.getID() / COLS, j = m.getID() - i * COLS;
                itemButton[i, j].TextureColor = Color.White;
                itemButtonFlag[i, j, SELECTED] = true;
            }
            //For some reason
            numSelectedMonsters = player.SelectedMonsters.Count;
            updateSelectedMonstersText();
        }

        public bool wasSubmitted()
        {
            return state == InventoryState.FINISHED;
        }

        /**
         * Call this after the player has submitted the market purchase
         */
        public void commit()
        {
            if (state != InventoryState.FINISHED)
                throw new Exception("The selection must be submitted before you can commit it!");

            //Set the proper values in the Player object
            List<MonsterBuilder> newMonsterList = new List<MonsterBuilder>();
            for (int i = 0; i < ROWS; i++)
                for (int j = 0; j < COLS; j++)
                    if (itemButtonFlag[i, j, SELECTED])
                        newMonsterList.Add(monsters[i * COLS + j]);
            player.SelectedMonsters = newMonsterList;
        }

        private void HandleAlloc(int button, Point mouse)
        {
            for (int i = 0; i < ROWS; i++)
                for (int j = 0; j < COLS; j++)
                {
                    if (itemButton[i, j].PaintBounds.Contains(mouse))
                    {
                        if (itemButtonFlag[i, j, LOCKED]) return;
                        /* We don't care if the player left or right clicks
                         * since we aren't counting up or down, only on or
                         * off states */
                        if (itemButtonFlag[i, j, SELECTED])
                        {
                            itemButtonFlag[i, j, SELECTED] = false;
                            itemButton[i, j].TextureColor = disabledColor;
                            itemButton[i, j].BorderColor = Color.Black;
                            numSelectedMonsters--;
                        }
                        else //the user wants to select this item
                        {
                            if (numSelectedMonsters == Player.MAX_CREATURES_INGAME) return;
                            itemButtonFlag[i, j, SELECTED] = true;
                            itemButton[i, j].TextureColor = Color.White;
                            switch (monsters[i * COLS + j].getType())
                            {
                                case CreatureType.BEASTS:
                                    itemButton[i, j].BorderColor = Color.Blue; break;
                                case CreatureType.DRAGONKIN:
                                    itemButton[i, j].BorderColor = Color.Red; break;
                                case CreatureType.ROBOTS:
                                    itemButton[i, j].BorderColor = Color.Green; break;
                                default:
                                    itemButton[i, j].BorderColor = Color.White; break;
                            }
                            numSelectedMonsters++;
                        }
                        updateSelectedMonstersText();
                    }
                }
        }

        private void HandleToolTip(Point mouse)
        {
            //Find the button that got hovered over, if found...
            bool tipFound = false;
            if (submit.PaintBounds.Contains(mouse))
            {
                if (numSelectedMonsters == 0)
                    tooltip.Text = "You must take at least ONE\nmonster to battle with ya...";
                else if (numSelectedMonsters == 1)
                    tooltip.Text = "I can pwn with one monster!";
                else
                    tooltip.Text = "This'll work.";
                tipFound = true;
            }
            for (int i = 0; i < ROWS && !tipFound; i++)
                for (int j = 0; j < COLS && !tipFound; j++)
                {
                    if (itemButton[i, j].PaintBounds.Contains(mouse))
                    {
                        if (itemButtonFlag[i, j, LOCKED])
                        {
                            tooltip.Text = String.Format("You do not own this monster!", i, j);
                        }
                        else if (itemButtonFlag[i, j, SELECTED])
                        {
                            tooltip.Text = String.Format("Click to de-select {0}.", monsters[i * COLS + j].getName());
                        }
                        else
                        {
                            tooltip.Text = String.Format("Take {0} to battle with you!\nSummon Cost: {1} Mana", monsters[i * COLS + j].getName(), monsters[i * COLS + j].getManaCost());
                        }
                        tipFound = true;
                    }
                }

            if (tipFound)
            {
                Vector2 textSize = tooltip.TextFont.MeasureString(tooltip.Text);
                int new_width = (int)(textSize.X + 0.5f) + 5;
                int new_height = (int)(textSize.Y + 0.5f) + 5;

                tooltip.Bounds = new Rectangle(mouse.X - mainFrame.Bounds.X - (new_width / 2) - 10, mouse.Y - mainFrame.Bounds.Y - 80, new_width, new_height);
                tooltip.DrawBackground = true;
                tooltip.DrawBorder = true;
            }
            else  //hide the tooltip from the user
            {
                //In retrospect I should've done stuff like this with the
                //Visible property..
                tooltip.DrawBorder = false;
                tooltip.DrawBackground = false;
                tooltip.Text = "";
            }
        }

        public void update(Point mouse)
        {
            HandleToolTip(mouse);
        }

        private void allocateTextures()
        {
            for (int i = 0; i < ROWS; i++)
                for (int j = 0; j < COLS; j++)
                    buttonTextures[i, j] = monsters[i * COLS + j].getInvTexture();
            lockedTexture = content.Load<Texture2D>("Textures/market/locked");
        }


    }
}