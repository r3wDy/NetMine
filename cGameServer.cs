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
            int recBytes = recieveSocket.getSocket().EndReceive(res);
            byte[] tmp = recieveSocket.getBuffer();

            recieveSocket.getSocket().BeginReceive(recieveSocket.getBuffer(), 0, 255, System.Net.Sockets.SocketFlags.None, onDataInput, null);
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

            //Send the players id to the player
            justConnected.sendData(new cWelcomeMsg(newPlayersID).toByteArray());
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

}
