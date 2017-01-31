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
        const string CirclesText = "Circles";
        const string FlashesText = "Flashes";
        const string BigCircleText = "Big Circle";
        const string BigFlashText = "Big Flash";
        const string ScreenFlashText = "Screen Flash";

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState previousKeyboardState;
        VirtualResolutionRenderer virtualResolutionRenderer;

        AudioGraph audioGraph;
        AudioFrameOutputNode frameOutputNode;

        DeviceInformation audioInput;
        DeviceInformation audioOutput;

        CircleManager circleManager;
        FlashManager flashManager;
        BigCircleManager bigCircleManager;
        BigFlash bigFlash;
        ScreenFlashManager screenFlashManager;

        Timer silenceTimer;
        Timer balanceTimer;

        public static int WindowWidth;
        public static int WindowHeight;
        public static Rectangle WindowRectangle;

        SpriteFont debugFont;
        List<AdjustableMax> maxListLeft;
        List<AdjustableMax> maxListRight;
        AdjustableMax averageLowLeft;
        AdjustableMax averageLowRight;

        List<RadialControllerMenuItem> menuItems;
        float controlValue = 0.5f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            menuItems = new List<RadialControllerMenuItem>();
            RadialControllerMenuItem circleMenuItem = RadialControllerMenuItem.CreateFromKnownIcon(CirclesText, RadialControllerMenuKnownIcon.InkColor);
            RadialControllerMenuItem flashMenuItem = RadialControllerMenuItem.CreateFromKnownIcon(FlashesText, RadialControllerMenuKnownIcon.InkColor);
            RadialControllerMenuItem bigCircleMenuItem = RadialControllerMenuItem.CreateFromKnownIcon(BigCircleText, RadialControllerMenuKnownIcon.InkColor);
            RadialControllerMenuItem bigFlashMenuItem = RadialControllerMenuItem.CreateFromKnownIcon(BigFlashText, RadialControllerMenuKnownIcon.InkColor);
            RadialControllerMenuItem screenFlashMenuItem = RadialControllerMenuItem.CreateFromKnownIcon(ScreenFlashText, RadialControllerMenuKnownIcon.InkColor);
            menuItems.Add(circleMenuItem);
            menuItems.Add(flashMenuItem);
            menuItems.Add(bigCircleMenuItem);
            menuItems.Add(bigFlashMenuItem);
            menuItems.Add(screenFlashMenuItem);
            foreach (var item in menuItems)
            {
                World.dial.Menu.Items.Add(item);
            }
            World.dialConfig.SetDefaultMenuItems(new List<RadialControllerSystemMenuItemKind>());
            World.dial.RotationChanged += Dial_RotationChanged;
            World.dial.ButtonClicked += Dial_ButtonClicked;
        }

        private void Dial_ButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            var selected = World.dial.Menu.GetSelectedMenuItem();
            if (selected.DisplayText == CirclesText)
            {
                circleManager.On = !circleManager.On;
            }
            else if (selected.DisplayText == FlashesText)
            {
                flashManager.On = !flashManager.On;
            }
            else if (selected.DisplayText == BigCircleText)
            {
                bigCircleManager.On = !bigCircleManager.On;
            }
            else if (selected.DisplayText == BigFlashText)
            {
                bigFlash.On = !bigFlash.On;
            }
            else if (selected.DisplayText == ScreenFlashText)
            {
                screenFlashManager.On = !screenFlashManager.On;
            }
        }

        private void Dial_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            //if (args.RotationDeltaInDegrees > 0.5f)
            //{
            //    controlValue += 0.01f;
            //}
            //else if (args.RotationDeltaInDegrees < 1)
            //{
            //    controlValue -= 0.01f;
            //}
            //controlValue = MathHelper.Clamp(controlValue, 0.5f, 1);
            //if (controlValue == 0.5f || controlValue == 1)
            //{
            //    World.dial.UseAutomaticHapticFeedback = false;
            //}
            //else
            //{
            //    World.dial.UseAutomaticHapticFeedback = true;
            //}
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            maxListLeft = new List<AdjustableMax>(220);
            maxListRight = new List<AdjustableMax>(220);
            averageLowLeft = new AdjustableMax();
            averageLowRight = new AdjustableMax();
            ApplicationView view = ApplicationView.GetForCurrentView();
            view.TitleBar.BackgroundColor = Windows.UI.Colors.Black;
            view.TitleBar.ButtonBackgroundColor = Windows.UI.Colors.Black;
            silenceTimer = new Timer(TimeSpan.FromSeconds(5), Reset, 0);
            balanceTimer = new Timer(TimeSpan.FromMinutes(2), Reset, 0.1f);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override async void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            virtualResolutionRenderer = new VirtualResolutionRenderer(graphics, new Size(3000, 2000));
            WindowWidth = (int)virtualResolutionRenderer.VirtualResolution.Width;
            WindowHeight = (int)virtualResolutionRenderer.VirtualResolution.Height;
            WindowRectangle = new Rectangle(0, 0, WindowWidth, WindowHeight);

            debugFont = Content.Load<SpriteFont>("DebugFont");
            List<Texture2D> textures = new List<Texture2D>(new[]
            {
                //Content.Load<Texture2D>("circle"),
                //Content.Load<Texture2D>("square"),
                //Content.Load<Texture2D>("triangle"),
                //Content.Load<Texture2D>("star"),
                Content.Load<Texture2D>("heart")
            });
            circleManager = new CircleManager(textures);
            flashManager = new FlashManager(Content.Load<Texture2D>("flash"));
            bigCircleManager = new BigCircleManager(Content.Load<Texture2D>("circle_big"));
            bigFlash = new BigFlash(Content.Load<Texture2D>("flash_big"));
            screenFlashManager = new ScreenFlashManager(graphics);

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
                maxListLeft.Add(new AdjustableMax());
                maxListRight.Add(new AdjustableMax());
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
                    maxListLeft[j].Value = leftChannel[j];
                    maxListRight[j].Value = rightChannel[j];

                    int customWindowHeight = (int)(WindowHeight * 1);
                    int slot = j * (customWindowHeight / 220);
                    int inverseSlot = WindowHeight - slot;

                    if (circleManager.On)
                    {
                        if (maxListLeft[j].Value >= 0.25f)
                        {
                            circleManager.Spawn(maxListLeft[j].Value, j, inverseSlot, World.Side.Left);
                        }
                        if (maxListRight[j].Value >= 0.25f)
                        {
                            circleManager.Spawn(maxListRight[j].Value, j, inverseSlot, World.Side.Right);
                        }
                    }

                    if (flashManager.On)
                    {
                        if (maxListLeft[j].Value >= controlValue)
                        {
                            flashManager.Spawn(j, maxListLeft[j].Value, inverseSlot, World.Side.Left);
                        }
                        if (maxListRight[j].Value >= controlValue)
                        {
                            flashManager.Spawn(j, maxListRight[j].Value, inverseSlot, World.Side.Right);
                        }
                    }
                }

                averageLowLeft.Value = HelperMethods.Average(leftChannel, 0, 16);
                averageLowRight.Value = HelperMethods.Average(rightChannel, 0, 16);
                if (CheckAverageValue(0.01f))
                {
                    silenceTimer.Reset();
                }

                if (bigFlash.On)
                {
                    if (CheckAverageValue(0.70f))
                    {
                        bigFlash.Pump(GetAverageValue(averageLowLeft.Value, averageLowRight.Value));
                    }
                }
                if (bigCircleManager.On)
                {
                    if (CheckAverageValue(0.85f))
                    {
                        bigCircleManager.Spawn();
                    }
                }
                if (screenFlashManager.On)
                {
                    if (CheckAverageValue(0.7f))
                    {
                        screenFlashManager.Flash(GetAverageValue(averageLowLeft.Value, averageLowRight.Value));
                    }
                }
            }
        }

        private bool CheckAverageValue(float value)
        {
            return averageLowLeft.Value >= value && averageLowRight.Value >= value;
        }

        private float GetAverageValue(float leftValue, float rightValue)
        {
            return (leftValue + rightValue) / 2f;
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

            silenceTimer.Update(gameTime);
            balanceTimer.Update(gameTime);
            circleManager.Update(gameTime);
            flashManager.Update(gameTime);
            bigCircleManager.Update(gameTime);
            bigFlash.Update(gameTime);
            screenFlashManager.Update(gameTime);

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

            virtualResolutionRenderer.BeginDraw();
            spriteBatch.Begin(SpriteSortMode.Deferred,
                null, null, null, null, null,
                virtualResolutionRenderer.GetTransformationMatrix());
            circleManager.Draw(spriteBatch);
            flashManager.Draw(spriteBatch);
            bigCircleManager.Draw(spriteBatch);
            bigFlash.Draw(spriteBatch);
            screenFlashManager.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void Reset()
        {
            Reset(0);
        }

        private void Reset(float percent)
        {
            for (int i = 0; i < 220; i++)
            {
                maxListLeft[i].Reset(percent);
                maxListRight[i].Reset(percent);
            }
        }
    }
}
