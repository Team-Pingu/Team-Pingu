using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.Scripts.TimelineEvents
{
    public enum TimelineEventType
    {
        AutominionSpawn,
        MoneyBonus,
    }

    public enum TimelinePhaseType
    {
        Preparation,
        Match,
        Pre
    }

    [Serializable]
    public class TimelinePhase
    {
        public string Name;
        public TimelinePhaseType Type;
        public int Duration;
        public int StartTime = 0;
        public bool IsExecuted { get; private set; } = false;
        public void SetExecuted(bool executed = true)
        {
            IsExecuted = executed;
        }
    }

    [Serializable]
    public class TimelineEvent
    {
        public TimelineEvent()
        {

        }

        public TimelineEvent(TimelineEvent original)
        {
            Name = original.Name;
            Type = original.Type;
            IsRepeated = original.IsRepeated;
            RepeatInterval = original.RepeatInterval;
            MaxExecutionAmount = original.MaxExecutionAmount;
            CustomAttributeValue = original.CustomAttributeValue;
            ExecutionTime = original.ExecutionTime;
        }

        public string Name;
        public TimelineEventType Type;
        public bool IsRepeated = false;
        [Tooltip("Interval of when Event should repeat in seconds")]
        public int RepeatInterval = 60;
        public int MaxExecutionAmount;
        [Tooltip("Set a custom value, which can be used in any way")]
        public string CustomAttributeValue;
        [Tooltip("Time of exection in seconds. When Event is repeatable, this is the initial execution time.")]
        public int ExecutionTime = 0;

        public bool IsExecuted { get; private set; } = false;
        public void SetExecuted(bool executed = true)
        {
            IsExecuted = executed;
        }
    }

    [CreateAssetMenu(fileName = "TimelineEventsConfig", menuName = "ScriptableObjects/TimelineEvents")]
    public class TimelineEventsScriptableObject : ScriptableObject
    {
        [Tooltip("Preperation Phase Duration in seconds")]
        public int PreperationPhaseDuration = 120; // 2 minutes

        [Tooltip("Match Phase Duration in seconds")]
        public int MatchPhaseDuration = 780; // 13 minutes

        [SerializeField]
        public TimelineEvent[] MatchEvents;

        public int InitialAutoMinionSpawnAmount = 4;

        private void ValidateEvents()
        {
            foreach (var e in MatchEvents)
            {
                if (e.ExecutionTime >= MatchPhaseDuration)
                {
                    Debug.LogWarning($"{e.Name} will never be executed, because it is outside the match duration.");
                    continue;
                }
                if (e.IsRepeated)
                {
                    if (e.MaxExecutionAmount != 0 && ((e.ExecutionTime + e.MaxExecutionAmount * e.RepeatInterval) >= MatchPhaseDuration))
                        Debug.LogWarning($"{e.Name} repeated executed events exceed match duration.");
                }
            }
        }

        public void OnValidate()
        {
            ValidateEvents();
        }

        /// <summary>
        /// Unpack all Matchevents in a straight event timeline, removing repeated events completely and mapping them
        /// </summary>
        /// <returns></returns>
        public TimelineEvent[] GetPropagatedMatchEventsInExecutionOrder()
        {
            var propagatedEvents = new List<TimelineEvent>();
            foreach (var e in MatchEvents)
            {
                if (e.IsRepeated)
                {
                    int executionTime = e.ExecutionTime;
                    int i = 0;
                    while (executionTime < MatchPhaseDuration)
                    {
                        if (e.MaxExecutionAmount != 0 && i >= e.MaxExecutionAmount) break;
                        var newEvent = new TimelineEvent(e);
                        newEvent.Name = $"{e.Name} ({i + 1})";
                        newEvent.ExecutionTime = executionTime;

                        propagatedEvents.Add(newEvent);

                        executionTime += e.RepeatInterval;
                        i++;
                    }
                }
                else
                {
                    var newEvent = new TimelineEvent(e);
                    propagatedEvents.Add(newEvent);
                }
            }

            return propagatedEvents.OrderBy(e => e.ExecutionTime).ToArray();
        }

        public TimelinePhase[] GetTimelinePhasesInExecutionOrder()
        {
            var result = new List<TimelinePhase>();
            result.Add(
                new TimelinePhase()
                {
                    Name = "Preparation Phase",
                    Type = TimelinePhaseType.Preparation,
                    Duration = this.PreperationPhaseDuration
                }
            );
            result.Add(
                new TimelinePhase()
                {
                    Name = "Match Phase",
                    Type = TimelinePhaseType.Match,
                    Duration = this.MatchPhaseDuration,
                }
            );

            // calc start time of each event based on previous events
            int startTime = 0;
            foreach(var e in result)
            {
                e.StartTime = startTime;
                startTime += e.Duration;
            }

            return result.OrderBy(e => e.StartTime).ToArray();
        }
    }
}
