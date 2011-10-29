#region File Description
//-----------------------------------------------------------------------------
// SampleGrid.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
#endregion

namespace KinectViewer
{
    public class LabelledVector {
        public Vector3 from, to;
        public Color color;
        public String text;

        protected static BasicEffect effect;
        protected static VertexPositionColor[] vertices;

        public LabelledVector(Vector3 f, Vector3 t, Color c, String txt) {
            from = f; to = t; color = c; text = txt;
        }

        public static void Load(GraphicsDevice device) {
            effect = new BasicEffect(device);
            vertices = new VertexPositionColor[6];
        }
        
        public void Draw(GraphicsDevice device, Matrix view, Matrix projection, SpriteBatch batch, SpriteFont font) {
            vertices[0] = new VertexPositionColor(from, color);
            vertices[1] = new VertexPositionColor(to,   color);

            Vector3 delt = Vector3.Multiply(Vector3.Subtract(to, from), 0.1f);
            
            Vector3 perp = Vector3.Cross(Vector3.Forward, delt);
            Vector3 arrowBase = Vector3.Subtract(to, delt);

            vertices[2] = new VertexPositionColor(Vector3.Add(arrowBase, perp), color);
            vertices[3] = new VertexPositionColor(to, color);
            vertices[4] = new VertexPositionColor(Vector3.Subtract(arrowBase, perp), color);
            vertices[5] = new VertexPositionColor(to, color);
            
            effect.World = Matrix.Identity;
            effect.View = view;
            effect.Projection = projection;
            effect.VertexColorEnabled = true;
            effect.LightingEnabled = false;

            for (int i = 0; i < effect.CurrentTechnique.Passes.Count; ++i)
            {
                effect.CurrentTechnique.Passes[i].Apply();
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, vertices, 0, 3);
            }

            Vector3 txtPos = device.Viewport.Project(Vector3.Lerp(from, to, 0.5f), projection, view, Matrix.Identity);
            Console.WriteLine(txtPos.ToString());
            batch.DrawString(font, text, new Vector2(txtPos.X + 20, txtPos.Y - 10), color);
              
        }
    }
}
