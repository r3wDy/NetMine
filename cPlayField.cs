using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace XtremeSweeper
{

    class cPlayField
    {
        int xExtend;
        int yExtend;

        List<List<cOneField>> FieldGrid;

        public cPlayField(int i_xExtend, int i_yExtend)
        {
            xExtend = i_xExtend;
            yExtend = i_yExtend;
            FieldGrid = new List<List<cOneField>>();

            for(int x=0;x<xExtend;x++)
            {
                FieldGrid.Add(new List<cOneField>());
                for(int y=0;y<yExtend;y++)
                    FieldGrid[x].Add(new cOneField(this,x,y));
            }

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
