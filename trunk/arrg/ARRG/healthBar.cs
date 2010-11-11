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
    int health, mana, playerNum;
    float healthBarLength,manaBarLength, healthRatio, manaRatio;
    Boolean player1;
    GeometryNode healthNode = new GeometryNode("Box");
    TransformNode healthTransNode = new TransformNode();
    GeometryNode manaNode = new GeometryNode("Box");
    TransformNode manaTransNode = new TransformNode();
    G2DLabel playerName, barHealth;
    ContentManager content;
   

    public healthBar(Scene scene, Player player, Boolean player1)
    {
      this.scene = scene;
      this.player = player;
      this.player1 = player1;
      health = player.Health;
      healthBarLength = 1.2f;
      manaBarLength = healthBarLength / 2;
      healthRatio = healthBarLength/((float)health);
      manaRatio = manaBarLength / ((float)mana);
      mana = player.Mana;

      createObjects();
    }

    public healthBar(Scene scene, int playerNum, int health, int mana)
    {
      this.scene = scene;
      this.playerNum = playerNum;
      this.health = health;
      this.mana = mana;
      this.content = State.Content;
      healthBarLength = 1.2f;
      manaBarLength = healthBarLength / 2;
      healthRatio = healthBarLength / ((float)health);
      manaRatio = manaBarLength / ((float)mana);
      createObjects();
    }

    private void createObjects()
    {

      
      //For player: To change health +/- from the X value of the Translation and Scale Vectors
 
      manaNode.Model = new Box(Vector3.One * 3);

      Material manaMat = new Material();
      manaMat.Diffuse = Color.Blue.ToVector4();
      manaMat.Specular = Color.White.ToVector4();
      manaMat.SpecularPower = 5;
      manaNode.Material = manaMat;
      manaTransNode.Scale = new Vector3(manaBarLength, 0.15f, 0.1f);

      barHealth = new G2DLabel();
      barHealth.Transparency = 1.0f;
      barHealth.BackgroundColor = Color.Red;

      playerName = new G2DLabel();
      playerName.Transparency = 1.0f;
      playerName.BackgroundColor = Color.Black;
      playerName.TextFont = content.Load<SpriteFont>("UIFont");

      //For player: To change health +/- from the X value of the Translation and Scale Vectors

      if (playerNum ==1)
      {
        manaTransNode.Translation = new Vector3(-0.9f, 2.0f, -6);
        playerName.Bounds = new Rectangle(235, -5, 136, 44);
        playerName.Texture = content.Load<Texture2D>("Textures/healthbar/player1"); 
        barHealth.Bounds = new Rectangle(40, 10, 360, 60);
        barHealth.Texture = content.Load<Texture2D>("Textures/healthbar/healthp1");

        
      }
      else
      {
        manaTransNode.Translation = new Vector3(0.9f, 2.0f, -6);
        playerName.Bounds = new Rectangle(430, -5, 136, 44);
        playerName.Texture = content.Load<Texture2D>("Textures/healthbar/player2");
        barHealth.Bounds = new Rectangle(400, 10, 360, 60);
        barHealth.Texture = content.Load<Texture2D>("Textures/healthbar/healthp2");
      }

      scene.RootNode.AddChild(manaTransNode);
      manaTransNode.AddChild(manaNode);

      //scene.RootNode.AddChild(healthTransNode);
      //healthTransNode.AddChild(healthNode);
      scene.UIRenderer.Add2DComponent(barHealth);
      scene.UIRenderer.Add2DComponent(playerName);
      
    }

    public void adjustHealth(int modifier)
    {
      int newmod = modifier * 15;
      if(playerNum ==1)
      {
        barHealth.Bounds = new Rectangle((barHealth.Bounds.X - newmod), 10, (barHealth.Bounds.Width + newmod), 60);

      }
      else
      {
        barHealth.Bounds = new Rectangle(400, 10, (barHealth.Bounds.Width + newmod), 60);
        //Console.WriteLine("X:" + healthTransNode.Translation.X + " Y:" + healthTransNode.Translation.Y + " Z:" + healthTransNode.Translation.Z);
      }
    }

    public void adjustMana(int modifier)
    {
      float amount = modifier * manaRatio;
      manaBarLength = manaBarLength + amount;
      manaTransNode.Scale += new Vector3(amount, 0, 0);
      if (playerNum == 1)
      {
        manaTransNode.Translation += new Vector3(-amount, 0, 0);

      }
      else
      {
        manaTransNode.Translation += new Vector3(amount / 2, 0, 0);
      }
    }

    public void Kill()
    {
        scene.RootNode.RemoveChild(healthTransNode);
        scene.RootNode.RemoveChild(manaTransNode);
    }
  }
}
