/************************************************************************************ 
 * Copyright (c) 2008-2010, Columbia University
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Columbia University nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY COLUMBIA UNIVERSITY ''AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * 
 * ===================================================================================
 * Author: Ohan Oda (ohan@cs.columbia.edu)
 * Note: Only a small amount of the code here is from the tutorial.
 * 
 *************************************************************************************/

//#define USE_ARTAG

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
using GoblinXNA.Graphics;
using GoblinXNA.SceneGraph;
using Model = GoblinXNA.Graphics.Model;
using GoblinXNA.Graphics.Geometry;
using GoblinXNA.Graphics.ParticleEffects;
using GoblinXNA.Device.Generic;
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;

namespace ARRG_Game
{
    enum MenuStates { NONE, TITLE, TALENT, PRE_GAME, MARKET, INVENTORY, INGAME };
    enum InGameStates { NONE, DRAW, SUMMON, ATTACK, DAMAGE, DISCARD };
    enum CreatureType { NONE, BEASTS, DRAGONKIN, ROBOTS, ALL };
    enum CreatureID
    {
        BEAR = 0, PENGUIN, RHINO, TIGER, //This enum directly affects the market layout
        DALEK, GUNDAM, SAMUS, CENTURION,
        DRAGON1, DRAGON2, ESCAFLOWNE, DRAGON3,
        JONATHAN, MEYNARD, ALEX
    };
    enum CardType { NONE, STAT_MOD, DMG_DONE, DMG_PREVENT };
    enum ModifierType
    {
        NONE, POWER, DAMAGE_MOD, CRIT, HIT, DODGE, HP, HP_MOD, PARRY,
        ADDITIONAL_ATTACK_CHANCE, FIREBREATH_ATTACK_CHANCE, LIGHTNING_ATTACK_CHANCE
    };
    public class ARRG : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        Player p, p2;
        List<MonsterBuilder> monsters, initial_monsters;
        List<Card> cards;
        Scene scene;
        MarkerNode groundMarkerNode;
        private const int dice_count = 6;
        healthBar hb, hb2;
        TitleScreen titleScreen;
        TalentScreen talentScreen;
        MarketScreen marketScreen;
        InventoryScreen inventoryScreen;
        SpriteBatch spriteBatch;
        preGameScreen preGame;
        Brb bigRed;

        //Set up the states
        MenuStates menuState = MenuStates.TITLE;
        InGameStates gameState = InGameStates.DRAW;

        // set this to false if you are going to use a webcam
        bool useStaticImage = false;

        public ARRG()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Initialize the GoblinXNA framework
            State.InitGoblin(graphics, Content, "");

            // Initialize the scene graph
            scene = new Scene(this);

            // Use the newton physics engine to perform collision detection
            scene.PhysicsEngine = new NewtonPhysics();

            this.IsMouseVisible = true;

            // For some reason, it causes memory conflict when it attempts to update the
            // marker transformation in the multi-threaded code, so if you're using ARTag
            // then you should not enable the marker tracking thread
#if !USE_ARTAG
            State.ThreadOption = (ushort)ThreadOptions.MarkerTracking;
#endif

            // Set up optical marker tracking
            // Note that we don't create our own camera when we use optical marker
            // tracking. It'll be created automatically
            SetupMarkerTracking();

            // Set up the lights used in the scene
            CreateLights();

            // Create 3D objects
            CreateObjects();

            // Create the ground that represents the physical ground marker array
            CreateMonsterList();

            // Use per pixel lighting for better quality (If you using non NVidia graphics card,
            // setting this to true may reduce the performance significantly)
            scene.PreferPerPixelLighting = true;

            // Enable shadow mapping
            // NOTE: In order to use shadow mapping, you will need to add 'PostScreenShadowBlur.fx'
            // and 'ShadowMap.fx' shader files as well as 'ShadowDistanceFadeoutMap.dds' texture file
            // to your 'Content' directory
            scene.EnableShadowMapping = false;

            // Show Frames-Per-Second on the screen for debugging
            State.ShowFPS = true;


            //Player player = new Player();
            //Player player2 = new Player();
            //hb = new healthBar(scene, player, true);
            //hb2 = new healthBar(scene, player2, false);
            //hb2.adjustHealth(-10);

