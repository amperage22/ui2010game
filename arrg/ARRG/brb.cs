using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;
using GoblinXNA.UI;
using GoblinXNA.UI.UI2D;

namespace ARRG_Game
{
    class Brb
    {
        Scene scene;
        MenuStates curState;
        InGameStates gameState;
        ContentManager content;
        G2DLabel brbButton;
        public Brb(ref Scene scene, MenuStates menuState, InGameStates gameState)
        {
            this.scene = scene;
            this.curState = menuState;
            
            this.content = State.Content;
            this.gameState = gameState;

            createObject();
        }

        private void createObject()
        {
            brbButton = new G2DLabel();
            brbButton.Bounds = new Rectangle(350, 0, 115, 115);
            brbButton.Transparency = 1.0f;
            brbButton.BackgroundColor = Color.Black;
            brbButton.Texture = content.Load<Texture2D>("Textures/brb/brbArrg");
            brbButton.MouseReleasedEvent += new MouseReleased(updateMenuBrb);
            brbButton.TextFont = content.Load<SpriteFont>("UIFont");

            scene.UIRenderer.Add2DComponent(brbButton);
        }

        public void updateMenuState(MenuStates s)
        {
            curState = s;
        }
        public void updateGameState(InGameStates s)
        {
            gameState = s;
        }
        public MenuStates getMenuState()
        {
            return curState;
        }
        public InGameStates getInGameState()
        {
            return gameState;
        }

        private void updateMenuBrb(int mouseButton, Point mouse)
        {
            switch (curState)
            {
                case MenuStates.INGAME: gameState = (gameState == InGameStates.DISCARD ? InGameStates.DRAW : ++gameState); brbButton.Text = "";
                    switch (gameState)
                    {
                        case InGameStates.ATTACK:
                            //brbButton.Texture = content.Load<Texture2D>("Textures/brb/brbAttack");
                            brbButton.Texture = null;
                            brbButton.BackgroundColor = Color.Red;
                            brbButton.TextColor = Color.White;
                            brbButton.Text = gameState.ToString();
                            
                            break;
                        case InGameStates.DAMAGE:
                            //brbButton.Texture = content.Load<Texture2D>("Textures/brb/brbDamage");
                            brbButton.Texture = null;
                            brbButton.BackgroundColor = Color.Red;
                            brbButton.TextColor = Color.White;
                            brbButton.Text = gameState.ToString();
                            break;
                        case InGameStates.DISCARD:
                            //brbButton.Texture = content.Load<Texture2D>("Textures/brb/brbDiscard");
                            brbButton.Texture = null;
                            brbButton.BackgroundColor = Color.Red;
                            brbButton.TextColor = Color.White;
                            brbButton.Text = gameState.ToString();
                            break;
                        case InGameStates.DRAW:
                            //brbButton.Texture = content.Load<Texture2D>("Textures/brb/brbDraw");
                            brbButton.Texture = null;
                            brbButton.BackgroundColor = Color.Red;
                            brbButton.TextColor = Color.White;
                            brbButton.Text = gameState.ToString();
                            break;
                        case InGameStates.SUMMON:
                            brbButton.Texture = content.Load<Texture2D>("Textures/brb/brbSummon");
                            break;
                        case default(InGameStates):
                            break;
                    }
                    break;
                case MenuStates.INVENTORY:
                    brbButton.Texture = content.Load<Texture2D>("Textures/brb/brbInventory");
                    break;
                case MenuStates.MARKET:
                    brbButton.Texture = content.Load<Texture2D>("Textures/brb/brbMarket");
                    break;
                case MenuStates.TITLE:
                    brbButton.Texture = content.Load<Texture2D>("Textures/brb/brbArrg");
                    break;
                case MenuStates.TALENT:
                    brbButton.Texture = content.Load<Texture2D>("Textures/brb/brbTalent");
                    break;
                case default(MenuStates):
                    brbButton.Texture = content.Load<Texture2D>("Textures/brb/brbArrg");
                    break;
            }
        }

    }
}
