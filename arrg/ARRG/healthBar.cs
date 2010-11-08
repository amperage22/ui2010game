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
      healthBarLength = 1.2f;
      manaBarLength = healthBarLength / 2;
      healthRatio = healthBarLength / ((float)health);
      manaRatio = manaBarLength / ((float)mana);
      createObjects();
      

    }

    private void createObjects()
    {

      healthNode.Model = new Box(Vector3.One * 3);
      Material healthMat = new Material();
      healthMat.Diffuse = Color.Red.ToVector4();
      healthMat.Specular = Color.White.ToVector4();
      healthMat.SpecularPower = 5;
      healthNode.Material = healthMat;
      
      //For player: To change health +/- from the X value of the Translation and Scale Vectors
 
      manaNode.Model = new Box(Vector3.One * 3);

      Material manaMat = new Material();
      manaMat.Diffuse = Color.Blue.ToVector4();
      manaMat.Specular = Color.White.ToVector4();
      manaMat.SpecularPower = 5;
      manaNode.Material = manaMat;

      //For player: To change health +/- from the X value of the Translation and Scale Vectors

      if (playerNum ==1)
      {
        healthTransNode.Translation = new Vector3(-1.5f, 2.1f, -6);
        healthTransNode.Scale = new Vector3(healthBarLength, 0.14f, 0.1f);
        manaTransNode.Translation = new Vector3(-0.9f, 2.0f, -6);
        manaTransNode.Scale = new Vector3(manaBarLength, 0.15f, 0.1f);
      }
      else
      {
        healthTransNode.Translation = new Vector3(1.5f, 2.1f, -6);
        healthTransNode.Scale = new Vector3(healthBarLength, 0.14f, 0.1f);
        manaTransNode.Translation = new Vector3(0.9f, 2.0f, -6);
        manaTransNode.Scale = new Vector3(manaBarLength, 0.15f, 0.1f);
      }

      scene.RootNode.AddChild(manaTransNode);
      manaTransNode.AddChild(manaNode);

      scene.RootNode.AddChild(healthTransNode);
      healthTransNode.AddChild(healthNode);
    }

    public void adjustHealth(int modifier)
    {
      Console.WriteLine(modifier);
      float amount = modifier * healthRatio;
      healthBarLength = amount;
      healthTransNode.Scale = new Vector3(amount, 0, 0);
      if (playerNum == 1)
      {
        healthTransNode.Translation = new Vector3(-modifier, 0, 0);
        Console.WriteLine(healthTransNode.Translation.X);

      }
      else
      {
        healthTransNode.Translation = new Vector3(modifier / 2, 0, 0);
        Console.WriteLine(healthTransNode.Translation.X);
      }
    }

    public void adjustMana(int modifier)
    {
      float amount = modifier * manaRatio;
      manaBarLength = amount;
      manaTransNode.Scale = new Vector3(amount, 0, 0);
      if (playerNum == 1)
      {
        manaTransNode.Translation = new Vector3(-modifier, 0, 0);

      }
      else
      {
        manaTransNode.Translation = new Vector3(modifier / 2, 0, 0);
      }

    }

    public void Kill()
    {
        scene.RootNode.RemoveChild(healthTransNode);
        scene.RootNode.RemoveChild(manaTransNode);
    }
  }
}
