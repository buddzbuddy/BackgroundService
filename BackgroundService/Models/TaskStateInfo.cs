using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackgroundService.Models
{
    public class TaskInfo
    {
        public string CurrentState;
        public List<string> States = new List<string>();
        public void SetState(string description)
        {
            CurrentState = description;
            States.Add(States.Count + ". " + description);
        }
    }
}