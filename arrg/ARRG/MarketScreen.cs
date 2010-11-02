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
        private bool[,,] itemButtonFlag = new bool[ROWS, COLS, 2];
        private G2DLabel tooltip;
        private G2DButton submit, clear;
        private Color disabledColor = new Color(80, 80, 80);

        private Scene scene;
        private ContentManager content;
        private SpriteFont font;
        private MarketState state;

        private Player player;
        private int amountSpent;

        /*
         * Makes the talent screen as per specifications.
         * s The scene to display the talent screen on
         * f The font to be used with within the talent screen being created
         */
        public MarketScreen(Scene scene, ContentManager content, Player player)
        {
            this.scene = scene;
            this.content = content;
            this.player = player;
            amountSpent = 0;
            font = content.Load<SpriteFont>("UIFont");

            allocateTextures();

            CreateFrame();

            tooltip = new G2DLabel();
            tooltip.TextFont = font;
            tooltip.BackgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.1f);
            tooltip.TextColor = Color.White;
            tooltip.HorizontalAlignment = GoblinEnums.HorizontalAlignment.Center;
            tooltip.VerticalAlignment = GoblinEnums.VerticalAlignment.Center;
            tooltip.DrawBorder = false;
            tooltip.DrawBackground = false;
            itemFrame.AddChild(tooltip);

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

        private void CreateFrame()
        {
            // Create the main panel which holds all other GUI components
            mainFrame = new G2DPanel();
            mainFrame.Bounds = new Rectangle(180, 148, 440, 304);
            mainFrame.Border = GoblinEnums.BorderFactory.LineBorder;
            mainFrame.Transparency = 1.0f;  // Ranges from 0 (fully transparent) to 1 (fully opaque)
            mainFrame.Texture = content.Load<Texture2D>("Textures/inventory/market_bg");
            mainFrame.DrawBorder = true;

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
            mainFrame.AddChild(itemFrame);

            //Submit and clear buttons
            Texture2D market_button = content.Load<Texture2D>("Textures/market_button");
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
                        }
                        else //the user wants to select this item
                        {
                            itemButtonFlag[i, j, SELECTED] = true;
                            itemButton[i, j].TextureColor = disabledColor;
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
                        tooltip.Text = String.Format("{0}, {1}", i, j);
                        tipFound = true;
                        break;
                    }
                }

            if (tipFound)
            {
                Vector2 textSize = tooltip.TextFont.MeasureString(tooltip.Text);
                int new_width = (int)(textSize.X + 0.5f) + 5;
                int new_height = (int)(textSize.Y + 0.5f) + 5;

                tooltip.Bounds = new Rectangle(mouse.X - mainFrame.Bounds.X - (new_width / 2) + 10, mouse.Y - mainFrame.Bounds.Y - 40, new_width, new_height);
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
                    buttonTextures[i, j] = content.Load<Texture2D>(getTextureName(i, j));
        }

        //whoo hoo...huge switch for loading textures with different names...
        private String getTextureName(int row, int col) {
            switch (row)
            {
                case 0: switch (col)
                    {
                        case 0: return "Textures/inventory/bear"; //http://www.telegraph.co.uk/news/picturegalleries/picturesoftheday/4804523/Pictures-of-the-day-25-February-2009.html?image=2
                        case 1: return "Textures/inventory/dalek"; //http://thewertzone.blogspot.com/2010/04/specieswatch-daleks.html
                        case 2: return "Textures/inventory/dragon1"; //http://www.goodreads.com/topic/show/401347-mutantpack-characters-crash-s-gang
                        case 3: return "Textures/inventory/dragon2"; //http://www.google.com/imgres?imgurl=http://images.fanpop.com/images/image_uploads/Spyro--Year-of-the-Dragon-WP-spyro-the-dragon-321436_1024_768.jpg&imgrefurl=http://www.fanpop.com/spots/spyro-the-dragon/images/321436&usg=__OgRjWhM1IVMlwKCkbafrdJS3p38=&h=768&w=1024&sz=2305&hl=en&start=34&zoom=1&tbnid=8TlO_avf2pbPjM:&tbnh=119&tbnw=168&prev=/images%3Fq%3Ddragon%26um%3D1%26hl%3Den%26sa%3DN%26biw%3D1280%26bih%3D909%26tbs%3Disch:10,600&um=1&itbs=1&iact=hc&vpx=663&vpy=605&dur=615&hovh=194&hovw=259&tx=162&ty=117&ei=FKPPTK7MN8ydnwezromOBg&oei=2KLPTNvvHoSclgez3dX6BQ&esq=2&page=2&ndsp=35&ved=1t:429,r:17,s:34&biw=1280&bih=909
                        case 4: return "Textures/inventory/gundam"; //http://loyalkng.com/2009/03/12/real-life-size-gundam-mobile-suit-gundam-30th-anniversary-running-2-months/
                    } break;
                case 1: switch (col)
                    {
                        case 0: return "Textures/inventory/penguin"; //http://ssjpenguin.yolasite.com/about-me.php
                        case 1: return "Textures/inventory/rhino"; //http://www.techbanyan.com/11579/female-rhino-south-africa-killed-poachers/
                        case 2: return "Textures/inventory/samus"; //http://ocremix.org/forums/member.php?u=12644
                        case 3: return "Textures/inventory/tank"; //http://www.core77.com/blog/object_culture/tanks_but_no_tanks_9108.asp
                        case 4: return "Textures/inventory/tiger"; //http://www.dailymail.co.uk/news/article-479533/A-spine-tingling-encounter-Indian-tigers-facing-extinction.html
                    } break;
                case 2: switch (col)
                    {
                        case 0: return "Textures/inventory/unknown";
                        case 1: return "Textures/inventory/unknown";
                        case 2: return "Textures/inventory/d_jonathan";
                        case 3: return "Textures/inventory/d_meynard";
                        case 4: return "Textures/inventory/d_alex";
                    } break;
            }
            return "Wow, you really screwed something up...";
        }
    }
}