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
  class preGameScreen
  {
    enum preGameState { READY, DISPLAYING, FINISHED };
    MenuStates decision;
    private Scene scene;
    private ContentManager content;
    private preGameState state;
    //private G2DPanel menuFrame;
    private G2DButton arrg, inventory, game, market;
    private bool optionsOnScreen = false;
    //private string preGameDecision;

    public preGameScreen(Scene scene, ContentManager content)
    {
      this.scene = scene;
      this.content = content;

      createFrame();

      state = preGameState.READY;
    }

    public void display()
    {
      if (state != preGameState.DISPLAYING)
      {
        //scene.UIRenderer.Add2DComponent(menuFrame);
        //state = preGameState.DISPLAYING;
        scene.UIRenderer.Add2DComponent(arrg);
      }
    }

    private void createFrame()
    {
      //menuFrame = new G2DPanel();
      //menuFrame.Bounds = new Rectangle(10, 10, 400, 400);
      //menuFrame.Border = GoblinEnums.BorderFactory.EmptyBorder;
      //menuFrame.Transparency = 1;  // Ranges from 0 (fully transparent) to 1 (fully opaque)

      int buttonX, buttonY;
      buttonX = 310;
      buttonY = 250;


      Texture2D arrg_button = content.Load<Texture2D>("Textures/brb/brbArrg");
      arrg = new G2DButton("ARRG");
      arrg.Bounds = new Rectangle(buttonX, buttonY, 110, 110);
      arrg.Texture = arrg_button;;
      arrg.HighlightColor = Color.TransparentWhite;
      arrg.ActionPerformedEvent += new ActionPerformed(handleButtonPress);
      arrg.DrawBorder = false;

      Texture2D talent_button = content.Load<Texture2D>("Textures/brb/brbMarket");
      market = new G2DButton("Market");
      market.Bounds = new Rectangle(buttonX-150, buttonY - 150, 110, 110);
      market.Texture = talent_button;
      market.DrawBorder = false;
      market.HighlightColor = Color.TransparentWhite;
      market.ActionPerformedEvent += new ActionPerformed(handleButtonPress);


      Texture2D inventory_button = content.Load<Texture2D>("Textures/brb/brbInventory");
      inventory = new G2DButton("Inventory");
      inventory.Bounds = new Rectangle(buttonX + 150, buttonY - 150, 110, 110);
      inventory.Texture = inventory_button;
      inventory.DrawBorder = false;
      inventory.HighlightColor = Color.TransparentWhite;
      inventory.ActionPerformedEvent += new ActionPerformed(handleButtonPress);

      Texture2D game_button = content.Load<Texture2D>("Textures/brb/brbStart");
      game = new G2DButton("Game");
      game.Bounds = new Rectangle(buttonX, buttonY + 150, 110, 110);
      game.Texture = game_button;
      game.HighlightColor = Color.TransparentWhite;
      game.ActionPerformedEvent += new ActionPerformed(handleButtonPress);
      game.DrawBorder = false;
      //menuFrame.AddChild(game);
      //menuFrame.AddChild(arrg);
      //menuFrame.AddChild(inventory);
      //menuFrame.AddChild(talent);

    }

    public void handleButtonPress(object source)
    {
      G2DButton comp = (G2DButton)source;
      switch (comp.Text)
      {
        case "ARRG":
          togglePathsVisible();
          break;
        case "Market":
          decision = MenuStates.MARKET;
          finishPreGame();
          break;
        case "Game":
          decision = MenuStates.INGAME;
          finishPreGame();
          break;
        case "Inventory":
          decision = MenuStates.INVENTORY;
          finishPreGame();
          break;
      }
    }

    public void togglePathsVisible()
    {
        if (optionsOnScreen)
        {
            scene.UIRenderer.Remove2DComponent(inventory);
            scene.UIRenderer.Remove2DComponent(market);
            scene.UIRenderer.Remove2DComponent(game);
        }
        else
        {
            scene.UIRenderer.Add2DComponent(inventory);
            scene.UIRenderer.Add2DComponent(market);
            scene.UIRenderer.Add2DComponent(game);
        }
        optionsOnScreen = !optionsOnScreen;
    }

    public void finishPreGame()
    {
      togglePathsVisible();
      scene.UIRenderer.Remove2DComponent(arrg);
      state = preGameState.FINISHED;
    }

    public Boolean isPregameFinished()
    {
      return state == preGameState.FINISHED;
    }

    public bool isDisplaying()
    {
        return state == preGameState.DISPLAYING;
    }
    public MenuStates getDecision()
    {
        if (state != preGameState.FINISHED)
            throw new Exception("Not finished yet damnit!");
        state = preGameState.READY;
      return decision;
    }

  }
}
