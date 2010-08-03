using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;


namespace XtremeSweeper
{

    enum NW_MSG_TYPE
    {
        ACK = 0x01,
        WELCOME = 0x10,
        PING = 0x11,
        NEW_GAME = 0x20,
        PLAYFIELD = 0x21,
        CLICK = 0x22,
        INITGAME = 0x23
    }

    enum eGAME_TYPE
    {
        COOP = 0x01,
        TIMETRAIL = 0x02,
        SOLO = 0x03
    }

    class cGameServer
    {
        Dictionary<int,cGameClient> m_Clients;
        System.Net.Sockets.Socket m_ServerSocket;
        TimeSpan pingTimer;
        TimeSpan pingTimeOut;


        void onDataInput(IAsyncResult res)
        {
            cGameClient recieveSocket = (cGameClient)res.AsyncState;
            int recBytes = recieveSocket.getSocket().EndReceive(res);
            byte[] tmp = recieveSocket.getBuffer();

            cBaseMessage cbm = new cBaseMessage(tmp);
            processMessage(cbm);
            recieveSocket.getSocket().BeginReceive(recieveSocket.getBuffer(), 0, 255, System.Net.Sockets.SocketFlags.None, onDataInput, recieveSocket);
        }


        void processMessage(cBaseMessage i_cbm)
        {
            NW_MSG_TYPE mType = i_cbm.getMsgType();
            int pID = i_cbm.getPlayerID();
            
            switch (mType)
            {
                case NW_MSG_TYPE.ACK:
                    //We got an ACK from a client. Save this state
                    if (m_Clients.Keys.Contains(pID))
                        m_Clients[pID].setIsAck(true);
                    break;

                case NW_MSG_TYPE.INITGAME:
                    cInitGameMsg igMSG = new cInitGameMsg(i_cbm.getRawData());
                    break;
            }
        }


        public void tick(GameTime i_Time)
        {
            //Check for ack on last msg
            foreach (KeyValuePair<int,cGameClient> cgc in m_Clients)
            {
                if (!cgc.Value.getIsAck())
                    cgc.Value.IncMissAck();

                if (cgc.Value.getAckHigh())
                {
                    throw new Exception("Ack too high, cleint discon");
                }
            }

            pingTimer += i_Time.ElapsedGameTime;

            if (pingTimer > pingTimeOut)
            {
                pingTimer = pingTimer-pingTimer;
                //Time to send a ping to our clients
                foreach (KeyValuePair<int, cGameClient> cgc in m_Clients)
                {
                    if(cgc.Value.getIsAck())
                        cgc.Value.sendData(new cPingMessage(cgc.Value.getPlayerID()));
                }
            }
        }


        void onClientConnect(IAsyncResult res)
        {
            cGameClient justConnected = new cGameClient();
            Random rnd = new Random();
            int newPlayersID = 0;

            do
                newPlayersID = rnd.Next();
            while (m_Clients.Keys.Contains(newPlayersID));


            justConnected.setPlayerID(newPlayersID);
            justConnected.setSocket(m_ServerSocket.EndAccept(res));
            justConnected.setSocketSide(eSocketSide.SERVER);
            justConnected.getSocket().BeginReceive(justConnected.getBuffer(), 0, 255, System.Net.Sockets.SocketFlags.None, onDataInput, justConnected);
            m_Clients.Add(newPlayersID, justConnected);

            //Send the players id to the player
            cWlcMessage cbm = new cWlcMessage(newPlayersID);
            justConnected.sendData(cbm);

            //return to wait for new connection
            m_ServerSocket.BeginAccept(onClientConnect, null);
        }

        public bool isConnected()
        {
            return m_ServerSocket.Connected;
        }

        public void sendData(int i_ToPlayer)
        {
            if (!m_Clients.Keys.Contains(i_ToPlayer))
                return;

            cGameClient gClient = m_Clients[i_ToPlayer];
            byte[] data = new byte[3];
            data[0] = 133;

            if (isConnected())
                gClient.getSocket().Send(data);
        }

        public void waitForConnection()
        {
            IPEndPoint IPE = new IPEndPoint(0,500);
            m_ServerSocket.Bind(IPE);
            m_ServerSocket.Listen(255);
            m_ServerSocket.BeginAccept(onClientConnect, null);
        }

        public cGameServer()
        {
            m_Clients = new Dictionary<int, cGameClient>();
            m_ServerSocket = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            pingTimer = new TimeSpan(0, 0, 0);
            pingTimeOut = new TimeSpan(0, 0, 3); //3 sec
        }


        void setupClient(cGameClient i_Client)
        {

        }


    }



    class cBaseMessage
    {
        int m_PlayerID;
        NW_MSG_TYPE m_Type;
        List<string> m_SplitValues;
        byte[] m_rawMsgData;


        /*
         * ===INTERFACE
         * */

        public byte[] getRawData()
        {
            return m_rawMsgData;        
        }

        public virtual string asString()
        {
            return m_PlayerID.ToString() + '|' + ((int)m_Type).ToString() + '|';
        }

        public virtual void fromString()
        {
            m_PlayerID = nextValueAsInt();
            m_Type = (NW_MSG_TYPE)Enum.Parse(typeof(NW_MSG_TYPE), nextValueAsString(), true);
        }

        /*
         * ===PRIVATE STUFF
         * */

        public cBaseMessage()
        {
            m_PlayerID = 0;
        }

