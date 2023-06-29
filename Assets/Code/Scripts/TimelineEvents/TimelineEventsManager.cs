using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Scripts.TimelineEvents
{
    public class SimpleTimelineEvent
    {
        public string Name;
        public float Percentage;
    }

    public class TimelineEventsManager : MonoBehaviour
    {
        public TimelineEventsScriptableObject TimelineEventsConfig;
        public bool TimerRunning { get; private set; } = false;

        private float _startTime = 0;
        private UpgradeManager _upgradeManager;
        private int _autoMinionSpawnAmount;
        private TimelineEvent[] _timelineEvents;

        public event Action<TimelineEvent> OnTimelineEventExecuted;

        void Start()
        {
            _autoMinionSpawnAmount = TimelineEventsConfig.InitialAutoMinionSpawnAmount;
            _timelineEvents = TimelineEventsConfig.GetPropagatedMatchEvents();
            _upgradeManager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();

            StartTimelineEvents();
        }

        private void Update()
        {
            EvaluateTimelineEvents();
        }

        private void EvaluateTimelineEvents()
        {
            if (TimerRunning)
            {
                int currentTime = (int)GetSecondsRunning();
                foreach(TimelineEvent tle in _timelineEvents)
                {
                    if (tle.IsExecuted) continue;
                    if (currentTime >= tle.ExecutionTime)
                    {
                        ExecuteEvent(tle);
                        OnTimelineEventExecuted?.Invoke(tle);
                        tle.SetExecuted();
                    }
                }
            }
        }

        public void StartTimelineEvents()
        {
            _startTime = Time.realtimeSinceStartup;
            TimerRunning = true;
        }

        public void StopTimelineEvents()
        {
            ResetTimelineEvents();
            TimerRunning = false;
        }

        public void ResetTimelineEvents()
        {
            _startTime = 0;
            foreach(TimelineEvent tle in _timelineEvents)
            {
                tle.SetExecuted(false);
            }

            _autoMinionSpawnAmount = TimelineEventsConfig.InitialAutoMinionSpawnAmount;
            // TODO: Reset Money Bonus too?
        }

        public float GetSecondsRunning()
        {
            if (!TimerRunning) return 0;
            return Time.realtimeSinceStartup - _startTime;
        }

        public string GetFormattedSecondsRunning()
        {
            float currentTime = GetSecondsRunning();
            int seconds = (int)currentTime;
            int minutes = seconds / 60;
            string _minutes = (minutes < 10 ? "0" : "") + minutes.ToString();
            string _seconds = (seconds < 10 ? "0" : "") + (seconds % 60).ToString();
            return $"{_minutes}:{_seconds}";
        }

        public SimpleTimelineEvent[] GetWholeTimescale()
        {
            // TODO: List of all Phases and Events
            return null;
        }

        private void ExecuteEvent(TimelineEvent timelineEvent)
        {
            if (timelineEvent == null) return;
            if (timelineEvent.Type == TimelineEventType.AutominionSpawn)
            {
                ExecuteAutominionSpawnEvent(timelineEvent);
            } else if (timelineEvent.Type == TimelineEventType.MoneyBonus) {
                ExecuteMoneyBonusEvent(timelineEvent);
            }
        }

        private void ExecuteAutominionSpawnEvent(TimelineEvent timelineEvent)
        {
            int minionAmount;
            if (!int.TryParse(timelineEvent.CustomAttributeValue, out minionAmount))
            {
                Debug.LogError("Could not parse CustomAttributeValue");
                return;
            }

            // TODO: Spawn minions
            Debug.Log($"Spawning {_autoMinionSpawnAmount} Autominions");

            if (timelineEvent.IsRepeated)
            {
                // relative minion amount adjustment
                _autoMinionSpawnAmount += minionAmount;
            } else
            {
                // absolute
                _autoMinionSpawnAmount = minionAmount;
            }
        }

        private void ExecuteMoneyBonusEvent(TimelineEvent timelineEvent)
        {
            float moneyBonusValue;
            if (!float.TryParse(timelineEvent.CustomAttributeValue, out moneyBonusValue))
            {
                Debug.LogError("Could not parse CustomAttributeValue");
                return;
            }

            if (_upgradeManager == null) return;

            // update money bonus relatively
            _upgradeManager.UpdateMoneyBonusMultiplier(moneyBonusValue);
            Debug.Log($"Increased Money Bonus to {_upgradeManager.MoneyBonusMultiplier}");
        }
    }
}
