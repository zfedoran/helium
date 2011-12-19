using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace helium.animation
{
    public struct AnimationVertex : IVertexType
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 texture;
        public Vector4 blendweights;
        public Vector4 blendindices;

        private static readonly VertexDeclaration vertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.BlendWeight, 0),
            new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.BlendIndices, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return vertexDeclaration; }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}, {5}, {6}", position, normal, texture, blendweights, blendindices);
        }
    }
}
