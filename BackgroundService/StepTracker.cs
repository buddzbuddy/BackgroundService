using Intersoft.CISSA.DataAccessLayer.Model.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BackgroundService
{
    public static class StepTracker
    {
        public class Track
        {
            public Guid ActivityId { get; set; }
            public DateTime Started { get; set; }
            public Guid UserId { get; set; }
            public string ProcessName { get; set; }
            public Status ProcessState { get; set; }
            public enum Status
            {
                InProcess, Stoped, Error
            }
            public List<Step> Steps = new List<Step>();
            public class Step
            {
                public int No { get; set; }
                public string Name { get; set; }
            }
        }
        //public static List<Track> Tracks = new List<Track>();

        public static void Start(WorkflowContext context, string processName, string stepName)
        {
            List<Track> tracks = null;
            var key = string.Format("{0}__{1}", context.UserId, context.ActivityId);
            var objFromCache = CacheManager.Cache[key];
            if (objFromCache == null)
            {
                tracks = new List<Track>();
                //TODO: throw exception
            }
            else
            {
                tracks = objFromCache as List<Track>;
            }
            tracks.Add(new Track
            {
                Started = DateTime.Now,
                ActivityId = context.ActivityId,
                UserId = context.UserId,
                ProcessName = processName,
                ProcessState = Track.Status.InProcess,
                Steps = new List<Track.Step>
                {
                    new Track.Step { No = 1, Name = stepName }
                }
            });
            CacheManager.Cache[key] = tracks;
        }
        public static void Next(Guid userId, Guid activityId, string stepName)
        {
            List<Track> tracks = null;
            var key = string.Format("{0}__{1}", userId, activityId);
            var objFromCache = CacheManager.Cache[key];
            if (objFromCache == null)
            {
                tracks = new List<Track>();
                //TODO: throw exception
            }
            else
            {
                tracks = objFromCache as List<Track>;
            }
            var track = tracks.Find(x => x.UserId == userId);
            if (track != null)
            {
                track.Steps.Add(new Track.Step
                {
                    Name = stepName,
                    No = track.Steps.Max(x => x.No) + 1
                });
            }
        }
        public static void Stop(Guid userId, Guid activityId)
        {
            List<Track> tracks = null;
            var key = string.Format("{0}__{1}", userId, activityId);
            var objFromCache = CacheManager.Cache[key];
            if (objFromCache == null)
            {
                tracks = new List<Track>();
                //TODO: throw exception
            }
            else
            {
                tracks = objFromCache as List<Track>;
            }
            var track = tracks.Find(x => x.UserId == userId);
            tracks.Remove(track);
        }
        public static void StopWithError(Guid userId, Guid activityId)
        {
            List<Track> tracks = null;
            var key = string.Format("{0}__{1}", userId, activityId);
            var objFromCache = CacheManager.Cache[key];
            if (objFromCache == null)
            {
                tracks = new List<Track>();
                //TODO: throw exception
            }
            else
            {
                tracks = objFromCache as List<Track>;
            }
            var track = tracks.Find(x => x.UserId == userId);
            track.ProcessName = track.ProcessName + " - ошибка";
            track.ProcessState = Track.Status.Error;
        }
    }
}