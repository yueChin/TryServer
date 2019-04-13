using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using ClientNetFrame;
using GameProtocol;
using GameProtocol.Model.Login;
using GameProtocol.Model.Match;
using UI.Match;
using UnityEngine;

namespace Assets.Net
{
    public class MatchHandler : MonoBehaviour, IHandler
    {
        public void MessageReceive(SocketModel model)
        {
            switch (model.command)
            {
                case MatchProtocol.LevelMatch_SRes:
                {
                    switch (model.GetMessage<int>())
                    {
                        case 0:
                        case -2:
                            GameApp.Instance.GameLevelMngr.CloseSystemUI(GameResource.SystemUIType.MatchPanel);
                            break;
                        case -1:
                            GameApp.Instance.CommonHintDlg.OpenHint("游戏即将开始");
                            break;
                        }
                    
                }
                    break;
                    ;
                case MatchProtocol.MatchInfo_BRQ:
                {
                    MatchModel m = model.GetMessage<MatchModel>();
                    if (GameApp.Instance.UI_Match != null)
                        GameApp.Instance.UI_Match.UpdateRoomRoleInfo(m);
                }
                    break;
                case MatchProtocol.MacchInfoFinsh_BRQ:
                {
                    //TODO: LoadBattle
                    GameApp.Instance.GameLevelMngr.LoadScene(GameResource.SceneName.Main);
                }
                    break;
                case MatchProtocol.StartMatch_SRes:
                {
                    ResponseStartMatchModel m = model.GetMessage<ResponseStartMatchModel>();
                    switch (m.Status)
                    {
                        case 0:
                            GameApp.Instance.GameLevelMngr.LoadSystemUI(GameResource.SystemUIType.MatchPanel, () =>
                            {
                                GameObject go;
                                if (!GameApp.Instance.GameLevelMngr.SystemUIGODict.TryGetValue(
                                    GameResource.SystemUIType.MatchPanel, out go))
                                {
                                    if (!go.GetComponent<UI_Match>())
                                        go.AddComponent<UI_Match>();
                                    go.GetComponent<UI_Match>().StartMatch(m);
                                }
                            });
                            break;
                        case -1:
                            GameApp.Instance.CommonHintDlg.OpenHint("当前余额不足");
                            break;
                        case -2:
                            GameApp.Instance.CommonHintDlg.OpenHint("当前已在匹配队列中");
                            break;
                            ;
                    }
                }
                    break;
            }
        }
    }
}
