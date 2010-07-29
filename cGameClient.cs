using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace XtremeSweeper
{
    enum PlayerState
    {
        PLAYING,
        OBSERVING,
        DEAD,
        MENU
    }

    enum SocketSide
    {
        SERVER,
        CLIENT
    }

    class cGameClient
    {
        int m_PlayerID;
        PlayerState m_PlayerState;    
        cPlayField m_ClientField;
        SocketSide m_SockSide;
        Byte[] Buffer;

        System.Net.Sockets.Socket m_ClientSocket;

        public cGameClient()
        {
            m_PlayerState = PlayerState.MENU;
            Buffer = new byte[255];
        }


        void onDataInput(IAsyncResult res)
        {
        }

        void onConnect(IAsyncResult res)
        {
            m_ClientSocket.EndConnect(res);
            setSocketSide(SocketSide.CLIENT);
            m_ClientSocket.BeginReceive(getBuffer(),0,255, System.Net.Sockets.SocketFlags.None, onDataInput, null);
        }


        public void sendData()
        {
            byte[] data = new byte[3];
            data[0] = 255;

            if (isConnected())
                m_ClientSocket.Send(data);
        }


        public byte[] getBuffer()
        {
            return Buffer;
        }

        public bool isConnected()
        {
            return m_ClientSocket.Connected;
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

        public void setSocketSide(SocketSide i_SSide)
        {
            m_SockSide = i_SSide;
        }

        public SocketSide getSocketSide()
        {
           return m_SockSide;
        }

    }
}