            //Set up the stuff needed for the first (title) state
            titleScreen = new TitleScreen(Content, scene);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            base.Initialize();
        }

        private void CreateLights()
        {
            // Create a directional light source
            LightSource lightSource = new LightSource();
            lightSource.Direction = new Vector3(1, -1, -1);
            lightSource.Diffuse = Color.White.ToVector4();
            lightSource.Specular = new Vector4(0.6f, 0.6f, 0.6f, 1);

            // Create a light node to hold the light source
            LightNode lightNode = new LightNode();
            lightNode.LightSources.Add(lightSource);

            scene.RootNode.AddChild(lightNode);
        }

        private void SetupMarkerTracking()
        {
            // Create our video capture device that uses DirectShow library. Note that 
            // the combinations of resolution and frame rate that are allowed depend on 
            // the particular video capture device. Thus, setting incorrect resolution 
            // and frame rate values may cause exceptions or simply be ignored, depending 
            // on the device driver.  The values set here will work for a Microsoft VX 6000, 
            // and many other webcams.
            IVideoCapture captureDevice = null;

            if (useStaticImage)
            {
                //do nothing
            }
            else
            {
                captureDevice = new DirectShowCapture();
                captureDevice.InitVideoCapture(0, FrameRate._60Hz, Resolution._640x480,
                    ImageFormat.R8G8B8_24, false);
            }

            // Add this video capture device to the scene so that it can be used for
            // the marker tracker
            scene.AddVideoCaptureDevice(captureDevice);

            IMarkerTracker tracker = null;

#if USE_ARTAG
			// Create an optical marker tracker that uses ARTag library
			tracker = new ARTagTracker();
			// Set the configuration file to look for the marker specifications
			tracker.InitTracker(638.052f, 633.673f, captureDevice.Width,
				captureDevice.Height, false, "ARTag.cf");
#else
            // Create an optical marker tracker that uses ALVAR library
            tracker = new ALVARMarkerTracker();
            ((ALVARMarkerTracker)tracker).MaxMarkerError = 0.02f;
            tracker.InitTracker(captureDevice.Width, captureDevice.Height, "calib.xml", 9.0);
#endif

            // Set the marker tracker to use for our scene
            scene.MarkerTracker = tracker;

            // Display the camera image in the background. Note that this parameter should
            // be set after adding at least one video capture device to the Scene class.
            scene.ShowCameraImage = true;
        }

        private void CreateMonsterList()
        {
            monsters = new List<MonsterBuilder>();
            initial_monsters = new List<MonsterBuilder>();
            cards = new List<Card>();

            monsters.Add(new MonsterBuilder(CreatureID.BEAR, CreatureType.BEASTS, "Bearrorist", "bear", Content.Load<Texture2D>("Textures/inventory/bear"), 3, 4, true, 1, 0));
            monsters.Add(new MonsterBuilder(CreatureID.PENGUIN, CreatureType.BEASTS, "Penguinist", "penguin", Content.Load<Texture2D>("Textures/inventory/penguin"), 1, 2, true, 1, 0));
            monsters.Add(new MonsterBuilder(CreatureID.RHINO, CreatureType.BEASTS, "Rhymenoceros", "rhino", Content.Load<Texture2D>("Textures/inventory/rhino"), 3, 5, true, 1, 0));
            monsters.Add(new MonsterBuilder(CreatureID.TIGER, CreatureType.BEASTS, "Tigeriffic", "tiger", Content.Load<Texture2D>("Textures/inventory/tiger"), 5, 3, true, 1, 0));

            monsters.Add(new MonsterBuilder(CreatureID.DALEK, CreatureType.ROBOTS, "Dalek", "dalek", Content.Load<Texture2D>("Textures/inventory/dalek"), 4, 5, true, 1, 0));
            monsters.Add(new MonsterBuilder(CreatureID.GUNDAM, CreatureType.ROBOTS, "Gundam", "gundam", Content.Load<Texture2D>("Textures/inventory/gundam"), 8, 8, true, 1, 0));
            monsters.Add(new MonsterBuilder(CreatureID.SAMUS, CreatureType.ROBOTS, "Samus", "samus", Content.Load<Texture2D>("Textures/inventory/samus"), 3, 4, true, 1, 0));
            monsters.Add(new MonsterBuilder(CreatureID.CENTURION, CreatureType.ROBOTS, "Centurion", "centurion", Content.Load<Texture2D>("Textures/inventory/centurion"), 4, 2, false, 1, 0));

            monsters.Add(new MonsterBuilder(CreatureID.DRAGON1, CreatureType.DRAGONKIN, "Whelp", "dragon1", Content.Load<Texture2D>("Textures/inventory/dragon1"), 1, 3, true, 1, 0));
            monsters.Add(new MonsterBuilder(CreatureID.DRAGON2, CreatureType.DRAGONKIN, "Drake", "dragon2", Content.Load<Texture2D>("Textures/inventory/dragon2"), 3, 5, false, 1, 0));
            monsters.Add(new MonsterBuilder(CreatureID.ESCAFLOWNE, CreatureType.DRAGONKIN, "UNKNOWN1", "escaflowne", Content.Load<Texture2D>("Textures/inventory/unknown"), 0, 0, false, 50, 0));
            monsters.Add(new MonsterBuilder(CreatureID.DRAGON3, CreatureType.DRAGONKIN, "UNKNOWN2", "dragon3", Content.Load<Texture2D>("Textures/inventory/unknown"), 0, 0, false, 50, 0));

            //Set up the initial monsters for use after the talent screen has been submitted
            initial_monsters.Add(new MonsterBuilder(CreatureID.JONATHAN, CreatureType.DRAGONKIN, "ConeOfFire", "cone", Content.Load<Texture2D>("Textures/inventory/d_jonathan"), 3, 1, false, 50, 0));
            initial_monsters.Add(new MonsterBuilder(CreatureID.MEYNARD, CreatureType.ROBOTS, "Tank", "tank", Content.Load<Texture2D>("Textures/inventory/tank"), 6, 3, false, 50, 0));
            initial_monsters.Add(new MonsterBuilder(CreatureID.ALEX, CreatureType.BEASTS, "WTF", "alexmodel", Content.Load<Texture2D>("Textures/inventory/d_alex"), 2, 2, false, 1, 0));
            monsters.AddRange(initial_monsters);

            p2.PurchasedMonsters = monsters; //For testing purposes

<<<<<<< .mine
            cards.Add(new Card(scene, 164, 5, ModifierType.HP, CreatureType.ALL));
            cards.Add(new Card(scene, CardType.DMG_DONE, 183, 5));
            //p.Cards = cards;
=======
            //Modifier type spells
            cards.Add(new Card(scene, 164, 2, 4, ModifierType.HP, CreatureType.ALL));
            cards.Add(new Card(scene, 165, 1, 2, ModifierType.HP, CreatureType.ALL));
            cards.Add(new Card(scene, 166, 2, 4, ModifierType.POWER, CreatureType.ALL));
            cards.Add(new Card(scene, 167, 1, 2, ModifierType.POWER, CreatureType.ALL));
            cards.Add(new Card(scene, 168, 2, 4, ModifierType.CRIT, CreatureType.ALL));
            cards.Add(new Card(scene, 169, 1, 2, ModifierType.CRIT, CreatureType.ALL));
            cards.Add(new Card(scene, 170, 2, 4, ModifierType.ADDITIONAL_ATTACK_CHANCE, CreatureType.ALL));
            cards.Add(new Card(scene, 171, 1, 2, ModifierType.ADDITIONAL_ATTACK_CHANCE, CreatureType.ALL));
            cards.Add(new Card(scene, 172, 3, 6, ModifierType.POWER, CreatureType.ALL));
            cards.Add(new Card(scene, 173, 3, 6, ModifierType.HP, CreatureType.ALL));

            //dmg dealing spells
            cards.Add(new Card(scene, CardType.DMG_DONE, 174, 1, 2));
            cards.Add(new Card(scene, CardType.DMG_DONE, 175, 1, 2));
            cards.Add(new Card(scene, CardType.DMG_DONE, 176, 2, 4));
            cards.Add(new Card(scene, CardType.DMG_DONE, 177, 2, 4));
            cards.Add(new Card(scene, CardType.DMG_DONE, 178, 3, 5));

            //dmg preventing spells
            cards.Add(new Card(scene, CardType.DMG_PREVENT, 179, 1, 2));
            cards.Add(new Card(scene, CardType.DMG_PREVENT, 180, 1, 2));
            cards.Add(new Card(scene, CardType.DMG_PREVENT, 181, 2, 4));
            cards.Add(new Card(scene, CardType.DMG_PREVENT, 182, 2, 4));
            cards.Add(new Card(scene, CardType.DMG_PREVENT, 183, 3, 6));
>>>>>>> .r160
        }

        private void CreateObjects()
        {
            //TODO: replace 54 with a constant to show what the ids are of.
            int[] ground_markers = new int[54];
            for (int i = 0; i < ground_markers.Length; i++)
                ground_markers[i] = i;
            groundMarkerNode = new MarkerNode(scene.MarkerTracker, "ground_markers.txt", ground_markers);


            scene.RootNode.AddChild(groundMarkerNode);

            p = new Player(scene, 1, groundMarkerNode);
            p2 = new Player(scene, 2, groundMarkerNode);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (menuState)
            {
                case MenuStates.TITLE: UpdateTitle(); break;
                case MenuStates.TALENT: UpdateTalent(); break;
                case MenuStates.PRE_GAME: UpdatePreGame(); break;
                case MenuStates.MARKET: UpdateMarket(); break;
                case MenuStates.INVENTORY: UpdateInventory(); break;
                case MenuStates.INGAME: UpdateInGame(); break;
                case default(MenuStates): break;
            }
            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            base.Draw(gameTime);
            switch (menuState)
            {
                case MenuStates.TITLE: DrawTitle(gameTime); break;
                case MenuStates.TALENT: DrawTalent(); break;
                case MenuStates.PRE_GAME: DrawPreGame(); break;
                case MenuStates.MARKET: DrawMarket(); break;
                case MenuStates.INVENTORY: DrawInventory(); break;
                case MenuStates.INGAME: DrawInGame(); break;
                case default(MenuStates): break;
            }
        }

        /// <summary>
        /// Update method for Title state
        /// </summary>
        private void UpdateTitle()
        {
            if (titleScreen.playerChoice() != CreatureType.NONE && titleScreen.playerChoice() != CreatureType.ALL)
            {
                switch (titleScreen.playerChoice())
                {
                    case CreatureType.BEASTS: talentScreen = new TalentScreen(scene, Content, 0); break;
                    case CreatureType.DRAGONKIN: talentScreen = new TalentScreen(scene, Content, 1); break;
                    case CreatureType.ROBOTS: talentScreen = new TalentScreen(scene, Content, 2); break;
                }
                titleScreen.Kill(scene);
                //Should we somehow free up the memory of the title screen?
                menuState = MenuStates.TALENT;
                //bigRed.updateMenuBrb(menuState,gameState);
                talentScreen.Display();
            }
        }
        /// <summary>
        /// Update method for Talent state
        /// </summary>
        private void UpdateTalent()
        {
            talentScreen.update(new Point(Mouse.GetState().X, Mouse.GetState().Y));
            if (talentScreen.wasSubmitted())
            {
                p.Buffs = talentScreen.getBuffs();

                //Add the initial monsterbuilders to the player's list */
                p.PurchasedMonsters = new List<MonsterBuilder>(initial_monsters);
                p.SelectedMonsters = new List<MonsterBuilder>(initial_monsters);

                preGame = new preGameScreen(scene, Content);
                menuState = MenuStates.PRE_GAME;
                preGame.display();
            }
        }
        /// <summary>
        /// Update method for Pre-game state
        /// </summary>
        private void UpdatePreGame()
        {
            if (preGame.isPregameFinished())
            {

                //TEST Gotta figure out if we want to create random values if the gamer wants to just jump right in
                //menuState = MenuStates.MARKET;  //Should be moved to PRE_GAME and called upon stage transition
                //marketScreen = new MarketScreen(scene, Content, p, monsters);  //Should be moved to PRE_GAME and called upon stage transition
                //marketScreen.Display();
                //END TEST
                switch (menuState = preGame.getDecision())
                {
                    case MenuStates.MARKET:
                        if (marketScreen == null)
                            marketScreen = new MarketScreen(scene, Content, monsters);
                        marketScreen.Display(p);
                        break;
                    case MenuStates.INVENTORY:
                        if (inventoryScreen == null)
                            inventoryScreen = new InventoryScreen(scene, Content, monsters);
                        inventoryScreen.Display(p);
                        break;
                    case MenuStates.INGAME:
                        bigRed = new Brb(scene, menuState, gameState);
                        p.showHealth();
                        p2.showHealth();
                        break;
                }
            }
            else if (!preGame.isDisplaying())
                preGame.display();
        }
        /// <summary>
        /// Update method for Market state
        /// </summary>
        private void UpdateMarket()
        {
            marketScreen.update(new Point(Mouse.GetState().X, Mouse.GetState().Y));
            if (marketScreen.wasSubmitted())
            {
                //This will automatically update the player passed during creation
                marketScreen.commit();

                menuState = MenuStates.PRE_GAME;
            }
        }
        /// <summary>
        /// Update method for Inventory state
        /// </summary>
        private void UpdateInventory()
        {
            inventoryScreen.update(new Point(Mouse.GetState().X, Mouse.GetState().Y));
            if (inventoryScreen.wasSubmitted())
            {
                //This should set the array of selected monsters within the player automatically
                inventoryScreen.commit();

                menuState = MenuStates.PRE_GAME;
            }
        }
        /// <summary>
        /// Update method for InGame state
        /// </summary>
        private void UpdateInGame()
        {
            switch (gameState)
            {
                case InGameStates.DRAW: UpdateDraw(); break;
                case InGameStates.SUMMON: UpdateSummon(); break;
                case InGameStates.ATTACK: UpdateAttack(); break;
                case InGameStates.DAMAGE: UpdateDamage(); break;
                case InGameStates.DISCARD: UpdateDiscard(); break;
            }
        }
        private void UpdateDraw()
        {
            p.updateDraw();
            p2.updateDraw();
            gameState = bigRed.getInGameState();
        }
        private void UpdateSummon()
        {
            p.updateSummon();
            p2.updateSummon();
            gameState = bigRed.getInGameState();
        }
        private void UpdateAttack()
        {
            p.updateAttack(p2.Die);
            p2.updateAttack(p.Die);

            foreach (Card c in cards)
            {
                c.getNearestCreature(p.Die, p2.Die);
                //c.update();
            }
            gameState = bigRed.getInGameState();

            if (gameState == InGameStates.DAMAGE)
            {
                p.applyHealthMods();
                p2.applyHealthMods();
                foreach (Card c in cards)
                    c.castSpell();
//                DateTime endTime = DateTime.Now.AddSeconds(5.0);
  //              while (DateTime.Now.CompareTo(endTime) < 0)
    //            {

<<<<<<< .mine

                  /*Call attack animations here
                  foreach (Die die in p.Die)
                  {
                    if (die.CurrentMonster != null)
                      for (int c = 0; c < 1000; c++)
                      {
                        if (c % 100 == 0)
                        {
                          die.CurrentMonster.startAttackAnimation();
                          Console.WriteLine("Start Attack" +DateTime.Now.Millisecond);
                        }
                      }
                  }
      //          }
              */
                  foreach (Die die in p.Die)
                  {
                    Console.WriteLine("New Die");
                    if (die.CurrentMonster != null)
                      for (int c = 0; c < 1000; c++)
                      {
                        if (c % 100 == 0)
                        {
                          die.CurrentMonster.startAttackAnimation();
                          Console.WriteLine("End Attack " + DateTime.Now.Millisecond + " " +c);
                        }
                      }
                  }

=======
                //Call attack animations here

>>>>>>> .r160
            }
        }
        private void UpdateDamage()
        {
            //End attack animations here
<<<<<<< .mine
      
=======
            p.updateDamage();
            p2.updateDamage();
>>>>>>> .r160
            gameState = bigRed.getInGameState();
            foreach (Die die in p.Die)
            {
              if (die.CurrentMonster != null)
                die.CurrentMonster.endAttackAnimation();
            }

            if (gameState == InGameStates.DISCARD)
            {
                p.applyMonsterDmg();
                p2.applyMonsterDmg();

                p.damageResolution();
                p2.damageResolution();

                p.applyPlayerDamage(p2);
                p2.applyPlayerDamage(p);
            }

        }
        private void UpdateDiscard()
        {
            gameState = bigRed.getInGameState();
        }


        /// <summary>
        /// Draw method for Title state
        /// </summary>
        private void DrawTitle(GameTime gameTime)
        {
            titleScreen.Draw(spriteBatch);

            /**
             * I could not seperate the 3-button UI that was created in TitleScreen
             * from the scene and so it would either draw the camera image over
             * the title screen texture OR would draw the buttons behind the title
             * screen texture.  This should fix that. */
            scene.UIRenderer.Draw(0.0f, false);
        }
        /// <summary>
        /// Draw method for Talent state
        /// </summary>
        private void DrawTalent()
        {

        }
        /// <summary>
        /// Draw method for Pre-game state
        /// </summary>
        private void DrawPreGame()
        {

        }
        /// <summary>
        /// Draw method for Market state
        /// </summary>
        private void DrawMarket()
        {

        }
        /// <summary>
        /// Draw method for Inventory state
        /// </summary>
        private void DrawInventory()
        {

        }
        /// <summary>
        /// Draw method for InGame state
        /// </summary>
        private void DrawInGame()
        {

        }
    }
}
