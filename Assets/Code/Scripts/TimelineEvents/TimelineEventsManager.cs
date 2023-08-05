using Code.Scripts.Player.Controller;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using Code.Scripts.Pathfinding;
using static UnityEditor.Experimental.GraphView.GraphView;

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
        public TimelineEvent NextUserInterfaceVisibleEvent;
        public int PhaseExecutionOffset;
    }

    public class TimelineEventsManager : NetworkBehaviour
    {
        [SerializeField]
        public TimelineEventsScriptableObject TimelineEventsConfig;
        public bool TimerRunning { get; private set; } = false;
        public TimelineEvent[] TimelineEvents { get; private set; }
        public TimelinePhase[] TimelinePhases { get; private set; }

        private NetworkVariable<float> _startTime = new NetworkVariable<float>(0);
        private UpgradeManager _upgradeManager;
        private int _autoMinionSpawnAmount;
        private TimelinePhase _currentPhase;
        private PlayerController _playerController;
        private Player.Controller.Player _player;
        private Bank _bank;

        public event Action<TimelineEventChangedEventParams> OnTimelineEventExecuted;
        public event Action<TimelinePhaseChangedPhaseParams> OnTimelinePhaseChanged;
        public event Action OnTimelineEnded;
        public event Action<int> OnTimerChanged;
        public int AllPhasesDuration { get; private set; }
        public int Timer { get; private set; } = 0;

        private Boolean isTimelineRunning = false;

        void Start()
        {
            _autoMinionSpawnAmount = TimelineEventsConfig.InitialAutoMinionSpawnAmount;
            TimelineEvents = TimelineEventsConfig.GetPropagatedMatchEventsInExecutionOrder();
            TimelinePhases = TimelineEventsConfig.GetTimelinePhasesInExecutionOrder();
            _upgradeManager = GameObject.Find("UpgradeManager").GetComponent<UpgradeManager>();
            AllPhasesDuration = TimelinePhases.Sum(x => x.Duration);

            _player = FindObjectOfType<Player.Controller.Player>();
            _playerController = FindAnyObjectByType<PlayerController>();
            _bank = _playerController.GetBank();

            if ((NetworkManager.Singleton == null))
            {
                StartTimelineEvents();
                isTimelineRunning = true;
            }
        }

        private void Update()
        {
            if (NetworkManager.Singleton != null)
            {
                if (NetworkManager.Singleton.IsClient) return;
                if (!isTimelineRunning)
                {
                    if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
                    {
                        StartTimelineEvents();
                        isTimelineRunning = true;
                    }
                }
            }

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
                        TimelineEvent nextUIVisibleEvent = TimelineEvents.Where(
                            x => (!x.IsExecuted && x.Name != tle.Name && x.IsVisibleInUI)
                        ).FirstOrDefault();
                        var eventParams = new TimelineEventChangedEventParams()
                        {
                            NextTimelineEvent = i >= TimelineEvents.Length ? null : TimelineEvents[i],
                            NextUserInterfaceVisibleEvent = nextUIVisibleEvent,
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
            _startTime.Value = Time.realtimeSinceStartup;
            TimerRunning = true;
        }

        public void StopTimelineEvents()
        {
            _startTime.Value = 0;
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
            int seconds = (int)(Time.realtimeSinceStartup - _startTime.Value);
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
            else if (timelineEvent.Type == TimelineEventType.MoneyIncrease)
            {
                ExecuteMoneyIncreaseEvent(timelineEvent);
            }
        }

        private void ExecuteMoneyIncreaseEvent(TimelineEvent timelineEvent)
        {
            int increaseAmount;
            if (!int.TryParse(timelineEvent.CustomAttributeValue, out increaseAmount))
            {
                Debug.LogError("Could not parse CustomAttributeValue");
                return;
            }

            _bank.Deposit((int)(increaseAmount * _upgradeManager.MoneyBonusMultiplier));
        }

        private void ExecuteAutominionSpawnEvent(TimelineEvent timelineEvent)
        {
            if (_player.Role != PlayerRole.Attacker) return;

            int minionAmount;
            if (!int.TryParse(timelineEvent.CustomAttributeValue, out minionAmount))
            {
                Debug.LogError("Could not parse CustomAttributeValue");
                return;
            }

            // Spawn minions
            Debug.Log($"Spawning {_autoMinionSpawnAmount} Autominions");
            var tiles = FindObjectOfType<Pathfinder>()?.GetAllPathsStartTiles();
            if (tiles == null) Debug.LogError("Path start tiles cannot be found!");
            else
            {
                var autoMinionDict = new Dictionary<string, int> { { "AutoMinion", _autoMinionSpawnAmount } };
                foreach (var tile in tiles)
                {
                    _player.SetActiveEntities(autoMinionDict);
                    tile.SpawnActiveEntities();
                }
            }

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
