using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;



namespace XtremeSweeper
{

    class cPlayField
    {
        int xExtend;
        int yExtend;

        Texture2D m_SpriteFlagged;
        Texture2D m_SpriteMine;
        Texture2D m_SpriteClosed;
        Texture2D m_SpriteOpen;

        List<List<cOneField>> FieldGrid;

        public cPlayField()
        {

        }

        public void setupNewField(int i_xExtend, int i_yExtend,int i_Mines)
        {
            xExtend = i_xExtend;
            yExtend = i_yExtend;
            FieldGrid = new List<List<cOneField>>();

            for (int x = 0; x < xExtend; x++)
            {
                FieldGrid.Add(new List<cOneField>());
                for (int y = 0; y < yExtend; y++)
                {
                    cOneField tmpNewField = new cOneField(this, x, y);
                    tmpNewField.setGfx(m_SpriteClosed, m_SpriteMine, m_SpriteFlagged, null);
                    FieldGrid[x].Add(tmpNewField);
                }
            }

        }


        public void loadContents(ContentManager i_CManager)
        {
            m_SpriteFlagged = i_CManager.Load<Texture2D>("FieldFlagged");
            m_SpriteMine = i_CManager.Load<Texture2D>("FieldMine");
            m_SpriteClosed = i_CManager.Load<Texture2D>("FieldClosed");
            //m_SpriteOpen = i_CManager.Load<Texture2D>("FieldOpen");
        }


        public cOneField getFieldAt(int i_xPos, int i_yPos)
        {
            if (i_xPos < xExtend)
                if (i_yPos < yExtend)
                    return FieldGrid[i_xPos][i_yPos];

            throw new Exception("Fieldindex for getField out of Range");
        }

        public void setFielStateAt(int i_xPos, int i_yPos,eFieldState i_NewState)
        {
            getFieldAt(i_xPos,i_yPos).setState(i_NewState);
        }

        public eFieldState getFieldStateAt(int i_xPos, int i_yPos)
        {
            return getFieldAt(i_xPos, i_yPos).getState();
        }

        public void render(SpriteBatch i_SBatch)
        {
            foreach(List<cOneField> outerList in FieldGrid)
                foreach (cOneField cOF in outerList)
                {
                    cOF.Render(i_SBatch);
                }
        }


        public bool parseString(string i_FieldString)
        {

            for (int x = 0; x < xExtend; x++)
            {
                for (int y = 0; y < yExtend; y++)
                {
                    switch (i_FieldString[x * xExtend + y])
                    {
                        case '0':
                            setFielStateAt(x, y, eFieldState.CLOSED);
                            break;
                        case '1':
                            setFielStateAt(x, y, eFieldState.FLAG);
                            break;
                        case '2':
                            setFielStateAt(x, y, eFieldState.MINE);
                            break;
                        case '3':
                            setFielStateAt(x, y, eFieldState.OPEN);
                            break;
                        default:
                            return false;
                    }
                }
            }
            return true;
        }

        public string asString()
        {
            string FieldString = "";
            FieldString += xExtend.ToString() + '|' + yExtend.ToString()+'|';
            for (int x = 0; x < xExtend; x++)
            {
                for (int y = 0; y < yExtend; y++)
                {
                    switch (getFieldStateAt(x,y))
                    {
                        case eFieldState.CLOSED:
                            FieldString += '0';
                            break;
                        case eFieldState.FLAG:
                            FieldString += '1';
                            break;
                        case eFieldState.MINE:
                            FieldString += '2';
                            break;
                        case eFieldState.OPEN:
                            FieldString += '3';
                            break;
                    }
                }
            }
            return FieldString;
        }

        public int getXExtend()
        {
            return xExtend;
        }

        public int getYExtend()
        {
            return yExtend;
        }
        /*
        * ====INTERNER KRAMS
        */


        void placeMines(int i_MineCount)
        {
            Random rnd = new Random();
            int MineCountdown = i_MineCount;
            while (MineCountdown > 0)
            {
                int MinePosX,MinePosY;
                MinePosX = rnd.Next(xExtend);
                MinePosY = rnd.Next(yExtend);

                if(getFieldStateAt(MinePosX,MinePosY) != eFieldState.MINE)
                {
                    MineCountdown--;
                    setFielStateAt(MinePosX,MinePosY,eFieldState.MINE);
                }
            }
        }
    }
}
