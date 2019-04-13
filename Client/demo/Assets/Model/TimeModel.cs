using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Manager;

namespace Assets.Model
{
    public class TimeTaskModel
    {       
        public int Id;
        public long Time;
        public TimeMngr.TaskEvent Event;

        public TimeTaskModel()
        {
        }

        public TimeTaskModel(int id,long time,TimeMngr.TaskEvent eEvent)
        {
            this.Id = id;
            this.Time = time;
            this.Event = eEvent;
        }

        public void Run()
        {
            Event();
        }
    }
}
