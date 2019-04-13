using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Common;
using Assets.Net;
using GameProtocol;
using GameProtocol.Model.Match;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Match
{
    public class UI_Match : MonoBehaviour
    {
        /// <summary>
        /// 时钟计数
        /// </summary>
        private int mTimeCount = 0;
        /// <summary>
        /// 时钟id
        /// </summary>
        private int mTimeID = -1;

        private Text mCountDown;
        private Text mMatchContent;
        private Text mGameType;
        private Button mCloseBtn;
        private Button mCancelBtn;

        void Awake()
        {
            GameApp.Instance.UI_Match = this;
            mCountDown = transform.Find("MatchPanel/CountNumber").GetComponent<Text>();
            mCloseBtn = transform.Find("MatchPanel/CloseBtn").GetComponent<Button>();
            mMatchContent = transform.Find("MatchPanel/MatchContent").GetComponent<Text>();
            mGameType = transform.Find("MatchPanel/GameType").GetComponent<Text>();
            mCancelBtn = transform.Find("MatchPanel/CancelBtn").GetComponent<Button>();
        }

        void Start()
        {
            AddOnClick();
            StartTimeCount();
        }

        void AddOnClick()
        {
            mCloseBtn.onClick.AddListener(() =>
            {
                this.Write(TypeProtocol.Match, MatchProtocol.LevelMatch_CReq, SeverConst.GameType.WinThree);
            });
            mCancelBtn.onClick.AddListener(() =>
            {
                this.Write(TypeProtocol.Match,MatchProtocol.LevelMatch_CReq,SeverConst.GameType.WinThree);
            });
        }

        /// <summary>
        /// 开始计时
        /// </summary>
        void StartTimeCount()
        {
            UpdateTimeAnim();
            //一秒钟后再次计时
            GameApp.Instance.TimeMngr.AddSchedule(() =>
            {
                mTimeCount++;
                StartTimeCount();
            }, 1000);
        }

        //更新UI
        void UpdateTimeAnim()
        {
            //将当前计时显示在UI上
            mCountDown.text = mTimeCount.ToString();
        }

        /// <summary>
        /// 刷新当前客户端房间信息
        /// </summary>
        public void UpdateRoomRoleInfo(MatchModel model)
        {
            mMatchContent.text = string.Format("正在匹配中...... {0}/{1}", model.Team.Count, model.MaxPlayer);
            mGameType.text = GameApp.Instance.GameConst.GameName[(int) model.Type];
        }

        public void StartMatch(ResponseStartMatchModel info)
        {
            mMatchContent.text = string.Format("正在匹配中...... {0}/{1}", info.PlayerCount, info.MaxPlayer);
            mGameType.text = GameApp.Instance.GameConst.GameName[(int)info.Type];
            mTimeCount = 0;
        }
    }

}

