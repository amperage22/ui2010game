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

namespace ARRG_Game
{
    /// <summary>
    /// This tutorial introduce GoblinXNA's 2D GUI facilities.
    /// </summary>
    class TalentScreen
    {
        enum TalentState { READY, DISPLAYING, FINISHED };
        private enum Creature { BEASTS = 0, DRAGONKIN = 1, ROBOTS = 2 };

        private G2DPanel backgroundFrame, mainFrame, talentFrame;
        private G2DButton[] tab = new G2DButton[3];
        private G2DButton[,] talentButton = new G2DButton[3, 3];
        //The label to show how many points the player has used on a certain talent
        private G2DLabel[,] pointsAlloc = new G2DLabel[3, 3];
        private TalentTree[] talents = new TalentTree[3];
        private int activeTab, pointsRemaining;
        private G2DLabel pointCount, tooltip;
        private G2DButton submit, clear;

        Scene scene;
        ContentManager content;
        SpriteFont font;
        TalentState state;

        private const String tree1 = "Beasts";
        private const String tree2 = "Dragonkin";
        private const String tree3 = "Robots";
        private const int INITIAL_TALENT_POINTS = 15;

        /*
         * Makes the talent screen as per specifications.
         * s The scene to display the talent screen on
         * f The font to be used with within the talent screen being created
         * activeTab The initial tree to display and tab to select
         */
        public TalentScreen(Scene scene, ContentManager content, int activeTab)
        {
            this.scene = scene;
            this.content = content;
            this.activeTab = activeTab;
            pointsRemaining = INITIAL_TALENT_POINTS;
            font = content.Load<SpriteFont>("UIFont");

            CreateFrame();
            
            tooltip = new G2DLabel();
            tooltip.TextFont = font;
            tooltip.BackgroundColor = Color.Black;
            tooltip.TextColor = Color.White;
            tooltip.DrawBackground = true;
            tooltip.Enabled = false;

            state = TalentState.READY;
        }

        public void Display()
        {
            if (state != TalentState.DISPLAYING)
            {
                scene.UIRenderer.Add2DComponent(backgroundFrame);
                state = TalentState.DISPLAYING;
            }
        }

        private void CreateFrame()
        {
            backgroundFrame = new G2DPanel();
            backgroundFrame.Bounds = new Rectangle(240, 90, 320, 420);
            backgroundFrame.Border = GoblinEnums.BorderFactory.LineBorder;
            backgroundFrame.Transparency = 1.0f;
            backgroundFrame.BackgroundColor = Color.Black;
            backgroundFrame.Texture = content.Load<Texture2D>("Textures/talentscreen_bg");

            // Create the main panel which holds all other GUI components
            mainFrame = new G2DPanel();
            mainFrame.Bounds = new Rectangle(10, 10, 300, 400);
            mainFrame.Border = GoblinEnums.BorderFactory.EmptyBorder;
            mainFrame.Transparency = 0.85f;  // Ranges from 0 (fully transparent) to 1 (fully opaque)
            
            //Set up the 3 tabs
            for (int i = 0; i < 3; i++)
            {
                tab[i] = new G2DButton(i == 0 ? tree1 : i == 1 ? tree2 : tree3);
                tab[i].TextFont = font;
                tab[i].Bounds = new Rectangle(100 * i, 0, 100, 48);
                tab[i].BackgroundColor = (activeTab == i ? new Color(80, 80, 80) : Color.LightGray);
                tab[i].TextColor = (activeTab == i ? Color.White : Color.Black);
                tab[i].ActionPerformedEvent += new ActionPerformed(HandleTabButtonPress);
                mainFrame.AddChild(tab[i]);
            }

            pointCount = new G2DLabel(String.Format("Points Remaining: {0}", pointsRemaining));
            pointCount.Bounds = new Rectangle(8, 370, 110, 20);
            pointCount.TextFont = font;
            pointCount.TextColor = Color.White;
            pointCount.DrawBorder = true;
            pointCount.BorderColor = Color.White;
            mainFrame.AddChild(pointCount);

            submit = new G2DButton("Submit");
            submit.TextFont = font;
            submit.Bounds = new Rectangle(140, 367, 70, 25);
            submit.BackgroundColor = (Color.LightGray);
            submit.TextColor = (Color.Black);
            submit.ActionPerformedEvent += new ActionPerformed(HandleSubmit);
            mainFrame.AddChild(submit);

            clear = new G2DButton("Clear");
            clear.TextFont = font;
            clear.Bounds = new Rectangle(220, 367, 70, 25);
            clear.BackgroundColor = (Color.LightGray);
            clear.TextColor = (Color.Black);
            clear.ActionPerformedEvent += new ActionPerformed(HandleClear);
            mainFrame.AddChild(clear);

            ChangeToTree(activeTab);

            backgroundFrame.AddChild(mainFrame);
        }

        //Handles the talentscreen tab logic when a tab is clicked
        private void HandleTabButtonPress(object source)
        {
            //Set the new active tabs visual state
            for (int i = 0; i < 3; i++)
            {
                //Verify if we need to compute anything
                if (source != tab[i]) continue;
                if (i == activeTab) return;

                //Keep the tab/screen states consistent for the user
                tab[activeTab].BackgroundColor = Color.LightGray;
                tab[activeTab].TextColor = Color.Black;
                tab[i].BackgroundColor = new Color(80, 80, 80);
                tab[i].TextColor = Color.White;
                activeTab = i;
                ChangeToTree(i);

                //Get outta here
                return;
            }
        }

