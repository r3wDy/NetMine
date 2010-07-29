using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace XtremeSweeper
{
    class cGameServer
    {
        Dictionary<int,cGameClient> m_Clients;
        System.Net.Sockets.Socket m_ServerSocket;

        void onDataInput(IAsyncResult res)
        {
            cGameClient recieveSocket = (cGameClient)res.AsyncState;
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
            justConnected.setSocketSide(SocketSide.SERVER);
            justConnected.getSocket().BeginReceive(justConnected.getBuffer(), 0, 255, System.Net.Sockets.SocketFlags.None, onDataInput, justConnected);
            m_Clients.Add(newPlayersID, justConnected);

            //return to wait for new connection
            m_ServerSocket.BeginAccept(onClientConnect, null);
        }

        void sendData()
        {
            byte[] data = new byte[3];
            data[0] = 255;

            if (isConnected())
                m_ClientSocket.Send(data);
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
        }

        cPlayField generateField(int i_xExtend, int i_yExtend)
        {
            cPlayField newField = new cPlayField(i_xExtend, i_yExtend);
            return newField;
        }

        void setupClient(cGameClient i_Client)
        {

        }


    }

    enum NW_MSGS
    {
        WELCOME = 0x10,
        BYE = 0x11,
        ERROR = 0x20,
        CHATMSG = 0x30,
        ACK = 0x40,
        ACTION_CLICK = 0x50,
        GAMEDATA_FIELD = 0x60,

    }

    struct NW_MSG_BASE
    {
        NW_MSGS MSGID;
        int PlayerID;
    };

    struct NW_MSGS_ACTION_CLICK
    {
        NW_MSG_BASE Header;
        byte Button;
        byte XPos;
        byte YPos;
    }

    struct NW_MSG_CHATMSG
    {
        NW_MSG_BASE Header;
    }
}
