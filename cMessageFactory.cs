using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace XtremeSweeper
{
    enum NW_MSGS
    {
        WELCOME = 0x10,
        BYE = 0x11,
        ERROR = 0x20,
        CHATMSG = 0x30,
        ACK = 0x40,
        ACTION_CLICK = 0x50,
        GAMEDATA_FIELD = 0x60
    }

    [Serializable]
    class cBaseMessage
    {
        NW_MSGS MsgID;
        int PlayerID;

        public void setTargetPlayerID(int i_targetPlayerID)
        {
            PlayerID = i_targetPlayerID;
        }

        public void setMsgID(NW_MSGS i_ID)
        {
            MsgID = i_ID;
        }

        public NW_MSGS getMsgID()
        {
            return MsgID;
        }

        public int getPlayerID()
        {
            return PlayerID;
        }

        public virtual byte[] toByteArray()
        {
            MemoryStream Buffer = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(Buffer, this);
            return Buffer.ToArray();
        }

        public void FromByteArray(byte[] i_Buffer)
        {
            MemoryStream Buffer = new MemoryStream();
            Buffer.Read(i_Buffer, 0, i_Buffer.Length);
            BinaryFormatter bf = new BinaryFormatter();
            cBaseMessage tmp = (cBaseMessage)bf.Deserialize(Buffer);

            setMsgID(tmp.getMsgID());
            setTargetPlayerID(tmp.getPlayerID());
        }
    }

    [Serializable]
    class cWelcomeMsg : cBaseMessage
    {
        public cWelcomeMsg(int i_PlayerID)
        {
            setTargetPlayerID(i_PlayerID);
            setMsgID(NW_MSGS.WELCOME);
        }

        public void FromByteArray(byte[] i_Buffer)
        {
            MemoryStream Buffer = new MemoryStream();
            Buffer.Read(i_Buffer,0,i_Buffer.Length);
            BinaryFormatter bf = new BinaryFormatter();
            cWelcomeMsg tmp = (cWelcomeMsg)bf.Deserialize(Buffer);   
        }
    }

}
