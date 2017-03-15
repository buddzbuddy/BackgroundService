using BackgroundService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace BackgroundService.Processes
{
    public class TestProcess
    {
        int delayInSec = 5;
        string taskKey = "";
        public TestProcess(string taskKey)
        {
            this.taskKey = taskKey;

            var taskInfo = new TaskInfo();
            taskInfo.SetState("Process started");
            Caching.Set(taskKey, taskInfo);

            for (int i = 1; i < 100; i++)
            {
                Step(i.ToString());
            }
            /*Step1();
            Step2();
            Step3();
            Step4();*/
        }

        void Step(string no)
        {
            Thread.Sleep(delayInSec * 1000);
            var taskInfo = Caching.Get<TaskInfo>(taskKey);
            taskInfo.SetState("State " + no);
            Caching.Set(taskKey, taskInfo);
        }

        void Step1()
        {
            Thread.Sleep(delayInSec * 1000);
            var taskInfo = Caching.Get<TaskInfo>(taskKey);
            taskInfo.SetState("State 1");
            Caching.Set(taskKey, taskInfo);
        }

        void Step2()
        {
            Thread.Sleep(delayInSec * 1000);
            var taskInfo = Caching.Get<TaskInfo>(taskKey);
            taskInfo.SetState("State 2");
            Caching.Set(taskKey, taskInfo);
        }

        void Step3()
        {
            Thread.Sleep(delayInSec * 1000);
            var taskInfo = Caching.Get<TaskInfo>(taskKey);
            taskInfo.SetState("State 3");
            Caching.Set(taskKey, taskInfo);
        }

        void Step4()
        {
            Thread.Sleep(delayInSec * 1000);
            var taskInfo = Caching.Get<TaskInfo>(taskKey);
            taskInfo.SetState("State 4");
            Caching.Set(taskKey, taskInfo);
        }
    }
}