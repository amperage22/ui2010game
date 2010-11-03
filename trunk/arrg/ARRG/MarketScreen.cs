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
    class MarketScreen
    {
        enum MarketState { READY, DISPLAYING, FINISHED };

        private const int ROWS = 3, COLS = 5, SELECTED = 0, LOCKED = 1;

        private G2DPanel mainFrame, itemFrame;
        private G2DButton[,] itemButton = new G2DButton[ROWS, COLS];
        private Texture2D[,] buttonTextures = new Texture2D[ROWS, COLS];
        private Texture2D lockedTexture;
        private bool[,,] itemButtonFlag = new bool[ROWS, COLS, 2];
        private G2DLabel tooltip, goldLeft;
        private G2DButton submit, clear;
        private Color disabledColor = new Color(80, 80, 80);

        private Scene scene;
        private ContentManager content;
        private SpriteFont font;
        private MarketState state;

        private Player player;
        private MonsterBuilder[] monsters = new MonsterBuilder[15];
        private int amountSpent;

        /*
         * Makes the talent screen as per specifications.
         * s The scene to display the talent screen on
         * f The font to be used with within the talent screen being created
         */
        public MarketScreen(Scene scene, ContentManager content, Player player, List<MonsterBuilder> monsters)
        {
            List<MonsterBuilder> mb = new List<MonsterBuilder>(monsters);
            mb.Add(new MonsterBuilder(CreatureID.UNKNOWN1, CreatureType.DRAGONKIN, "WTF", "alexmodel", content.Load<Texture2D>("Textures/inventory/unknown"), 0, 0, false, 1));
            mb.Add(new MonsterBuilder(CreatureID.UNKNOWN2, CreatureType.DRAGONKIN, "WTF", "alexmodel", content.Load<Texture2D>("Textures/inventory/unknown"), 0, 0, false, 1));
            mb.Add(new MonsterBuilder(CreatureID.JONATHAN, CreatureType.DRAGONKIN, "WTF", "alexmodel", content.Load<Texture2D>("Textures/inventory/d_jonathan"), 0, 0, false, 1));
            mb.Add(new MonsterBuilder(CreatureID.MEYNARD, CreatureType.DRAGONKIN, "WTF", "alexmodel", content.Load<Texture2D>("Textures/inventory/d_meynard"), 0, 0, false, 1));
            if (mb.Count != 15)
                throw new Exception("I can't handle this case");
            foreach (MonsterBuilder m in mb)
                this.monsters[m.getID()] = m;
            this.scene = scene;
            this.content = content;
            this.player = player;
            amountSpent = 0;
            font = content.Load<SpriteFont>("UIFont");

            allocateTextures();

            CreateFrame();

            Personalize();

            state = MarketState.READY;
        }

        public void Display()
        {
            if (state != MarketState.DISPLAYING)
            {
                scene.UIRenderer.Add2DComponent(mainFrame);
                state = MarketState.DISPLAYING;
            }
        }

        /**
         * Responsible for initializing all of the market screen stuff with
         * consideration to the player it corresponds to */
        private void Personalize()
        {
            if (player.PurchasedMonsters == null) return;
            foreach (MonsterBuilder m in player.PurchasedMonsters)
            {
                int i = m.getID() / COLS, j = m.getID() - i * COLS;
                itemButton[i, j].Texture = lockedTexture;
                itemButtonFlag[i, j, LOCKED] = true;
            }
        }

        private void CreateFrame()
        {
            // Create the main panel which holds all other GUI components
            mainFrame = new G2DPanel();
            mainFrame.Bounds = new Rectangle(180, 148, 440, 304);
            mainFrame.Border = GoblinEnums.BorderFactory.LineBorder;
            mainFrame.Transparency = 1.0f;  // Ranges from 0 (fully transparent) to 1 (fully opaque)
            mainFrame.Texture = content.Load<Texture2D>("Textures/market/market_bg");
            mainFrame.DrawBorder = true;

            //Submit and clear buttons
            Texture2D market_button = content.Load<Texture2D>("Textures/market/market_button");
            submit = new G2DButton("Confirm");
            submit.TextFont = font;
            submit.Bounds = new Rectangle(280, 266, 70, 25);
            submit.Texture = market_button;
            submit.TextureColor = Color.White;
            submit.DrawBorder = false;
            submit.TextColor = Color.White;
            submit.BorderColor = Color.White;
            submit.HighlightColor = Color.Black;
            submit.ActionPerformedEvent += new ActionPerformed(HandleSubmit);
            mainFrame.AddChild(submit);

            clear = new G2DButton("Clear");
            clear.TextFont = font;
            clear.Bounds = new Rectangle(360, 266, 70, 25);
            clear.Texture = market_button;
            clear.TextureColor = Color.White;
            clear.TextColor = Color.White;
            clear.DrawBorder = false;
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
                    itemButton[i, j].Texture = buttonTextures[i, j];
                    itemButton[i, j].MouseReleasedEvent += new MouseReleased(HandleAlloc);
                    itemButton[i, j].BorderColor = Color.Black;
                    itemButton[i, j].DrawBorder = true;
                    itemFrame.AddChild(itemButton[i, j]);
                }

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
        
        private void HandleSubmit(object source)
        {
            scene.UIRenderer.Remove2DComponent(mainFrame);
            state = MarketState.FINISHED;
        }

        private void HandleClear(object source)
        {
            for (int i = 0; i < ROWS; i++)
                for (int j = 0; j < COLS; j++)
                {
                    //itemButton[i, j].TextureColor = isDisabled ? disabledColor : Color.White;
                }
        }

        public bool wasSubmitted()
        {
            return state == MarketState.FINISHED;
        }

        /**
         * Call this after the player has submitted the market purchase
         */
        public void commit()
        {
            if (state != MarketState.FINISHED)
                throw new Exception("The selection must be submitted before you can commit it!");
            
            //Set the proper values in the Player object
            List<MonsterBuilder> newMonsterList = new List<MonsterBuilder>();
            for (int i = 0; i < ROWS; i++)
                for (int j = 0; j < COLS; j++)
                    if (itemButtonFlag[i, j, SELECTED] || itemButtonFlag[i, j, LOCKED])
                        newMonsterList.Add(monsters[i * COLS + j]);
            player.PurchasedMonsters = newMonsterList;

            player.Gold -= amountSpent;
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
                            itemButton[i, j].TextureColor = Color.White;
                            itemButton[i, j].BorderColor = Color.Black;
                        }
                        else //the user wants to select this item
                        {
                            itemButtonFlag[i, j, SELECTED] = true;
                            itemButton[i, j].TextureColor = disabledColor;
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
                        }
                    }
                }
        }

        private void HandleToolTip(Point mouse)
        {
            //Find the button that got hovered over, if found...
            bool tipFound = false;
            if (submit.PaintBounds.Contains(mouse))
            {
                tooltip.Text = "Purchase all your selected monsters.";
                tipFound = true;
            }
            for (int i = 0; i < ROWS && !tipFound; i++)
                for (int j = 0; j < COLS && !tipFound; j++)
                {
                    if (itemButton[i, j].PaintBounds.Contains(mouse))
                    {
                        if (itemButtonFlag[i, j, LOCKED])
                        {
                            tooltip.Text = String.Format("You already have this monster\nin your inventory.", i, j);
                        }
                        else if (itemButtonFlag[i, j, SELECTED])
                        {
                            tooltip.Text = String.Format("Click now to NOT buy {0}.", monsters[i * COLS + j].getName());
                        }
                        else
                        {
                            tooltip.Text = String.Format("Buy {0}!\nPrice: {1} Gold", monsters[i * COLS + j].getName(), monsters[i * COLS + j].getCost());
                        }
                        tipFound = true;
                        break;
                    }
                }

            if (tipFound)
            {
                Vector2 textSize = tooltip.TextFont.MeasureString(tooltip.Text);
                int new_width = (int)(textSize.X + 0.5f) + 5;
                int new_height = (int)(textSize.Y + 0.5f) + 5;

                tooltip.Bounds = new Rectangle(mouse.X - mainFrame.Bounds.X - (new_width / 2) - 10, mouse.Y - mainFrame.Bounds.Y - 40, new_width, new_height);
                tooltip.DrawBackground = true;
                tooltip.DrawBorder = true;
            }
            else  //hide the tooltip from the user
            {
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