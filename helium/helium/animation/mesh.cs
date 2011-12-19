using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace helium.animation
{
    public class AnimationMesh
    {
        //////////////////////////////////////////////////////////////////////////
        // CONSTRUCTORS
        //////////////////////////////////////////////////////////////////////////

        //-----------------------------------------------------------------------

        public AnimationMesh(string name)
        { 
            this.name = name; 
            isDirty = true; 
        }

        //-----------------------------------------------------------------------

        //////////////////////////////////////////////////////////////////////////
        // PUBLIC METHODS
        //////////////////////////////////////////////////////////////////////////

        //-----------------------------------------------------------------------

        public void SetVertices(AnimationVertex[] vertexArray)
        { this.vertices = vertexArray; isDirty = true; }

        public void SetIndices(int[] indexArray)
        { this.indices = indexArray; isDirty = true; }

        //-----------------------------------------------------------------------

        public void Warm(GraphicsDevice device)
        {
            vertexbuffer = new VertexBuffer(device, typeof(AnimationVertex), vertices.Length, BufferUsage.None);
            indexbuffer = new IndexBuffer(device, typeof(int), indices.Length, BufferUsage.None);

            vertexbuffer.SetData<AnimationVertex>(vertices);
            indexbuffer.SetData<int>(indices);

            isDirty = false;
        }

        //-----------------------------------------------------------------------

        public void Draw(GraphicsDevice device)
        {
            if (indices.Length == 0)
                return;

            if (isDirty)
                Warm(device);

            device.SetVertexBuffer(vertexbuffer);
            device.Indices = indexbuffer;
            device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length / 3);
        }

        //-----------------------------------------------------------------------

        public string GetName()
        { return name; }

        //-----------------------------------------------------------------------

        public void SetTextureName(string name)
        { textureName = name; }

        public string GetTextureName()
        { return textureName; }

        public void SetTexture(Texture2D texture)
        { this.texture = texture; }

        public Texture2D GetTexture()
        { return texture; }

        //-----------------------------------------------------------------------

        //////////////////////////////////////////////////////////////////////////
        // PRIVATE METHODS
        //////////////////////////////////////////////////////////////////////////

        //-----------------------------------------------------------------------

        private string name;
        private bool isDirty;
        private VertexBuffer vertexbuffer;
        private IndexBuffer indexbuffer;
        private AnimationVertex[] vertices;
        private int[] indices;
        private string textureName;
        private Texture2D texture;
    }
}