        private void HandleSubmit(object source)
        {
            if (pointsRemaining != 0) return;
            scene.UIRenderer.Remove2DComponent(backgroundFrame);
            state = TalentState.FINISHED;
        }

        private void HandleClear(object source)
        {
            for (int i = 0; i < 3; i++)
                talents[i].reset();
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if ((i == 1 && j == 1) || (i == 2 && j != 1)) continue;
                    pointsAlloc[i, j].Text = talents[activeTab].getPointStr(i, j);
                }
            pointsRemaining = INITIAL_TALENT_POINTS;
            pointCount.Text = String.Format("Points Remaining: {0}", pointsRemaining);
        }

        public bool wasSubmitted() {
            return state == TalentState.FINISHED;
        }

        public void getTalentInfo() {
            if (state != TalentState.FINISHED)
                throw new Exception("The selection must be submitted before you can get the info!");
            //TODO: return TALENT INFO
        }

        private void ChangeToTree(int tree)
        {
            if (tree < 0 || tree > 2)
                throw new Exception("Tree must be 0, 1, or 2");

            if (talentFrame == null)
            {
                createTalentTrees();
                talentFrame = new G2DPanel();
                talentFrame.Bounds = new Rectangle(0, 60, 300, 300);
                talentFrame.Border = GoblinEnums.BorderFactory.EtchedBorder;
                talentFrame.Transparency = 1.0f;
                
                for (int i = 0; i < 3; i++) //rows of tree
                    for (int j = 0; j < 3; j++) //cols of tree
                    {
                        //We are going to have a 3, 2, 1 tree, so only certain things
                        //should be created.
                        if ((i == 1 && j == 1) || (i == 2 && j != 1)) continue;

                        talentButton[i, j] = new G2DButton(String.Format("{0}, {1}", i, j));
                        talentButton[i, j].Bounds = new Rectangle((j * 102) + 16, (i * 102) + 16, 64, 64);
                        talentButton[i, j].TextFont = font;
                        talentButton[i, j].ActionPerformedEvent += new ActionPerformed(HandleAlloc);
                        talentButton[i, j].MouseMovedEvent += new MouseMoved(HandleToolTip);
                        talentButton[i, j].MouseExitedEvent += new MouseExited(clearToolTip);
                        talentFrame.AddChild(talentButton[i, j]);
                        
                        //Put a black rectangle above the button but below the label
                        G2DPanel p = new G2DPanel();
                        p.Bounds = new Rectangle((j * 102) + 16, (i * 102) + 16, 10, 5);
                        p.BackgroundColor = Color.Black;
                        p.Transparency = 1.0f;
                        talentFrame.AddChild(p);
                        
                        pointsAlloc[i, j] = new G2DLabel(talents[activeTab].getPointStr(i, j));
                        pointsAlloc[i, j].TextFont = font;
                        pointsAlloc[i, j].Bounds = new Rectangle((j * 102) + 17, (i * 102) + 17, 10, 5);
                        talentFrame.AddChild(pointsAlloc[i, j]);
                    }

                mainFrame.AddChild(talentFrame);
            }

            //Swap the new talent tree in
            for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++) {
                        if ((i == 1 && j == 1) || (i == 2 && j != 1)) continue;
                        pointsAlloc[i, j].Text = talents[activeTab].getPointStr(i, j);
                    }
            
        }

        private void createTalentTrees() {
            /* Beasts Tree */
            talents[(int)Creature.BEASTS] = new TalentTree(CreatureType.BEASTS);
            talents[(int)Creature.DRAGONKIN] = new TalentTree(CreatureType.DRAGONKIN);
            talents[(int)Creature.ROBOTS] = new TalentTree(CreatureType.ROBOT);
        }

        private void HandleAlloc(object source)
        {
            if (pointsRemaining == 0) return;
            //Find out which button was clicked
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (talentButton[i, j] == source)
                        if (talents[activeTab].increment(i, j)) {
                            pointsRemaining--;
                            pointCount.Text = String.Format("Points Remaining: {0}", pointsRemaining);
                            pointsAlloc[i, j].Text = talents[activeTab].getPointStr(i, j);
                        }
        }

        private void HandleToolTip(Point mouse)
        {
            //Find the button that got hovered over...
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    if ((i == 1 && j == 1) || (i == 2 && j != 1)) continue;
                    Rectangle r = getGlobalRect(talentButton[i, j]);
                    if (talentButton[i, j].Bounds.Contains(mouse))
                    {
                        tooltip.Enabled = true;
                        tooltip.Text = talents[activeTab].getDescription(i, j);
                        tooltip.Bounds = new Rectangle(mouse.X, mouse.Y, 100, 50);
                    }
                }
        }

        private Rectangle getGlobalRect(G2DComponent c)
        {
            if (c == null)
                throw new Exception("Null component passed");
            Rectangle r = c.Bounds;
            c = (G2DComponent)c.Parent;
            while (c != null)
            {
                r.Offset(c.Bounds.X, c.Bounds.Y);
                c = (G2DComponent)c.Parent;
            }
            return r;
        }

        private void clearToolTip()
        {
            tooltip.Enabled = false;
        }
    }
}