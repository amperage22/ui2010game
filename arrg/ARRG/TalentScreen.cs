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
    public class TalentScreen
    {
        private G2DPanel backgroundFrame, mainFrame, talentFrame;
        private G2DButton[] tab = new G2DButton[3];
        private int activeTab;

        private const String tree1 = "Type I";
        private const String tree2 = "Type II";
        private const String tree3 = "Type III";

        /*
         * Makes the talent screen as per specifications.
         * s The scene to display the talent screen on
         * f The font to be used with within the talent screen being created
         * activeTab The initial tree to display and tab to select
         */
        public TalentScreen(Scene s, SpriteFont f, int activeTab)
        {
            this.activeTab = activeTab;
            CreateFrame(s, f);

            /*Set up any specific stuff with the talent screen here,
             * put stuff in the frame,
             * etc...
            blah... blah.. blah.
            */
        }

        private void CreateFrame(Scene s, SpriteFont f)
        {
            backgroundFrame = new G2DPanel();
            backgroundFrame.Bounds = new Rectangle(240, 90, 320, 420);
            backgroundFrame.Border = GoblinEnums.BorderFactory.LineBorder;
            backgroundFrame.Transparency = 1.0f;
            backgroundFrame.BackgroundColor = Color.Black;            

            // Create the main panel which holds all other GUI components
            mainFrame = new G2DPanel();
            mainFrame.Bounds = new Rectangle(10, 10, 300, 400);
            mainFrame.Border = GoblinEnums.BorderFactory.LineBorder;
            mainFrame.Transparency = 0.85f;  // Ranges from 0 (fully transparent) to 1 (fully opaque)
            
            //Set up the 3 tabs
            for (int i = 0; i < 3; i++)
            {
                tab[i] = new G2DButton(i == 0 ? tree1 : i == 1 ? tree2 : tree3);
                tab[i].TextFont = f;
                tab[i].Bounds = new Rectangle(100 * i, 0, 100, 48);
                tab[i].BackgroundColor = (activeTab == i ? Color.SeaGreen : Color.LightGray);
                tab[i].ActionPerformedEvent += new ActionPerformed(HandleTabButtonPress);
                mainFrame.AddChild(tab[i]);
            }
            ChangeToTree(activeTab);

            backgroundFrame.AddChild(mainFrame);
            s.UIRenderer.Add2DComponent(backgroundFrame);
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
                tab[i].BackgroundColor = Color.SeaGreen;
                activeTab = i;
                ChangeToTree(i);

                //Get outta here
                return;
            }
        }

        private void ChangeToTree(int tree)
        {
            if (tree < 0 || tree > 2)
                throw new Exception("Tree must be 0, 1, or 2");

            if (talentFrame == null)
                talentFrame = new G2DPanel();
        }
    }
}