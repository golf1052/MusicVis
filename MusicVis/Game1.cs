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

        CircleManager circleManager;

        public static int WindowWidth;
        public static int WindowHeight;

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
                    int count = (int)(num * leftChannel[j]);
                    count = MathHelper.Clamp(count, 0, 5);
                    for (int k = 0; k < count; k++)
                    {
                        int customWindowHeight = (int)(WindowHeight * 1);
                        int slot = j * (customWindowHeight / 220);
                        int inverseSlot = WindowHeight - slot;
                        circleManager.Spawn(j, inverseSlot);
                    }
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

            WindowWidth = graphics.GraphicsDevice.Viewport.Width;
            WindowHeight = graphics.GraphicsDevice.Viewport.Height;

            circleManager = new CircleManager(Content.Load<Texture2D>("circle"));

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
            WindowWidth = graphics.GraphicsDevice.Viewport.Width;
            WindowHeight = graphics.GraphicsDevice.Viewport.Height;

            circleManager.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            circleManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
