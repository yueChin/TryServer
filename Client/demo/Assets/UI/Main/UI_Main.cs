using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using Assets.Net;
using GameProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Main
{
    public class UI_Main:MonoBehaviour
    {
        private Image mIcon;
        private Text mNickName;
        private Text mCoin;
        private Text mDiamond;
        private Text mID;
        private Button mWinThreeBtn;

        void Awake()
        {
            GameApp.Instance.UI_Main = this;
            mIcon = transform.Find("Panel/Icon").GetComponent<Image>();
            mNickName = transform.Find("Panel/Icon/NickName").GetComponent<Text>();
            mCoin = transform.Find("Panel/Coin/Coin").GetComponent<Text>();
            mDiamond = transform.Find("Panel/Diamond/Diamond").GetComponent<Text>();
            mID = transform.Find("Panel/Icon/UID").GetComponent<Text>();
            mWinThreeBtn = transform.Find("Panel/WinThreeBtn").GetComponent<Button>();
            GameSession.Instance.UserInfoChangeHandler += UpdateDate;
        }

        void Start()
        {
            UpdateDate();
        }

        void OnDestroy()
        {
            GameSession.Instance.UserInfoChangeHandler -= UpdateDate;
        }

        void AddOnClick()
        {
            mWinThreeBtn.onClick.AddListener(() =>
            {
                this.Write(TypeProtocol.Match,MatchProtocol.LevelMatch_CReq,SeverConst.GameType.WinThree);
            });
        }

        void UpdateDate()
        {
            if (mIcon == null || GameSession.Instance.UserModel == null)
                return;
            mNickName.text = GameSession.Instance.UserModel.NickName;
            mID.text = "ID" + GameSession.Instance.UserModel.Id + 10000;
            mCoin.text = GameSession.Instance.UserModel.Coin.ToString();
            mDiamond.text = GameSession.Instance.UserModel.Diamond.ToString();
        }
    }
}
