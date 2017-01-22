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
using Microsoft.Xna.Framework.Input;
using Windows.UI.ViewManagement;
using Windows.UI.Input;

namespace MusicVis
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState previousKeyboardState;

        AudioGraph audioGraph;
        AudioFrameOutputNode frameOutputNode;

        DeviceInformation audioInput;
        DeviceInformation audioOutput;

        CircleManager circleManager;
        FlashManager flashManager;

        public static int WindowWidth;
        public static int WindowHeight;
        public static Rectangle WindowRectangle;

        SpriteFont debugFont;
        List<TextItem> fontList;
        List<AdjustableMax> maxList;

        List<RadialControllerMenuItem> menuItems;
        float controlValue = 0.5f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            menuItems = new List<RadialControllerMenuItem>();
            World.dial.RotationResolutionInDegrees = 1;
            World.dial.UseAutomaticHapticFeedback = true;
            RadialControllerMenuItem control = RadialControllerMenuItem.CreateFromKnownIcon("value", RadialControllerMenuKnownIcon.Scroll);
            menuItems.Add(control);
            foreach (var item in menuItems)
            {
                World.dial.Menu.Items.Add(item);
            }
            World.dialConfig.SetDefaultMenuItems(new List<RadialControllerSystemMenuItemKind>());
            World.dial.RotationChanged += Dial_RotationChanged;
        }

        private void Dial_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            if (args.RotationDeltaInDegrees > 0)
            {
                controlValue += 0.05f;
            }
            else if (args.RotationDeltaInDegrees < 0)
            {
                controlValue -= 0.05f;
            }
            controlValue = MathHelper.Clamp(controlValue, 0.5f, 1);
            if (controlValue == 0.5f || controlValue == 1)
            {
                World.dial.UseAutomaticHapticFeedback = false;
            }
            else
            {
                World.dial.UseAutomaticHapticFeedback = true;
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            fontList = new List<TextItem>();
            maxList = new List<AdjustableMax>(220);
            ApplicationView view = ApplicationView.GetForCurrentView();
            view.TitleBar.BackgroundColor = Windows.UI.Colors.Black;
            view.TitleBar.ButtonBackgroundColor = Windows.UI.Colors.Black;
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
                    maxList[j].Value = leftChannel[j];
                    fontList[j].Text = $"{j}: {maxList[j].MinValue} {leftChannel[j].ToString()} {maxList[j].CurrentMax}";
                    int num = 10000;
                    int count = (int)(num * leftChannel[j]);
                    count = MathHelper.Clamp(count, 0, 5);
                    for (int k = 0; k < count; k++)
                    {
                        int customWindowHeight = (int)(WindowHeight * 1);
                        int slot = j * (customWindowHeight / 220);
                        int inverseSlot = WindowHeight - slot;
                        //circleManager.Spawn(j, inverseSlot);
                        if (maxList[j].Value >= controlValue)
                        {
                            flashManager.Spawn(j, maxList[j].Value, inverseSlot);
                        }
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
            WindowRectangle = new Rectangle(0, 0, WindowWidth, WindowHeight);

            debugFont = Content.Load<SpriteFont>("DebugFont");
            circleManager = new CircleManager(Content.Load<Texture2D>("circle"));
            flashManager = new FlashManager(Content.Load<Texture2D>("flash"));

            var audioInputDevices = await DeviceInformation.FindAllAsync(DeviceClass.AudioCapture);
            foreach (var device in audioInputDevices)
            {
                Debug.WriteLine(device.Name);
                Debug.WriteLine(device.IsDefault);
                if (device.Name.Contains("Output"))
                {
                    audioInput = device;
                }
            }
            var audioOutputDevices = await DeviceInformation.FindAllAsync(DeviceClass.AudioRender);
            foreach (var device in audioOutputDevices)
            {
                Debug.WriteLine(device.Name);
                Debug.WriteLine(device.IsDefault);
                if (device.Name.Contains("Input"))
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

            for (int i = 0; i < 220; i++)
            {
                TextItem tmp = new TextItem(debugFont, "0");
                tmp.color = Color.Black;
                if (i == 0)
                {
                    tmp.position = new Vector2(10, 50);
                }
                else if (i == 55)
                {
                    tmp.position = new Vector2(500, 50);
                }
                else if (i == 110)
                {
                    tmp.position = new Vector2(1000, 50);
                }
                else if (i == 165)
                {
                    tmp.position = new Vector2(1500, 50);
                }
                else
                {
                    tmp.PositionBelow(fontList[i - 1], 0);
                }
                fontList.Add(tmp);
                maxList.Add(new AdjustableMax());
            }

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
            KeyboardState keyboardState = Keyboard.GetState();

            if (IsDownAndUp(Keys.Enter) || IsDownAndUp(Keys.Space))
            {
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            }
            
            if (IsDownAndUp(Keys.Escape))
            {
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
            }
            WindowWidth = graphics.GraphicsDevice.Viewport.Width;
            WindowHeight = graphics.GraphicsDevice.Viewport.Height;

            circleManager.Update(gameTime);
            flashManager.Update(gameTime);

            previousKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        bool IsDownAndUp(Keys key)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            return keyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            circleManager.Draw(spriteBatch);
            flashManager.Draw(spriteBatch);
            //foreach (var item in fontList)
            //{
            //    item.Draw(spriteBatch);
            //}
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
