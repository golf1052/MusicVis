using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Diagnostics;
using Windows.Media.Render;
using Windows.Media;
using System.Collections.Generic;

namespace MusicVis
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        AudioGraph audioGraph;
        AudioFrameOutputNode frameOutputNode;

        DeviceInformation audioInput;
        DeviceInformation audioOutput;

        List<Sprite> leftSprites;
        List<Sprite> rightSprites;

        public Game1()
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
            base.Initialize();
        }

        private void AudioGraph_UnrecoverableErrorOccurred(AudioGraph sender, AudioGraphUnrecoverableErrorOccurredEventArgs args)
        {
            Debug.WriteLine("UNRECOVERABLE ERRORRRRRR");
        }

        private void AudioGraph_QuantumProcessed(AudioGraph sender, object args)
        {
            AudioFrame audioFrame = frameOutputNode.GetFrame();
            List<float[]> amplitudeData = HelperMethods.ProcessFrameOutput(audioFrame);
            List<float[]> channelData = HelperMethods.GetFftData(HelperMethods.ConvertTo512(amplitudeData, audioGraph), audioGraph);
            for (int i = 0; i < channelData.Count / 2; i++)
            {
                float[] leftChannel = channelData[i];
                float[] rightChannel = channelData[i + 1];

                for (int j = 0; j < 220; j++)
                {
                    int num = 100000;
                    leftSprites[j].drawRect.Width = (int)(num * leftChannel[j]);
                    leftSprites[j].drawRect.Height = (int)(num * leftChannel[j]);

                    rightSprites[j].drawRect.Width = (int)(num * rightChannel[j]);
                    rightSprites[j].drawRect.Height = (int)(num * rightChannel[j]);
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override async void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            leftSprites = new List<Sprite>();
            rightSprites = new List<Sprite>();
            for (int i = 0; i < 220; i++)
            {
                leftSprites.Add(new Sprite(graphics));
                leftSprites[i].position = new Vector2(100, 100 + i);
                leftSprites[i].color = new Color(World.Random.Next(0, 255), World.Random.Next(0, 255), World.Random.Next(0, 255));
                rightSprites.Add(new Sprite(graphics));
                rightSprites[i].position = new Vector2(100, 100 + i);
                rightSprites[i].color = new Color(World.Random.Next(0, 255), World.Random.Next(0, 255), World.Random.Next(0, 255));
            }

            var audioInputDevices = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);
            foreach (var device in audioInputDevices)
            {
                Debug.WriteLine(device.Name);
                if (device.Name.Contains("Stereo Mix"))
                {
                    audioInput = device;
                }
            }
            var audioOutputDevices = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);
            foreach (var device in audioOutputDevices)
            {
                Debug.WriteLine(device.Name);
                if (device.Name.Contains("ASUS"))
                {
                    audioOutput = device;
                }
            }

            AudioGraphSettings audioGraphSettings = new AudioGraphSettings(AudioRenderCategory.Media);
            audioGraphSettings.DesiredSamplesPerQuantum = 440;
            audioGraphSettings.DesiredRenderDeviceAudioProcessing = AudioProcessing.Default;
            audioGraphSettings.QuantumSizeSelectionMode = QuantumSizeSelectionMode.ClosestToDesired;
            audioGraphSettings.PrimaryRenderDevice = audioOutput;
            CreateAudioGraphResult audioGraphResult = await AudioGraph.CreateAsync(audioGraphSettings);
            if (audioGraphResult.Status != AudioGraphCreationStatus.Success)
            {
                Debug.WriteLine("AudioGraph creation failed! " + audioGraphResult.Status);
                return;
            }
            audioGraph = audioGraphResult.Graph;
            CreateAudioDeviceInputNodeResult inputNodeResult = await audioGraph.CreateDeviceInputNodeAsync(Windows.Media.Capture.MediaCategory.Media, audioGraph.EncodingProperties, audioInput);
            if (inputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                Debug.WriteLine("AudioDeviceInputNode creation failed! " + inputNodeResult.Status);
                return;
            }
            AudioDeviceInputNode inputNode = inputNodeResult.DeviceInputNode;
            CreateAudioDeviceOutputNodeResult outputNodeResult = await audioGraph.CreateDeviceOutputNodeAsync();
            if (outputNodeResult.Status != AudioDeviceNodeCreationStatus.Success)
            {
                Debug.WriteLine("AudioDeviceOutputNode creation failed! " + outputNodeResult.Status);
                return;
            }
            AudioDeviceOutputNode outputNode = outputNodeResult.DeviceOutputNode;
            frameOutputNode = audioGraph.CreateFrameOutputNode();
            inputNode.AddOutgoingConnection(frameOutputNode);
            //inputNode.AddOutgoingConnection(outputNode);
            audioGraph.QuantumProcessed += AudioGraph_QuantumProcessed;
            audioGraph.UnrecoverableErrorOccurred += AudioGraph_UnrecoverableErrorOccurred;
            audioGraph.Start();
            inputNode.Start();
            //outputNode.Start();
            frameOutputNode.Start();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            for (int i = 0; i < 220; i++)
            {
                leftSprites[i].Update();
                rightSprites[i].Update();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            for (int i = 0; i < 220; i++)
            {
                leftSprites[i].DrawRect(spriteBatch);
                rightSprites[i].DrawRect(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
