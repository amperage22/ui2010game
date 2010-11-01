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
  class brb
  {
    Scene scene;
    MenuStates curState;
    InGameStates gameState;
    ContentManager content;
    G2DPanel brbFrame;
    public brb(Scene scene, MenuStates menuState, ContentManager content, InGameStates gameState)
    {
      this.scene = scene;
      this.curState = menuState;
      this.content = content;
      this.gameState = gameState;

      createObject();
    }

    private void createObject()
    {

      /*
      GeometryNode sphereNode = new GeometryNode("Sphere");
      sphereNode.Model = new Sphere(3, 60, 60);
=======
        G2DButton brb;
      //GeometryNode sphereNode = new GeometryNode("Sphere");
      //sphereNode.Model = new Sphere(3, 60, 60);
>>>>>>> .r71

      //TransformNode sphereTransNode = new TransformNode();
      //sphereTransNode.Scale = new Vector3(0.15f, 0.15f, 0.15f);
      //sphereTransNode.Translation = new Vector3(0, 1.7f, -5);
      //Material sphereMaterial = new Material();
      //sphereMaterial.Diffuse = new Vector4(0.5f, 0, 0, 1);
      //sphereMaterial.Specular = Color.White.ToVector4();
      //sphereMaterial.SpecularPower = 10;

      //sphereNode.Material = sphereMaterial;
      //scene.RootNode.AddChild(sphereTransNode);
      //sphereTransNode.AddChild(sphereNode);
      */

      brbFrame = new G2DPanel();
      brbFrame.Bounds = new Rectangle(350, 0, 115, 115);
      brbFrame.Border = GoblinEnums.BorderFactory.LineBorder;
      brbFrame.Transparency = 1.0f;
      brbFrame.BackgroundColor = Color.Black;

      Console.WriteLine(curState);
      updateMenuBrb(curState,gameState);
      scene.UIRenderer.Add2DComponent(brbFrame);
    }

    public void updateMenuBrb(MenuStates state, InGameStates gameState)
    {
      //Console.WriteLine(state);
      switch (state)
      {
        case MenuStates.INGAME:
          switch(gameState)
          {
            case InGameStates.ATTACK:
              brbFrame.Texture = content.Load<Texture2D>("Textures/brb/brbAttack");
              break;
            case InGameStates.DAMAGE:
              brbFrame.Texture = content.Load<Texture2D>("Textures/brb/brbDamage");
              break;
            case InGameStates.DISCARD:
              brbFrame.Texture = content.Load<Texture2D>("Textures/brb/brbDiscard");
              break;
            case InGameStates.DRAW:
              brbFrame.Texture = content.Load<Texture2D>("Textures/brb/brbDraw");
              break;
            case InGameStates.SUMMON:
              brbFrame.Texture = content.Load<Texture2D>("Textures/brb/brbSummon");
              break;
            case default(InGameStates):
              break;
          }
          break;
        case MenuStates.INVENTORY:
          brbFrame.Texture = content.Load<Texture2D>("Textures/brb/brbInventory");
          break;
        case MenuStates.MARKET:
          brbFrame.Texture = content.Load<Texture2D>("Textures/brb/brbMarket");
          break;
        case MenuStates.TITLE:
          brbFrame.Texture = content.Load<Texture2D>("Textures/brb/brbArrg");
          break;
        case MenuStates.TALENT:
          brbFrame.Texture = content.Load<Texture2D>("Textures/brb/brbTalent");
          break;
        case default(MenuStates):
          brbFrame.Texture = content.Load<Texture2D>("Textures/brb/brbArrg");
          break;
      }
    }

  }
}
