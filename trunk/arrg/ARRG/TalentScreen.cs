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

        private G2DPanel backgroundFrame, mainFrame, talentFrame;
        private G2DButton[] tab = new G2DButton[3];
        private G2DButton[,] talent = new G2DButton[3, 3];
        private int activeTab;
        private G2DButton submit = new G2DButton();

        Scene scene;
        ContentManager content;
        SpriteFont font;
        TalentState state;

        private const String tree1 = "Beasts";
        private const String tree2 = "Dragonkin";
        private const String tree3 = "Robots";

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
            font = content.Load<SpriteFont>("UIFont");
            
            CreateFrame();
            state = TalentState.READY;

            /*Set up any specific stuff with the talent screen here,
             * put stuff in the frame,
             * etc...
            blah... blah.. blah.
            */
        }

        public void Display()
        {
            if (state != TalentState.DISPLAYING)
            {
                scene.UIRenderer.Add2DComponent(backgroundFrame);
                state = TalentState.DISPLAYING;
            }
        }

        public void Kill()
        {
            if (state == TalentState.DISPLAYING || state == TalentState.FINISHED) {
                scene.UIRenderer.Remove2DComponent(backgroundFrame);
                state = TalentState.READY;
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

            submit = new G2DButton("submit");
            submit.TextFont = font;
            submit.Bounds = new Rectangle(100 * 4, 0, 100, 48);
            submit.BackgroundColor = (Color.LightGray);
            submit.TextColor = (Color.Black);
            submit.ActionPerformedEvent += new ActionPerformed(HandleSubmit);
            mainFrame.AddChild(submit);

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
            state = TalentState.FINISHED;
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
                talentFrame = new G2DPanel();
                talentFrame.Bounds = new Rectangle(0, 60, 300, 300);
                talentFrame.Border = GoblinEnums.BorderFactory.EtchedBorder;
                talentFrame.Transparency = 1.0f;  // Ranges from 0 (fully transparent) to 1 (fully opaque)
                
                for (int i = 0; i < 3; i++) //rows of tree
                    for (int j = 0; j < 3; j++) //cols of tree
                    {
                        talent[i, j] = new G2DButton(String.Format("{0}, {1}", i, j));
                        talent[i, j].Bounds = new Rectangle((j * 102) + 16, (i * 102) + 16, 64, 64);
                        talent[i, j].TextFont = font;
                        talentFrame.AddChild(talent[i, j]);
                    }

                mainFrame.AddChild(talentFrame);
            }

            //Swap the new talent tree in
        }
    }
}