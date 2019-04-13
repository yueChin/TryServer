
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProtocol.Model.Fight;
using Server.Cache;
using ServerNetFrame;
using ServerNetFrame.Auto;

namespace Server.Logic.Fight
{
    public class FightRoom : IHandler
    {
        
        public List<int> TeamList = new List<int>();
        protected int RoomID = -1;
        protected  Dictionary<int,FightModel> FighterDict = new Dictionary<int, FightModel>();

        public void ClientClose(UserToken token, string error)
        {
            CacheProxy.Fight.CloseRoom();
        }

        public void MessageReceive(UserToken token, SocketModel message)
        {
            
        }

        void Leave()
        {

        }

        void StartGame()
        {

        }

        void EnterRoom()
        {

        }
    }
}
