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
    InGameStates curState;

    public brb(Scene scene, InGameStates curState)
    {
      this.scene = scene;
      this.curState = curState;

      createObject();
    }

    private void createObject()
    {
        G2DButton brb;
      //GeometryNode sphereNode = new GeometryNode("Sphere");
      //sphereNode.Model = new Sphere(3, 60, 60);

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
      
    }

  }
}
