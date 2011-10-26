using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Visualizer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class VisualizerGame : Microsoft.Xna.Framework.Game
    {
        bool trap_mouse = false;
        bool seen_k = false;
        bool seen_f = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        SpherePrimitive sphere;
        SampleGrid grid;
        SampleGrid grid2;
        protected List<LabelledVector> lines = new List<LabelledVector>();

        public VisualizerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.Title = "Visualizer";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>(@"SpriteFont1");

            sphere = new SpherePrimitive(GraphicsDevice, 0.5f, 8);

            grid = new SampleGrid();
            grid.GridSize = 16;
            grid.GridScale = 1.0f;
            grid.LoadGraphicsContent(GraphicsDevice);
            grid2 = new SampleGrid();
            grid2.GridSize = 16;
            grid2.GridScale = 1.0f;
            grid2.GridColor = Color.BlueViolet;
            grid2.LoadGraphicsContent(GraphicsDevice);

            LabelledVector.Load(GraphicsDevice);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, -20), new Vector3(0, 0, 100), Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2,
                                                        GraphicsDevice.Viewport.AspectRatio,
                                                        1.0f,
                                                        100);

            grid.ViewMatrix = viewMatrix;
            grid.ProjectionMatrix = projection;
            grid2.ViewMatrix = viewMatrix;
            grid2.ProjectionMatrix = projection;
            /*
            if (cur_skeleton != null)
            {
                Vector3 gpos = getLoc(cur_skeleton.Joints[JointID.Spine]);
                grid.WorldMatrix = Matrix.CreateTranslation(gpos);
                grid2.WorldMatrix = Matrix.Multiply(Matrix.CreateRotationX((float)Math.PI / 2), grid.WorldMatrix);
            }*/
            grid.Draw();
            grid2.Draw();

            spriteBatch.Begin();
            foreach (LabelledVector l in lines)
            {
                l.Draw(GraphicsDevice, viewMatrix, projection, spriteBatch, spriteFont);
            }
            spriteBatch.End();

            lock (pointSets)
            {
                foreach (List<double[]> pset in pointSets.Values)
                {
                    foreach (double[] p in pset)
                    {
                        sphere.Draw(Matrix.CreateTranslation(getLoc(p)), viewMatrix, projection, Color.Red);
                    }
                }
            }
            /*
            try
            {
                if (cur_skeleton != null)
                {
                    if (cur_skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        foreach (Joint joint in cur_skeleton.Joints)
                        {
                            var position = ConvertRealWorldPoint(joint.Position);
                            sphere.Draw(Matrix.CreateTranslation(position), viewMatrix, projection, Color.Red);
                        }
                    }
                }
            }
            catch
            {

            }
            */
            // Reset the fill mode renderstate.
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            base.Draw(gameTime);
        }

        internal void removePoints(int pointsId)
        {
            lock (pointSets)
            {
                if (pointSets.ContainsKey(pointsId))
                    pointSets.Remove(pointsId);
            }
        }

        int curKey = 0;
        Dictionary<int, List<double[]>> pointSets = new Dictionary<int, List<double[]>>();

        internal int addPoints(List<double[]> points)
        {
            lock (pointSets)
            {
                curKey++;
                pointSets.Add(curKey, points);
            }
            return curKey;
        }

        Matrix viewMatrix;

        Vector3 cameraPosition = new Vector3(0, 1, -2);
        float leftrightRot = 0;
        float updownRot = -MathHelper.Pi / 10.0f;
        const float rotationSpeed = 0.3f;
        const float moveSpeed = 10.0f;

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();


            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            ProcessInput(timeDifference);

            base.Update(gameTime);
        }

        private void ProcessInput(float amount)
        {
            if (!IsActive)
                return;

            MouseState currentMouseState = Mouse.GetState();
            if (trap_mouse)
            {
                float xDifference = currentMouseState.X - GraphicsDevice.Viewport.Width / 2;
                float yDifference = currentMouseState.Y - GraphicsDevice.Viewport.Height / 2;
                leftrightRot -= rotationSpeed * xDifference * amount;
                updownRot += rotationSpeed * yDifference * amount;
                Mouse.SetPosition(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
                UpdateViewMatrix();
            }

            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.K))
            {
                if (!seen_k)
                {
                    seen_k = true;
                    trap_mouse = !trap_mouse;
                }
            }
            else if (keyState.IsKeyUp(Keys.K))
            {
                if (seen_k)
                {
                    seen_k = false;
                }
            }

            if (keyState.IsKeyDown(Keys.F) && !seen_f)
            {
                graphics.PreferredBackBufferWidth = 2048;
                graphics.PreferredBackBufferHeight = 1152;
                graphics.ToggleFullScreen();
                seen_f = true;
            }
            
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W) || currentMouseState.LeftButton.HasFlag(ButtonState.Pressed))
                moveVector -= new Vector3(0, 0, -1);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S) || currentMouseState.RightButton.HasFlag(ButtonState.Pressed))
                moveVector -= new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                moveVector -= new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                moveVector -= new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.Q))
                moveVector += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.Z))
                moveVector += new Vector3(0, -1, 0);
            AddToCameraPosition(moveVector * amount);
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            cameraPosition += moveSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);

            Vector3 cameraOriginalTarget = new Vector3(0, 0, 100);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            Vector3 cameraFinalTarget = cameraPosition + cameraRotatedTarget;

            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraFinalTarget, cameraOriginalUpVector);
        }

        public Vector3 getLoc(double[] v) { return Vector3.Multiply(new Vector3((float)v[0], (float)v[1], (float)v[2]), 10); }
    }
}
