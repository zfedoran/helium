using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using helium.animation;

namespace helium.resources
{
    public static class ResourceLibrary
    {
        public static SpriteFont debugfont;
        public static AnimationModelContent dude;

        public static void Load(ContentManager content, GraphicsDevice device)
        {
            ModelProcessor modelProcessor = new ModelProcessor(content);

            debugfont = content.Load<SpriteFont>("fonts/arial");
            dude = modelProcessor.Load("models/dude");
        }

        public static void Unload()
        { }
    }
}
