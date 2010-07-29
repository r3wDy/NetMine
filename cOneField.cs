using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XtremeSweeper
{

    enum eFieldState
    {
        CLOSED,
        MINE,
        FLAG,
        OPEN
    }

    class cOneField
    {
        int m_xKoord;
        int m_yKoord;
        int m_renderPositionX;
        int m_renderPositionY;
        int m_renderWidth;
        int m_renderHeight;
        Rectangle renderTargetRect;

        char m_numberTag;

        cPlayField m_Parent;
        Texture2D m_SpriteFlagged;
        Texture2D m_SpriteMine;
        Texture2D m_SpriteGeneric;
        eFieldState m_State;

        /*
         * ====PUBLIC KRAMS
         */

        public cOneField(cPlayField xParent,int x_XKoord,int x_YKoord)
        {
            m_Parent = xParent;

            m_xKoord = x_XKoord;
            m_yKoord = x_YKoord;

            m_renderWidth = m_renderHeight = 15;
            m_renderPositionX = m_xKoord * m_renderWidth;
            m_renderPositionY = m_yKoord * m_renderHeight;
            renderTargetRect = new Rectangle(m_renderPositionX, m_renderPositionY, m_renderWidth, m_renderHeight);
            m_State = eFieldState.CLOSED;
        }

        public void setState(eFieldState i_NewState)
        {
            m_State = i_NewState;
        }

        public bool isMine()
        {
            return m_State == eFieldState.MINE;
        }

        public bool isOpen()
        {
            return m_State == eFieldState.OPEN;
        }

        public bool isFlagged()
        {
            return m_State == eFieldState.FLAG;
        }

        public bool isClosed()
        {
            return m_State == eFieldState.CLOSED;
        }

        public eFieldState getState()
        {
            return m_State;
        }

        public bool Render(SpriteBatch i_SBatch)
        {
                Texture2D spriteToUse = this.m_SpriteGeneric;

                if (this.isFlagged())
                    spriteToUse = this.m_SpriteFlagged;
                else if (this.isMine())
                    spriteToUse = this.m_SpriteMine;

                i_SBatch.Draw(spriteToUse, renderTargetRect, Color.Red);
                return true;
        }

        /*
        * ====INTERNER KRAMS
        */

        void generateNumberTag()
        {
            int sumMines = 0;

            for(int x=-1;x!=1;x++)
                for (int y = -1; y != 1; y++)
                {
                    cOneField gotField = m_Parent.getFieldAt(x, y);
                    if (gotField != null)
                    {
                        if (gotField.isMine())
                            sumMines++;
                    }                        
                }

            if (m_numberTag < 9)
                this.m_numberTag = sumMines.ToString().ToCharArray()[0];
            else
                throw new Exception("More than 8 mines around a field...");
        }

       
    }
}
