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
    delegate eGameState MenuCliclCallback();

    class cMenuPoint
    {
        bool m_Heighlight;
        bool m_Enabled;
        string m_Text;
        SpriteFont m_UsedFont;
        int m_yPos, m_xPos;
        MenuCliclCallback m_Callback;

        public cMenuPoint(string i_Text, int i_xpos, int i_ypos, bool i_Highl, bool i_enabl, SpriteFont i_Font, MenuCliclCallback i_Callback)
        {
            m_Enabled = i_enabl;
            m_Heighlight = i_Highl;
            m_UsedFont = i_Font;
            m_Text = i_Text;
            m_xPos = i_xpos;
            m_yPos = i_ypos;
            m_Callback = i_Callback;

        }

        public void setFont(SpriteFont i_SFont)
        {
            m_UsedFont = i_SFont;
        }

        public void draw( SpriteBatch i_SBatch)
        {
            Color drawColor = Color.Gray;

            if (m_Heighlight)
                drawColor = Color.White;

            i_SBatch.DrawString(m_UsedFont, m_Text, new Vector2(m_xPos, m_yPos), drawColor);
        }

        public eGameState InvokeCallback()
        {
            return m_Callback.Invoke();
        }

        public bool update(int i_CursorX, int i_CursorY)
        {
            if (i_CursorX >= m_xPos)
                if (i_CursorX <= m_xPos + 150)
                    if (i_CursorY >= m_yPos)
                        if (i_CursorY <= m_yPos + 20)
                        {
                            m_Heighlight = true;
                            return true;
                        }

            m_Heighlight = false;
            return false;
        }
    }

    class cMainMenu
    {
        Texture2D m_MenuBackground;
        Texture2D m_MouseCursor;
        Texture2D m_Button;
        Texture2D m_Logo;
        SpriteFont m_Font;

        cMenuPoint[] m_MenuPoints;
        private MouseState m_MouseState;

        public cMainMenu()
        {
        }


        eGameState onNewGame()
        {
            return eGameState.ENTER_SERVERLOBBY;
        }

        eGameState onJoinGame()
        {
            return eGameState.ENTER_CLIENTLOBBY;
        }

        eGameState onQuit()
        {
            return eGameState.QUITTING;
        }


        public void loadContent(ContentManager i_Content)
        {
            m_Font = i_Content.Load<SpriteFont>("MenuFont");

            m_MenuPoints = new cMenuPoint[3];

            int actYPos = 120;
            int ItemHeight = 20;

            m_MenuPoints[0] = new cMenuPoint("New Game",350,actYPos, false, false, m_Font, onNewGame);
            actYPos += ItemHeight;
            m_MenuPoints[1] = new cMenuPoint("Join Game",350,actYPos, false, false, m_Font, onJoinGame);
            actYPos += ItemHeight;
            m_MenuPoints[2] = new cMenuPoint("Quit Game",350,actYPos, false, false, m_Font, onQuit);
            //m_MenuBackground = i_Content.Load<Texture2D>("MainBG");
            // m_MouseCursor = i_Content.Load<Texture2D>("MainCursor");
            // m_Button = i_Content.Load<Texture2D>("MainButton");
            //   m_Logo = i_Content.Load<Texture2D>("Logo");
        }

        public eGameState update()
        {
            m_MouseState = Mouse.GetState();

            foreach (cMenuPoint tmpMP in m_MenuPoints)
            {
                if (tmpMP.update(m_MouseState.X, m_MouseState.Y))
                {
                    if (m_MouseState.LeftButton == ButtonState.Pressed)
                        return tmpMP.InvokeCallback();
                }
            }

            return eGameState.MENU;
        }

        public void renderMainMenu(SpriteBatch i_SBatch)
        {
            //int i_LogoLeftOffset = (i_SBatch.GraphicsDevice.DisplayMode.Width / 2) - (m_Logo.Width / 2);
            //int i_MenuLeftOffset = (i_SBatch.GraphicsDevice.DisplayMode.Width / 2) - (m_MenuBackground.Width / 2);
            //int i_MenuTopOffset = (i_SBatch.GraphicsDevice.DisplayMode.Height / 2) - (m_MenuBackground.Height / 2); 
            //Rectangle menuRec = new Rectangle(i_MenuLeftOffset,i_MenuTopOffset,m_MenuBackground.Width,m_MenuBackground.Height);
            //Rectangle logoRect = new Rectangle(i_LogoLeftOffset, 10, m_Logo.Width, m_Logo.Height);

            //Draw Logo
          //  i_SBatch.Draw(m_Logo, logoRect, Color.White);
            //Draw background
         //   i_SBatch.Draw(m_MenuBackground, menuRec, Color.White);
            //Draw Texts
            foreach (cMenuPoint tmpMP in m_MenuPoints)
            {
                tmpMP.draw(i_SBatch);
            }
        }
    }
}
