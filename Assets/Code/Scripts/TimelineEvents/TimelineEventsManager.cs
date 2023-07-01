using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Scripts.TimelineEvents
{
    public class TimelinePhaseChangedEventParams
    {
        public TimelinePhase PreviousTimelinePhase;
        public TimelinePhase CurrentTimelinePhase;
    }

    public class TimelineEventsManager : MonoBehaviour
    {
        [SerializeField]
        private TimelineEventsScriptableObject _timelineEventsConfig;
        public bool TimerRunning { get; private set; } = false;
        public TimelineEvent[] TimelineEvents { get; private set; }
        public TimelinePhase[] TimelinePhases { get; private set; }

        private float _startTime = 0;
        private UpgradeManager _upgradeManager;
        private int _autoMinionSpawnAmount;
        private TimelinePhase _currentPhase;

        public event Action<TimelineEvent> OnTimelineEventExecuted;
        /// <summary>
        /// Event for Phase change. First value = current phase, second = previous phase
        /// </summary>
        public event Action<TimelinePhaseChangedEventParams> OnTimelinePhaseChanged;

        void Start()
        {
            _autoMinionSpawnAmount = _timelineEventsConfig.InitialAutoMinionSpawnAmount;
            TimelineEvents = _timelineEventsConfig.GetPropagatedMatchEventsInExecutionOrder();
            TimelinePhases = _timelineEventsConfig.GetTimelinePhasesInExecutionOrder();
            _upgradeManager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();

            StartTimelineEvents();
        }

        private void Update()
        {
            EvaluateTimelinePhases();
            EvaluateTimelineEvents();
        }

        private void EvaluateTimelineEvents()
        {
            if (_currentPhase == null) return;
            if (!TimerRunning) return;
            if (_currentPhase.Type == TimelinePhaseType.Match)
            {
                int currentTime = (int)GetSecondsRunning() - _currentPhase.StartTime;
                foreach (TimelineEvent tle in TimelineEvents)
                {
                    if (tle.IsExecuted) continue;
                    if (currentTime >= tle.ExecutionTime)
                    {
                        Debug.Log($"Executing Event {tle.Name}");
                        ExecuteEvent(tle);
                        OnTimelineEventExecuted?.Invoke(tle);
                        tle.SetExecuted();
                    }
                }
            }
        }

        private void EvaluateTimelinePhases()
        {
            if (!TimerRunning) return;

            int currentTime = (int)GetSecondsRunning();
            foreach (TimelinePhase tlp in TimelinePhases)
            {
                if (tlp.IsExecuted) continue;
                if (currentTime >= tlp.StartTime)
                {
                    Debug.Log($"Current Phase started: {tlp.Name}");
                    OnTimelinePhaseChanged?.Invoke(new TimelinePhaseChangedEventParams()
                    {
                        PreviousTimelinePhase = _currentPhase,
                        CurrentTimelinePhase = tlp
                    });
                    _currentPhase = tlp;
                    tlp.SetExecuted();
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
            _startTime = 0;
            ResetTimelineEvents();
            ResetTimelinePhases();
            TimerRunning = false;
        }

        private void ResetTimelineEvents()
        {
            foreach (TimelineEvent tle in TimelineEvents)
            {
                tle.SetExecuted(false);
            }

            _autoMinionSpawnAmount = _timelineEventsConfig.InitialAutoMinionSpawnAmount;
            // TODO: Reset Money Bonus too?
        }

        private void ResetTimelinePhases()
        {
            foreach (TimelinePhase tlp in TimelinePhases)
            {
                tlp.SetExecuted(false);
            }
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

        private void ExecuteEvent(TimelineEvent timelineEvent)
        {
            if (timelineEvent == null) return;
            if (timelineEvent.Type == TimelineEventType.AutominionSpawn)
            {
                ExecuteAutominionSpawnEvent(timelineEvent);
            }
            else if (timelineEvent.Type == TimelineEventType.MoneyBonus)
            {
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
            }
            else
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
