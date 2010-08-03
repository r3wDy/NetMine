using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace XtremeSweeper
{
    enum eSocketSide
    {
        SERVER,
        CLIENT
    }

    class cGameClient
    {
        int m_PlayerID;
        bool m_ConnectedToServer;
        bool m_IsAck;
        int m_MissAckCounter;

        cPlayField m_ClientField;
        eSocketSide m_SockSide;
        Byte[] Buffer;

        System.Net.Sockets.Socket m_ClientSocket;

        public cGameClient()
        {
            Buffer = new byte[255];
            m_MissAckCounter = 0;
        }

        public int getPlayerID()
        {
            return m_PlayerID;
        }

        void onDataInput(IAsyncResult res)
        {
            cGameClient recieveSocket = (cGameClient)res.AsyncState;
            int recBytes = recieveSocket.getSocket().EndReceive(res);
            byte[] InputBuffer = recieveSocket.getBuffer();

            cBaseMessage IncomingMessage = new cBaseMessage(InputBuffer);
            processMessage(IncomingMessage);

            //wait again for data
            recieveSocket.getSocket().BeginReceive(getBuffer(), 0, 255, System.Net.Sockets.SocketFlags.None, onDataInput, this);
        }

        void onConnect(IAsyncResult res)
        {
            m_ClientSocket.EndConnect(res);
            setSocketSide(eSocketSide.CLIENT);
            m_ClientSocket.BeginReceive(getBuffer(), 0, 255, System.Net.Sockets.SocketFlags.None, onDataInput, this);

        }

        public void IncMissAck()
        {
            m_MissAckCounter++;
        }


        public bool getIsAck()
        {
            return m_IsAck;
        }

        public void setIsAck(bool i_Ack)
        {
            m_IsAck = i_Ack;

            if (m_IsAck)
                m_MissAckCounter = 0;
        }


        public bool getAckHigh()
        {
            if (m_MissAckCounter > 250)
                return true;
            return false;
        }


        public eGameState getGameState()
        {
            return eGameState.CLIENTLOBBY;
        }

        public void setGameState(eGameState i_NewState)
        {
        }

        bool processMessage(cBaseMessage i_cbm)
        {
            switch (i_cbm.getMsgType())
            {
                case NW_MSG_TYPE.WELCOME:
                    m_PlayerID = i_cbm.getPlayerID();
                    if(m_PlayerID != 0)
                        m_ConnectedToServer = true;
                    break;

                case NW_MSG_TYPE.PING:
                    //we got a ping, do nothing, hope sendData answers
                    break;
            }

            //ACK this one
            sendData(new cAckMessage(m_PlayerID));

            return true;
        }


        public void sendData(cBaseMessage cbm)
        {
            if (m_ClientSocket.Connected)
            {
                m_ClientSocket.Send(cbm.asArray());

                if (m_SockSide == eSocketSide.SERVER)
                { //Server sent this one
                    setIsAck(false); //wait for ack
                }
            }
        }


        public byte[] getBuffer()
        {
            return Buffer;
        }

        public bool isConnected()
        {
            return m_ConnectedToServer;
        }

        public void connect(string ip)
        {
            IPEndPoint IPE = new IPEndPoint(IPAddress.Parse(ip), 500);
            m_ClientSocket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
            m_ClientSocket.BeginConnect(IPE, onConnect, null);
        }

        public void setSocket(System.Net.Sockets.Socket i_Socket)
        {
            m_ClientSocket = i_Socket;
        }

        public System.Net.Sockets.Socket getSocket()
        {
            return m_ClientSocket;
        }


        public void setPlayerID(int i_newID)
        {
            m_PlayerID = i_newID;
        }

        public void setSocketSide(eSocketSide i_SSide)
        {
            m_SockSide = i_SSide;
        }

        public eSocketSide getSocketSide()
        {
           return m_SockSide;
        }

    }
}
