using System;
using System.Collections.Generic;
using System.Text;

namespace GameProtocol.Model.Fight
{
    [Serializable]
    public class FightModel
    {
        public int ID = -1;
        public string NickName = string.Empty;
        public int Coin = 0;
        public string Icon = string.Empty;
    }
}
