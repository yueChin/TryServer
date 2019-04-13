using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Manager;
using Assets.UI.Login;
using Assets.UI.Main;
using Assets.Util;
using UI.Match;

namespace Assets.Common
{
    public class GameApp
    {
        private static GameApp mInstance;

        public static GameApp Instance
        {
            get { return mInstance == null ? mInstance = new GameApp() : mInstance; }
        }

        public NetMsgUtil NetMsgUtil;
        public DebugMngr DebugMngr;
        public TimeMngr TimeMngr;
        public ResourceMngr ResourceMngr;
        public MusicMngr MusicMngr;
        public LoadMgr LoadMgr;
        public GameLevelMngr GameLevelMngr;
        public AliveMgr AliveMgr;

        public UI_Login UI_Login;
        public UI_Main UI_Main;
        public UI_Match UI_Match;
        public CommonHintDlg CommonHintDlg;
        public GameConst GameConst = new GameConst();
    }
}