        public cBaseMessage(byte[] i_DataBuffer)
        {
            m_rawMsgData = i_DataBuffer;
            fromArray(i_DataBuffer);
        }

        public cBaseMessage(int i_PlayerID)
        {
            m_PlayerID = i_PlayerID;
        }

        public void setPleyerID(int i_PlayerID)
        {
            m_PlayerID = i_PlayerID;
        }

        public int getPlayerID()
        {
            return m_PlayerID;
        }

        public NW_MSG_TYPE getMsgType()
        {
            return m_Type;
        }

        public void setMsgType(NW_MSG_TYPE i_Type)
        {
            m_Type = i_Type;
        }

        string nextValue()
        {
            if (m_SplitValues.Count() == 0)
            {
                throw new Exception("No more field for nextValue");
            }

            string tmp =  m_SplitValues[0];
            m_SplitValues.RemoveAt(0);
            return tmp;
        }

        public int nextValueAsInt()
        {
            return Convert.ToInt32(nextValue());
        }

        public string nextValueAsString()
        {
            return nextValue();
        }

        public bool nextValueAsBool()
        {
            return (nextValue() == "True");
        }


        public byte[] asArray()
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetBytes(asString());
        }

        public void fromArray(byte[] i_InData)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            m_SplitValues = enc.GetString(i_InData).Split(new char[] { '|' }).ToList();
            fromString();
        }
    }

    class cAckMessage : cBaseMessage
    {

        public cAckMessage(int i_PlayerID)
        {
            setPleyerID(i_PlayerID);
            setMsgType(NW_MSG_TYPE.ACK);
        }

        public cAckMessage(byte[] i_DataBuffer)
            : base(i_DataBuffer)
        {
        }

        public override string asString()
        {
            return base.asString();
        }
    }

    class cWlcMessage : cBaseMessage
    {

        public cWlcMessage(int i_PlayerID)
        {
            setPleyerID(i_PlayerID);
            setMsgType(NW_MSG_TYPE.WELCOME);
        }

        public cWlcMessage(byte[] i_DataBuffer)
            : base(i_DataBuffer)
        {
        }
    }

    class cPingMessage : cBaseMessage
    {

        public cPingMessage(int i_PlayerID)
        {
            setPleyerID(i_PlayerID);
            setMsgType(NW_MSG_TYPE.PING);
        }

        public cPingMessage(byte[] i_DataBuffer)
            : base(i_DataBuffer)
        {
        }
    }

    class cClkMessage : cBaseMessage
    {
        int m_WhichButton;
        int m_XPos;
        int m_YPos;

        public cClkMessage(int i_PlayerID,int i_Button,int i_XPos,int i_YPos)
            :base(i_PlayerID)
        {
            setMsgType(NW_MSG_TYPE.CLICK);
            m_WhichButton = i_Button;
            m_XPos = i_XPos;
            m_YPos = i_YPos;
        }

        public cClkMessage(byte[] i_DataBuffer)
            : base(i_DataBuffer)
        {
        }

        public override string asString()
        {
            string basestr = base.asString();
            return basestr + m_WhichButton.ToString() + '|' + m_XPos.ToString() + '|' + m_YPos.ToString() + '|';
        }

        public override void fromString()
        {
            m_WhichButton = nextValueAsInt();
            m_XPos = nextValueAsInt();
            m_YPos = nextValueAsInt();
        }

    }

    class cFieldMessage : cBaseMessage
    {
        int m_XExt;
        int m_YExt;
        cPlayField m_TheField;

        public cFieldMessage(int i_PlayerID,cPlayField i_Field)
            : base(i_PlayerID)
        {
            setMsgType(NW_MSG_TYPE.PLAYFIELD);
            m_XExt = i_Field.getXExtend();
            m_YExt = i_Field.getYExtend();
            m_TheField = i_Field;
        }

        public cFieldMessage(byte[] i_DataBuffer)
            : base(i_DataBuffer)
        {
        }

        public override string asString()
        {
            string basestr = base.asString();
            return basestr + m_TheField.asString() + '|';
        }

        public override void fromString()
        {

        }

    }

    class cInitGameMsg : cBaseMessage
    {
        int m_XExt;
        int m_YExt;
        bool m_UseExtras;
        eGAME_TYPE m_GType;


        public cInitGameMsg(int i_PlayerID, int i_XExt,int i_YExt,bool i_UseExtras,eGAME_TYPE i_GameType)
            : base(i_PlayerID)
        {
            setMsgType(NW_MSG_TYPE.INITGAME);
            m_XExt = i_XExt;
            m_YExt = i_YExt;
            m_UseExtras = i_UseExtras;
            m_GType = i_GameType;
        }


        public cInitGameMsg(byte[] i_DataBuffer)
            : base(i_DataBuffer)
        {
            //fromString();
        }

        public override string asString()
        {
            string basestr = base.asString();
            return basestr + m_XExt.ToString() + '|' + m_YExt.ToString() + '|' + m_UseExtras.ToString() + '|' + ((int)m_GType).ToString() +'|';
        }

        public override void fromString()
        {
            base.fromString();
            m_XExt = nextValueAsInt();
            m_YExt = nextValueAsInt();
            m_UseExtras = nextValueAsBool();
            m_GType = (eGAME_TYPE)Enum.Parse(typeof(eGAME_TYPE), nextValueAsString(), true);
        }

    }

}
