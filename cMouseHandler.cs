using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XtremeSweeper
{
    class cMouseHandler
    {
        private Vector2 pos;
        private Texture2D tex;
        private MouseState mouseState;
        //We create variables

        public cMouseHandler(Vector2 pos)
        {
            this.pos = pos; //Inital pos (0,0)
            tex = null;
        }

        public void setCursor(string i_ContentName, ContentManager i_CManager)
        {
            if (tex != null)
                tex.Dispose();

            tex = i_CManager.Load<Texture2D>(i_ContentName);
        }
        //On Update we will call this function
        public void Update()
        {
            mouseState = Mouse.GetState(); //Needed to find the most current mouse states.
            this.pos.X = mouseState.X; //Change x pos to mouseX
            this.pos.Y = mouseState.Y; //Change y pos to mouseY
        }
        //Drawing function to be called in the main Draw function.
        public void Draw(SpriteBatch batch) //SpriteBatch to use.
        {
            if (tex == null)
                return;

            batch.Draw(this.tex, this.pos, Color.White); //Draw it using the batch.
        }
    }
}