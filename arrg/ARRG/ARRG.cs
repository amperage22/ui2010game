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
using GoblinXNA.Device.Capture;
using GoblinXNA.Device.Vision;
using GoblinXNA.Device.Vision.Marker;
using GoblinXNA.Device.Util;
using GoblinXNA.Physics;
using GoblinXNA.Helpers;

namespace ARRG_Game
{
    public class ARRG : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        Scene scene;
        MarkerNode groundMarkerNode;
        Die[] dice;
        private const int dice_count = 6;

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

            //Create the dice AR recognition stuff
            CreateDice();

            // Create the ground that represents the physical ground marker array
            CreateGround();

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

        private void CreateGround()
        {
            GeometryNode groundNode = new GeometryNode("Ground");

            #if USE_ARTAG
            groundNode.Model = new Box(85, 66, 0.1f);
            #else
            groundNode.Model = new Box(95, 59, 0.1f);
            #endif

            // Set this ground model to act as an occluder so that it appears transparent
            groundNode.IsOccluder = false;

            // Make the ground model to receive shadow casted by other objects with
            // CastShadows set to true
            groundNode.Model.ReceiveShadows = true;

            Material groundMaterial = new Material();
            groundMaterial.Diffuse = Color.Black.ToVector4();
            groundMaterial.Specular = Color.White.ToVector4();
            groundMaterial.SpecularPower = 20;

            groundNode.Material = groundMaterial;

            groundMarkerNode.AddChild(groundNode);
        }

        private void CreateDice()
        {
            //Create mat for blue cylinder
            Material mat = new Material();
            mat.Diffuse = new Vector4(0, 1, 0, 1);
            mat.Specular = Color.White.ToVector4();
            mat.SpecularPower = 10;
            mat.Emissive = Color.Blue.ToVector4();

            //Setup the marker numbers
            int[] side_marker = new int[1];
            MarkerNode[] side = new MarkerNode[6];
            dice = new Die[dice_count];
            for (int i = 0; i < dice_count; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    side_marker[0] = (i * 6) + (j + 128); //6 sides on a die, 128 is the beginning dice id
                    String config_file = String.Format("Content/dice_markers/die{0}side{1}.txt", i, j);
                    side[j] = new MarkerNode(scene.MarkerTracker, config_file, side_marker);//String.Format("Content/dice_markers/die_{0}_side_{1}.txt", i, j), side_marker);
                    
                    //Create the blue cylinder for the die marker
                    GeometryNode cylinderNode = new GeometryNode("Cylinder");
                    cylinderNode.Model = new Cylinder(i+j+4, i+j+4, i+j+4, i+j+4);
                    cylinderNode.Material = mat;
                    TransformNode cylinderTransNode = new TransformNode();
                    cylinderTransNode.AddChild(cylinderNode);
                    cylinderTransNode.Translation = new Vector3(0, 0, 25);
                    side[j].AddChild(cylinderTransNode);
                    scene.RootNode.AddChild(side[j]);
                }

                dice[i] = new Die(side);
            }
        }

        private void CreateObjects()
        {

            //TODO: replace 54 with a constant to show what the ids are of.
            int[] ground_markers = new int[54];
            for (int i = 0; i < ground_markers.Length; i++)
                ground_markers[i] = i;
            groundMarkerNode = new MarkerNode(scene.MarkerTracker, "ground_markers.txt", ground_markers);

            // Create a material to apply to the cylinder model
            Material mat1 = new Material();
            mat1.Diffuse = new Vector4(0, 1, 0, 1);
            mat1.Specular = Color.White.ToVector4();
            mat1.SpecularPower = 10;

            //Create the cylinder
            GeometryNode cylinderNode = new GeometryNode("Cylinder");
            cylinderNode.Model = new Cylinder(10, 10, 20, 20);
            cylinderNode.Material = mat1;
            TransformNode cylinderTransNode = new TransformNode();
            cylinderTransNode.AddChild(cylinderNode);
            cylinderTransNode.Translation = new Vector3(0, 0, 15);

            groundMarkerNode.AddChild(cylinderTransNode);

            scene.RootNode.AddChild(groundMarkerNode);
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
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
