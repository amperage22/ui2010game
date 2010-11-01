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
        private int specialization, activeTab, pointsRemaining;
        private G2DLabel pointCount, tooltip;
        private G2DButton submit, clear;
        private Texture2D[, ,] buttonTextures = new Texture2D[3, 3, 3];
        private Texture2D[] tabTextures = new Texture2D[3];
        private Color disabledColor = new Color(80, 80, 80);

        Scene scene;
        ContentManager content;
        SpriteFont font;
        TalentState state;

        private const int INITIAL_TALENT_POINTS = 10;
        private const int MULTIPLE_TREE_THRESHOLD = INITIAL_TALENT_POINTS * 2 / 3;

        /*
         * Makes the talent screen as per specifications.
         * s The scene to display the talent screen on
         * f The font to be used with within the talent screen being created
         * activeTab The initial tree to display and tab to select
         */
        public TalentScreen(Scene scene, ContentManager content, int specialization)
        {
            this.scene = scene;
            this.content = content;
            activeTab = (this.specialization = specialization);
            pointsRemaining = INITIAL_TALENT_POINTS;
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
            talentFrame.AddChild(tooltip);

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
            backgroundFrame.Texture = content.Load<Texture2D>("Textures/talents/talentscreen_bg");

            // Create the main panel which holds all other GUI components
            mainFrame = new G2DPanel();
            mainFrame.Bounds = new Rectangle(10, 10, 300, 400);
            mainFrame.Border = GoblinEnums.BorderFactory.EmptyBorder;
            mainFrame.Transparency = 0.85f;  // Ranges from 0 (fully transparent) to 1 (fully opaque)

            //Set up the 3 tabs
            for (int i = 0; i < 3; i++)
            {
                tab[i] = new G2DButton();
                tab[i].TextFont = font;
                tab[i].Bounds = new Rectangle(100 * i, 0, 100, 48);
                tab[i].Texture = tabTextures[i];
                tab[i].TextureColor = disabledColor;
                tab[i].ActionPerformedEvent += new ActionPerformed(HandleTabButtonPress);
                tab[i].DrawBorder = false;
                mainFrame.AddChild(tab[i]);
            }

            pointCount = new G2DLabel(String.Format("Points Remaining: {0}", pointsRemaining));
            pointCount.Bounds = new Rectangle(8, 370, 110, 20);
            pointCount.TextFont = font;
            pointCount.TextColor = Color.White;
            pointCount.DrawBorder = true;
            pointCount.BorderColor = Color.White;
            mainFrame.AddChild(pointCount);


            Texture2D stone_button = content.Load<Texture2D>("Textures/stone_button");
            submit = new G2DButton("Save");
            submit.TextFont = font;
            submit.Bounds = new Rectangle(140, 367, 70, 25);
            submit.Texture = stone_button;
            submit.TextureColor = disabledColor;
            submit.TextColor = Color.White;
            submit.BorderColor = Color.White;
            submit.ActionPerformedEvent += new ActionPerformed(HandleSubmit);
            mainFrame.AddChild(submit);

            clear = new G2DButton("Clear");
            clear.TextFont = font;
            clear.Bounds = new Rectangle(220, 367, 70, 25);
            clear.Texture = stone_button;
            clear.TextureColor = Color.White;
            clear.TextColor = Color.Black;
            clear.BorderColor = Color.White;
            clear.ActionPerformedEvent += new ActionPerformed(HandleClear);
            mainFrame.AddChild(clear);

            ChangeToTree(activeTab);

            backgroundFrame.AddChild(mainFrame);
        }

        //Handles the talentscreen tab logic when a tab is clicked
        private void HandleTabButtonPress(object source)
        {
            if (INITIAL_TALENT_POINTS - pointsRemaining < MULTIPLE_TREE_THRESHOLD)
                return;
            //Set the new active tabs visual state
            for (int i = 0; i < 3; i++)
            {
                //Verify if we need to compute anything
                if (source != tab[i]) continue;
                if (i == activeTab) return;

                //Keep the tab/screen states consistent for the user
                tab[activeTab].TextureColor = Color.White;
                tab[i].TextureColor = disabledColor;
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
            ChangeToTree(specialization);
            closeSecondaryTabs();
            talents[specialization].reset();
            for (int i = 0; i < 3; i++)
            {
                bool isDisabled = !talents[activeTab].canAllocTier(i);
                for (int j = 0; j < 3; j++)
                {
                    if (i == 2 && j != 1) continue;
                    pointsAlloc[i, j].Text = talents[activeTab].getPointStr(i, j);
                    pointsAlloc[i, j].TextColor = Color.White;
                    talentButton[i, j].TextureColor = isDisabled ? disabledColor : Color.White;
                }
            }
            pointsRemaining = INITIAL_TALENT_POINTS;
            pointCount.Text = String.Format("Points Remaining: {0}", pointsRemaining);
            submit.TextureColor = disabledColor;
            submit.TextColor = Color.White;
        }

        public bool wasSubmitted()
        {
            return state == TalentState.FINISHED;
        }

        public List<Talent> getTalentInfo()
        {
            if (state != TalentState.FINISHED)
                throw new Exception("The selection must be submitted before you can get the info!");
            List<Talent> toReturn = new List<Talent>();
            for (int i = 0; i < 3; i++)
                toReturn.AddRange(talents[i].getTalents());
            return toReturn;
        }

        private void ChangeToTree(int tree)
        {
            if (tree < 0 || tree > 2)
                throw new Exception("Tree must be 0, 1, or 2");

            activeTab = tree;
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
                        if (i == 2 && j != 1) continue;

                        talentButton[i, j] = new G2DButton();
                        talentButton[i, j].Bounds = new Rectangle((j * 102) + 16, (i * 102) + 16, 64, 64);
                        talentButton[i, j].TextFont = font;
                        talentButton[i, j].Texture = buttonTextures[activeTab, i, j];
                        talentButton[i, j].MouseReleasedEvent += new MouseReleased(HandleAlloc);
                        talentButton[i, j].BorderColor = Color.Black;
                        talentButton[i, j].DrawBorder = false;
                        if (i > 0)
                            talentButton[i, j].TextureColor = disabledColor;
                        talentFrame.AddChild(talentButton[i, j]);

                        //Put a black rectangle above the button but below the label
                        G2DPanel p = new G2DPanel();
                        p.Bounds = new Rectangle((j * 102) + 16, (i * 102) + 16, 10, 5);
                        p.BackgroundColor = Color.Black;
                        p.Transparency = 1.0f;
                        talentFrame.AddChild(p);

                        pointsAlloc[i, j] = new G2DLabel(talents[activeTab].getPointStr(i, j));
                        pointsAlloc[i, j].TextFont = font;
                        Vector2 new_dimensions = pointsAlloc[i, j].TextFont.MeasureString(pointsAlloc[i, j].Text);
                        pointsAlloc[i, j].Bounds = new Rectangle((j * 102) + 17, (i * 102) + 17, (int)new_dimensions.X + 5, (int)new_dimensions.Y + 5);
                        pointsAlloc[i, j].BackgroundColor = new Color(100, 100, 100);
                        pointsAlloc[i, j].BorderColor = Color.White;
                        pointsAlloc[i, j].TextColor = Color.White;
                        pointsAlloc[i, j].DrawBackground = true;
                        pointsAlloc[i, j].DrawBorder = true;
                        talentFrame.AddChild(pointsAlloc[i, j]);
                    }

                mainFrame.AddChild(talentFrame);
            }

            else //Swap the new talent tree in

                for (int i = 0; i < 3; i++)
                {
                    bool isDisabled = !talents[activeTab].canAllocTier(i);
                    for (int j = 0; j < 3; j++)
                    {
                        if (i == 2 && j != 1) continue;
                        talentButton[i, j].Texture = buttonTextures[activeTab, i, j];
                        bool isMaxed = talents[activeTab].isMaxed(i, j);
                        talentButton[i, j].TextureColor = isDisabled || isMaxed ? disabledColor : Color.White;
                        pointsAlloc[i, j].TextColor = isMaxed ? Color.Red : Color.White;
                        pointsAlloc[i, j].Text = talents[activeTab].getPointStr(i, j);
                        Vector2 new_dimensions = pointsAlloc[i, j].TextFont.MeasureString(pointsAlloc[i, j].Text);
                        pointsAlloc[i, j].Bounds = new Rectangle(pointsAlloc[i, j].Bounds.X, pointsAlloc[i, j].Bounds.Y, (int)new_dimensions.X + 5, (int)new_dimensions.Y + 5);
                    }
                }

        }

        private void createTalentTrees()
        {
            /* Beasts Tree */
            talents[(int)Creature.BEASTS] = new TalentTree(CreatureType.BEASTS);
            talents[(int)Creature.DRAGONKIN] = new TalentTree(CreatureType.DRAGONKIN);
            talents[(int)Creature.ROBOTS] = new TalentTree(CreatureType.ROBOTS);
        }

        private void HandleAlloc(int button, Point mouse)
        {
            //Find the button that got clicked
            bool tipFound = false;
            //TODO: Make this large yucky if-chain more compact
            for (int i = 0; i < 3 && !tipFound; i++)
                for (int j = 0; j < 3; j++)
                {
                    if (i == 2 && j != 1) continue;
                    if (talentButton[i, j].PaintBounds.Contains(mouse))
                    {
                        bool beforeAlloc = talents[activeTab].canAllocTier(i + 1);
                        if (button == MouseInput.LeftButton)
                        {
                            if (pointsRemaining == 0) return;
                            if (talents[activeTab].increment(i, j))
                            {
                                if (beforeAlloc != talents[activeTab].canAllocTier(i + 1))
                                    openTier(i + 1);
                                pointsRemaining--;
                                if (pointsRemaining == 0)
                                {
                                    submit.TextureColor = Color.White;
                                    submit.TextColor = Color.Black;
                                }
                                pointCount.Text = String.Format("Points Remaining: {0}", pointsRemaining);
                                pointsAlloc[i, j].Text = talents[activeTab].getPointStr(i, j);
                                if (talents[activeTab].isMaxed(i, j))
                                {
                                    talentButton[i, j].TextureColor = disabledColor;
                                    pointsAlloc[i, j].TextColor = Color.Red;
                                }
                                int num = INITIAL_TALENT_POINTS - pointsRemaining;
                                if (num >= MULTIPLE_TREE_THRESHOLD && num - 1 < MULTIPLE_TREE_THRESHOLD)
                                    openSecondaryTabs();
                            }
                        }
                        else if (button == MouseInput.RightButton)
                        {
                            if (talents[activeTab].decrement(i, j))
                            {
                                /* There arises the case where the player will achieve the
                                 * 2/3 points necessary to open the other two trees.  He will
                                 * then spend points in them and go attempt to remove the
                                 * points he spent in his main specialization tree.  Here
                                 * we check for this behavior and clear the two remaining
                                 * trees and give him back those points if he goes below
                                 * the limit. */
                                if (activeTab == specialization)
                                {
                                    int pointsInSpecialization = talents[activeTab].getPointsAllocd();
                                    if (pointsInSpecialization < MULTIPLE_TREE_THRESHOLD && pointsInSpecialization + 1 >= MULTIPLE_TREE_THRESHOLD)
                                        closeSecondaryTabs();
                                }

                                if (beforeAlloc != talents[activeTab].canAllocTier(i + 1))
                                    closeTier(i + 1);
                                pointsRemaining++;
                                submit.TextureColor = disabledColor;
                                submit.TextColor = Color.White;
                                pointCount.Text = String.Format("Points Remaining: {0}", pointsRemaining);
                                pointsAlloc[i, j].Text = talents[activeTab].getPointStr(i, j);
                                talentButton[i, j].TextureColor = Color.White;
                                pointsAlloc[i, j].TextColor = Color.White;
                            }
                        }
                    }
                }
        }

        private String getTabHelpString()
        {
            String tree;
            switch (specialization)
            {
                case 0: tree = "BEAST"; break;
                case 1: tree = "DRAGONKIN"; break;
                case 2: tree = "ROBOT"; break;
                default: throw new Exception("Bad specialization value!");
            }
            int pointsNeeded = MULTIPLE_TREE_THRESHOLD - (INITIAL_TALENT_POINTS - pointsRemaining);
            return String.Format("You must first place {0} more point{1} in the\n{2} tree before you can access this one!", pointsNeeded, pointsNeeded == 1 ? "" : "s", tree);
        }

        private void HandleToolTip(Point mouse)
        {
            //Find the button that got hovered over, if found...
            bool tipFound = false;
            if (submit.PaintBounds.Contains(mouse) && pointsRemaining != 0)
            {
                tooltip.Text = "You must spend ALL your talent points first!";
                tipFound = true;
            }
            for (int i = 0; i < 3 && !tipFound; i++)
            {
                if (tab[i].PaintBounds.Contains(mouse))
                {
                    if (i == activeTab || talents[specialization].getPointsAllocd() >= MULTIPLE_TREE_THRESHOLD) break;
                    tooltip.Text = getTabHelpString();
                    tipFound = true;
                    break;
                }
                for (int j = 0; j < 3 && !tipFound; j++)
                {
                    if (i == 2 && j != 1) continue;
                    if (talentButton[i, j].PaintBounds.Contains(mouse))
                    {
                        tooltip.Text = talents[activeTab].getDescription(i, j);
                        tipFound = true;
                        break;
                    }
                }
            }

            if (tipFound)
            {
                Vector2 textSize = tooltip.TextFont.MeasureString(tooltip.Text);
                int new_width = (int)(textSize.X + 0.5f) + 5;
                int new_height = (int)(textSize.Y + 0.5f) + 5;

                tooltip.Bounds = new Rectangle(mouse.X - backgroundFrame.Bounds.X - (new_width / 2) + 10, mouse.Y - backgroundFrame.Bounds.Y - 40, new_width, new_height);
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
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    for (int k = 0; k < 3; k++)
                    {
                        if (j == 2 && k != 1) continue;
                        buttonTextures[i, j, k] = content.Load<Texture2D>(
                            String.Format(
                                "Textures/talents/{0}_{1}{2}",
                                i == (int)Creature.BEASTS ? "beast" : i == (int)Creature.DRAGONKIN ? "dragonkin" : "robot",
                                j + 1,
                                j == 2 ? "" : String.Format("_{0}", k + 1)));
                    }
                tabTextures[i] = content.Load<Texture2D>(
                    i == 0 ? "Textures/talents/beasts_tab" :    //http://www.geekcoefficient.com/blog/images/beast.jpg
                    i == 1 ? "Textures/talents/dragonkin_tab" : //http://www.crystalinks.com/dragon.gif
                             "Textures/talents/robots_tab");    //http://www.techgadgets.in/images/nikko-robot.jpg
            }
        }

        private void openTier(int t)
        {
            talentButton[t, 1].TextureColor = Color.White;
            if (t == 2) return;
            talentButton[t, 0].TextureColor = Color.White;
            talentButton[t, 2].TextureColor = Color.White;
        }
        private void closeTier(int t)
        {
            //Dont forget to give the player back their points
            //that could have been in the last tier
            pointsRemaining += talents[activeTab].getPointsInTier(t);
            talents[activeTab].resetTier(t);
            talentButton[t, 1].TextureColor = disabledColor;
            pointsAlloc[t, 1].Text = talents[activeTab].getPointStr(t, 1);
            pointsAlloc[t, 1].TextColor = Color.White;
            if (t == 2) return;
            talentButton[t, 0].TextureColor = disabledColor;
            pointsAlloc[t, 0].Text = talents[activeTab].getPointStr(t, 0);
            pointsAlloc[t, 0].TextColor = Color.White;
            talentButton[t, 2].TextureColor = disabledColor;
            pointsAlloc[t, 2].Text = talents[activeTab].getPointStr(t, 2);
            pointsAlloc[t, 2].TextColor = Color.White;
            //Make sure to close all tiers above it too!
            closeTier(t + 1);
        }
        private void openSecondaryTabs()
        {
            for (int k = 0; k < 3; k++)
                if (k != specialization)
                    tab[k].TextureColor = Color.White;
        }
        private void closeSecondaryTabs()
        {
            for (int i = 0; i < 3; i++)
                if (i != specialization)
                {
                    pointsRemaining += talents[i].getPointsAllocd();
                    talents[i].reset();
                    tab[i].TextureColor = disabledColor;
                }
        }
    }
}