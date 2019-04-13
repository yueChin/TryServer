using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using Assets.Model;
using UnityEngine;

namespace Assets.Manager
{
    
    public class TimeMngr:MonoBehaviour
    {
        public delegate void TaskEvent();
        private Dictionary<int,TimeTaskModel> mTaskDict = new Dictionary<int, TimeTaskModel>();
        private List<int> mRemoveList = new List<int>();
        private int mIndex;
        private void Awake()
        {
            GameApp.Instance.TimeMngr = this;
            //StartCoroutine(AddSchedele(2.0f, () => { Debug.LogError(2100); }));
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < mRemoveList.Count; i++)
            {
                mTaskDict.Remove(mRemoveList[i]);
            }
            List<int> taskId = new List<int>(mTaskDict.Keys);
            long time = DateTime.Now.Ticks;
            for (int i = 0; i < taskId.Count; i++)
            {
                if (mTaskDict[taskId[i]].Time < time)
                {
                    mRemoveList.Add(taskId[i]);
                    try
                    {
                        mTaskDict[taskId[i]].Run();;
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            }
        }

        public int AddSchedule(TaskEvent task, long time)
        {
            mIndex++;
            TimeTaskModel m = new TimeTaskModel(mIndex,time * 10000 + DateTime.Now.Ticks,task);
            mTaskDict.Add(mIndex,m);
            return mIndex;
        }

        public bool Remove(int id)
        {
            if (mRemoveList.Contains(id))
                return true;
            if (mTaskDict.ContainsKey(id))
            {
                mRemoveList.Add(id);
                return true;
            }

            return false;
        }

        IEnumerator AddSchedele(float time,TaskEvent task)
        {
            yield return new WaitForSeconds(2.0f);
            task();
        }
    }
}
