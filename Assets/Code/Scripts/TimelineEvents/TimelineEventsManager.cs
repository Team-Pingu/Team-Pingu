using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Scripts.TimelineEvents
{
    public class TimelinePhaseChangedPhaseParams
    {
        public TimelinePhase NextTimelinePhase;
        public TimelinePhase CurrentTimelinePhase;
    }

    public class TimelineEventChangedEventParams
    {
        public TimelineEvent NextTimelineEvent;
        public TimelineEvent CurrentTimelineEvent;
        public int PhaseExecutionOffset;
    }

    public class TimelineEventsManager : MonoBehaviour
    {
        [SerializeField]
        public TimelineEventsScriptableObject TimelineEventsConfig;
        public bool TimerRunning { get; private set; } = false;
        public TimelineEvent[] TimelineEvents { get; private set; }
        public TimelinePhase[] TimelinePhases { get; private set; }

        private float _startTime = 0;
        private UpgradeManager _upgradeManager;
        private int _autoMinionSpawnAmount;
        private TimelinePhase _currentPhase;

        public event Action<TimelineEventChangedEventParams> OnTimelineEventExecuted;
        public event Action<TimelinePhaseChangedPhaseParams> OnTimelinePhaseChanged;
        public event Action OnTimelineEnded;
        public event Action<int> OnTimerChanged;
        public int AllPhasesDuration { get; private set; }
        public int Timer { get; private set; } = 0;

        void Start()
        {
            _autoMinionSpawnAmount = TimelineEventsConfig.InitialAutoMinionSpawnAmount;
            TimelineEvents = TimelineEventsConfig.GetPropagatedMatchEventsInExecutionOrder();
            TimelinePhases = TimelineEventsConfig.GetTimelinePhasesInExecutionOrder();
            _upgradeManager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
            AllPhasesDuration = TimelinePhases.Sum(x => x.Duration);

            StartTimelineEvents();
        }

        private void Update()
        {
            Timer = GetSecondsRunning();
            EvaluateTimelinePhases();
            EvaluateTimelineEvents();
        }

        private void EvaluateTimelineEvents()
        {
            if (_currentPhase == null) return;
            if (!TimerRunning) return;
            if (_currentPhase.Type == TimelinePhaseType.Match)
            {
                int currentTime = Timer - _currentPhase.StartTime;
                int i = 0;
                foreach (TimelineEvent tle in TimelineEvents)
                {
                    i++;
                    if (tle.IsExecuted) continue;
                    if (currentTime >= tle.ExecutionTime)
                    {
                        Debug.Log($"Executing Event {tle.Name}");
                        ExecuteEvent(tle);
                        var eventParams = new TimelineEventChangedEventParams()
                        {
                            NextTimelineEvent = i >= TimelineEvents.Length ? null : TimelineEvents[i],
                            CurrentTimelineEvent = tle,
                            PhaseExecutionOffset = _currentPhase.StartTime
                        };
                        OnTimelineEventExecuted?.Invoke(eventParams);
                        tle.SetExecuted();
                    }
                }
            }
        }

        private void EvaluateTimelinePhases()
        {
            if (!TimerRunning) return;

            int currentTime = Timer;
            int i = 0;
            foreach (TimelinePhase tlp in TimelinePhases)
            {
                i++;
                if (tlp.IsExecuted) continue;
                if (currentTime >= tlp.StartTime)
                {
                    Debug.Log($"Current Phase started: {tlp.Name}");
                    OnTimelinePhaseChanged?.Invoke(new TimelinePhaseChangedPhaseParams()
                    {
                        NextTimelinePhase = i >= TimelinePhases.Length ? null : TimelinePhases[i],
                        CurrentTimelinePhase = tlp
                    });
                    _currentPhase = tlp;
                    tlp.SetExecuted();

                    if (tlp.Type == TimelinePhaseType.Match)
                    {
                        OnTimelineEventExecuted?.Invoke(new TimelineEventChangedEventParams()
                        {
                            NextTimelineEvent = TimelineEvents.FirstOrDefault(),
                            CurrentTimelineEvent = null,
                            PhaseExecutionOffset = tlp.StartTime
                        });
                    }
                }
            }

            if (currentTime >= AllPhasesDuration)
            {
                Debug.Log("Match Ended!");
                OnTimelineEnded?.Invoke();
                StopTimelineEvents();
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

            _autoMinionSpawnAmount = TimelineEventsConfig.InitialAutoMinionSpawnAmount;
            // TODO: Reset Money Bonus too?
        }

        private void ResetTimelinePhases()
        {
            foreach (TimelinePhase tlp in TimelinePhases)
            {
                tlp.SetExecuted(false);
            }
        }

        private int GetSecondsRunning()
        {
            if (!TimerRunning) return 0;
            int seconds = (int)(Time.realtimeSinceStartup - _startTime);
            //int seconds = (int)(_startTime + Time.deltaTime);
            OnTimerChanged?.Invoke(seconds);
            return seconds;
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