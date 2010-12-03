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
using GoblinXNA.UI.UI2D;

namespace ARRG_Game
{
  class healthBar
  {

    Player player;
    Scene scene;
    int health, mana, playerNum, healthRatio, manaRatio;
    Boolean player1;
    G2DLabel playerName, barHealth, barMana;
    ContentManager content;

    public healthBar(int playerNum, int health, int mana)
    {
      this.scene = GlobalScene.scene;
      this.playerNum = playerNum;
      this.health = health;
      this.mana = mana;
      this.content = State.Content;
      healthRatio = (360 - 57) / health; // (barlength-(brblength/2)) / Health
      manaRatio = (220 - 57) / mana;
      createObjects();
    }

    private void createObjects()
    {
      
      //For player: To change Health +/- from the X value of the Translation and Scale Vectors
      barMana = new G2DLabel();
      barMana.Transparency = 0.7f;
      barMana.BackgroundColor = Color.Blue;

      barHealth = new G2DLabel();
      barHealth.Transparency = 1.0f;
      barHealth.BackgroundColor = Color.Red;

      playerName = new G2DLabel();
      playerName.Transparency = 1.0f;
      playerName.BackgroundColor = Color.Black;
      playerName.TextFont = content.Load<SpriteFont>("UIFont");

      //For player: To change Health +/- from the X value of the Translation and Scale Vectors

      if (playerNum ==1)
      {
        playerName.Bounds = new Rectangle(235, -5, 136, 44);
        playerName.Texture = content.Load<Texture2D>("Textures/healthbar/player1"); 
        barHealth.Bounds = new Rectangle(40, 20, 360, 30);
        barHealth.Texture = content.Load<Texture2D>("Textures/healthbar/healthp1");
        barMana.Bounds = new Rectangle(180, 40, 220, 60);
        barMana.Texture = content.Load<Texture2D>("Textures/healthbar/manaBar");

        
      }
      else
      {
        playerName.Bounds = new Rectangle(430, -5, 136, 44);
        playerName.Texture = content.Load<Texture2D>("Textures/healthbar/player2");
        barHealth.Bounds = new Rectangle(400, 20, 360, 30);
        barHealth.Texture = content.Load<Texture2D>("Textures/healthbar/healthp2");
        barMana.Bounds = new Rectangle(400, 40, 220, 60);
        barMana.Texture = content.Load<Texture2D>("Textures/healthbar/manaBar");
      }

      //scene.UIRenderer.Add2DComponent(barMana);
      scene.UIRenderer.Add2DComponent(barHealth);
      scene.UIRenderer.Add2DComponent(playerName);
      
    }

    public void adjustHealth(int modifier)
    {
        int newmod = modifier * healthRatio;
      if(playerNum ==1)
      {
        barHealth.Bounds = new Rectangle((barHealth.Bounds.X - newmod), 20, (barHealth.Bounds.Width + newmod), 30);

      }
      else
      {
        barHealth.Bounds = new Rectangle(400, 20, (barHealth.Bounds.Width + newmod), 30);
        //Console.WriteLine("X:" + healthTransNode.Translation.X + " Y:" + healthTransNode.Translation.Y + " Z:" + healthTransNode.Translation.Z);
      }
    }

    public void adjustMana(int modifier)
    {
      int newmod = modifier * manaRatio;
      if (playerNum == 1)
      {
        barMana.Bounds = new Rectangle((barMana.Bounds.X - newmod), 40, (barMana.Bounds.Width + newmod), 60);

      }
      else
      {
        barMana.Bounds = new Rectangle(400, 40, (barMana.Bounds.Width + newmod), 60);
      }
    }

    public void Kill()
    {
        scene.UIRenderer.Remove2DComponent(playerName);
        //scene.UIRenderer.Remove2DComponent(barMana);
        scene.UIRenderer.Remove2DComponent(barHealth);
    }
  }
}
