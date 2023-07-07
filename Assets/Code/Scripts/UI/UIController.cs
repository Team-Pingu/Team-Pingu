using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Game.CustomUI;
using Game.CustomUI.Seed;
using Code.Scripts.Player.Controller;
using Code.Scripts;
using Code.Scripts.TimelineEvents;
using System.Linq;

enum ModalType
{
    UpgradeModal,
    AttackerInitModal,
    DefenderInitModal
}

public class UIController : MonoBehaviour
{
    [SerializeField]
    private bool UseSeedInitializer = true;
    public bool IsUpgradeMenuOpen;

    private VisualElement _root;
    private UnitCardPanel _cardPanel;
    private VisualElement _popupContainer;
    private VisualElement _upgradeMenu;
    private Button _upgradeMenuOpenButton;
    private Button _upgradeMenuCloseButton;
    private Label _currencyLabel;
    private VisualElement _timelineContainer;
    private VisualElement _timelineEventTemplate;
    private VisualElement _timelinePreparationPhase;
    private VisualElement _timelineTimer;
    private VisualElement _timelineMatchEventsContainer;
    private VisualElement _timelinePhaseContainer;
    private Label _timerPhaseLabel;
    private Label _timerPhaseTimerLabel;
    private Label _timerEventLabel;
    private Label _timerEventTimerLabel;

    private readonly string UPGRADE_MODAL_NAME = "game-upgrade-popup";
    private readonly string ATTACKER_INIT_MODAL_NAME = "game-start-popup-attacker";
    private readonly string DEFENDER_INIT_MODAL_NAME = "game-start-popup-defender";

    private Player _player;
    private Bank _bank;
    private TimelineEventsManager _timelineEventsManager;

    private void Start()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        _cardPanel = _root.Q<VisualElement>("unit-card-panel") as UnitCardPanel;
        _popupContainer = _root.Q<VisualElement>("popup-container");
        _upgradeMenuOpenButton = _root.Q<Button>("player-controls__upgrade-btn");
        _upgradeMenuCloseButton = _root.Q<Button>("game-upgrade-popup__actions__close");
        _upgradeMenu = _root.Q<VisualElement>(UPGRADE_MODAL_NAME);
        _currencyLabel = _root.Q<Label>("player-controls__currency__text");
        const string timerEventBaseClass = "timer__next-event";
        _timerEventLabel = _root.Q<Label>($"{timerEventBaseClass}__event-text");
        _timerEventTimerLabel = _root.Q<Label>($"{timerEventBaseClass}__timer-text");
        const string timerPhaseBaseClass = "timer__current-phase";
        _timerPhaseLabel = _root.Q<Label>($"{timerPhaseBaseClass}__phase-text");
        _timerPhaseTimerLabel = _root.Q<Label>($"{timerPhaseBaseClass}__timer-text");

        _upgradeMenuOpenButton.RegisterCallback<ClickEvent, VisualElement>(OnUpgradeMenuOpenClick, _upgradeMenu);
        _upgradeMenuCloseButton.RegisterCallback<ClickEvent>(OnUpgradeMenuCloseClick);
        _root.Q<Button>($"{ATTACKER_INIT_MODAL_NAME}__actions__close").RegisterCallback<ClickEvent>(OnModalClose);
        _root.Q<Button>($"{DEFENDER_INIT_MODAL_NAME}__actions__close").RegisterCallback<ClickEvent>(OnModalClose);
        _root.Q<VisualElement>("static")?.SendToBack();

        _player = GameObject.Find("Player").GetComponent<Player>();
        _bank = GameObject.Find("Player").GetComponent<Bank>();
        UpdateCurrencyText(_bank.CurrentBalance);
        _timelineEventsManager = GameObject.Find("TimelineEventsManager").GetComponent<TimelineEventsManager>();

        _bank.OnBalanceChanged += currentBalance => UpdateCurrencyText(currentBalance);

        InitTimelineElements();

        _timelineEventsManager.OnTimerChanged += OnTimerChanged;
        _timelineEventsManager.OnTimelineEventExecuted += OnTimelineEventExecuted;
        _timelineEventsManager.OnTimelinePhaseChanged += OnTimelinePhaseChanged;
        _timelineEventsManager.OnTimelineEnded += OnTimelineEnded;

        InitSeed();
    }

    #region Events
    private void OnTimerChanged(int currentTimerInSeconds)
    {
        //_timerPhaseTimerLabel.text = GetFormattedSecondsRunning(_timelineEventsManager.AllPhasesDuration - _timelineEventsManager.Timer);

        if (_timerEventTimerLabel?.userData != null && _timerEventTimerLabel.userData.GetType() == typeof(int) && (int)_timerEventTimerLabel.userData >= 0)
        {
            int eventTimerSecondsLeft = (int)_timerEventTimerLabel.userData - currentTimerInSeconds;
            if (eventTimerSecondsLeft > 5 || eventTimerSecondsLeft <= 0)
            {
                _timerEventTimerLabel.parent.style.display = DisplayStyle.None;
            }
            else
            {
                _timerEventTimerLabel.parent.style.display = DisplayStyle.Flex;
                _timerEventTimerLabel.text = eventTimerSecondsLeft.ToString();
            }
        }

        if (_timerPhaseTimerLabel?.userData != null && _timerPhaseTimerLabel.userData.GetType() == typeof(int) && (int)_timerPhaseTimerLabel.userData >= 0)
        {
            int phaseTimerSecondsLeft = (int)_timerPhaseTimerLabel.userData - currentTimerInSeconds;
            _timerPhaseTimerLabel.text = GetFormattedSecondsRunning(phaseTimerSecondsLeft);
        }

        float percentage = (_timelineEventsManager.Timer / (float)_timelineEventsManager.AllPhasesDuration) * 100;
        _timelineTimer.style.width = new StyleLength(new Length(percentage, LengthUnit.Percent));
    }

    private void OnTimelineEventExecuted(TimelineEventChangedEventParams args)
    {
        if (args.NextTimelineEvent == null)
        {
            _timerEventLabel.parent.style.display = DisplayStyle.None;
        } else
        {
            _timerEventLabel.parent.style.display = DisplayStyle.Flex;
            _timerEventLabel.text = args.NextTimelineEvent.Name;
            _timerEventTimerLabel.userData = args.NextTimelineEvent.ExecutionTime + args.PhaseExecutionOffset;
        }
    }

    private void OnTimelinePhaseChanged(TimelinePhaseChangedPhaseParams args)
    {
        if (args.CurrentTimelinePhase == null)
        {
            _timerPhaseLabel.parent.style.display = DisplayStyle.None;
        }
        else
        {
            _timerPhaseLabel.parent.style.display = DisplayStyle.Flex;
            _timerPhaseLabel.text = args.CurrentTimelinePhase.Name;
            _timerPhaseTimerLabel.userData = args.CurrentTimelinePhase.StartTime + args.CurrentTimelinePhase.Duration;
        }
    }

    private void OnTimelineEnded()
    {

    }
    #endregion

    void InitTimelineElements()
    {
        _timelineContainer = _root.Q("timeline");
        _timelineEventTemplate = _timelineContainer.Q("timeline-event-template");
        _timelinePreparationPhase = _timelineContainer.Q("timeline__phases__preparation");
        _timelineTimer = _timelineContainer.Q("timeline-timer");
        _timelineMatchEventsContainer = _timelineContainer.Q("timeline__phases__match__events");
        _timelinePhaseContainer = _timelineContainer.Q("timeline__phases");

        int matchPhaseDuration = _timelineEventsManager.TimelineEventsConfig.MatchPhaseDuration;
        foreach (TimelineEvent e in _timelineEventsManager.TimelineEvents)
        {
            VisualElement eventElement = new VisualElement();
            eventElement.AddToClassList("timeline-event-template");
            float percentage = (e.ExecutionTime / (float)matchPhaseDuration) * 100;
            eventElement.style.marginLeft = new StyleLength(new Length(percentage, LengthUnit.Percent));
            if (e.Type == TimelineEventType.AutominionSpawn)
            {
                eventElement.AddToClassList("timeline-event-template--autominion");
            } else if (e.Type == TimelineEventType.MoneyBonus)
            {
                eventElement.AddToClassList("timeline-event-template--moneybonus");
            }
            _timelineMatchEventsContainer.Add(eventElement);
        }

        float preparationPhasePercentage = (_timelineEventsManager.TimelineEventsConfig.PreperationPhaseDuration / (float)_timelineEventsManager.AllPhasesDuration) * 100;
        _timelinePreparationPhase.style.width = new StyleLength(new Length(preparationPhasePercentage, LengthUnit.Percent));

        // disable template event
        _timelineEventTemplate.style.display = DisplayStyle.None;
    }

    private string GetFormattedSecondsRunning(int seconds)
    {
        int minutes = seconds / 60;
        string __minutes = (minutes < 10 ? "0" : "") + minutes.ToString();
        string __seconds = (seconds < 10 ? "0" : "") + (seconds % 60).ToString();
        return $"{__minutes}:{__seconds}";
    }

    private void UpdateCurrencyText(int currentBalance)
    {
        _currencyLabel.text = $"{currentBalance}";
    }

    private void InitSeed()
    {
        if (!UseSeedInitializer) return;
        if (_player.PlayerController == null) return;

        ISeed Seed;
        if (_player.Role == PlayerRole.Attacker)
        {
            _cardPanel.UseSingleSelectionOnly = false;
            Seed = new AttackerSeed();
        }
        else
        {
            _cardPanel.UseSingleSelectionOnly = true;
            Seed = new DefenderSeed();
        }
        Seed.InflateUI(_root);
    }

    #region Events
    private void OnUpgradeMenuOpenClick(ClickEvent e, VisualElement vs)
    {
        Debug.Log("Open");
        if (IsUpgradeMenuOpen) return;
        IsUpgradeMenuOpen = true;
        OpenModal(ModalType.UpgradeModal);
    }

    private void OnUpgradeMenuCloseClick(ClickEvent e)
    {
        Debug.Log("Close");
        if (!IsUpgradeMenuOpen) return;
        IsUpgradeMenuOpen = false;
        OnModalClose(e);
    }

    private void OnModalClose(ClickEvent e)
    {
        _popupContainer.style.display = DisplayStyle.None;
    }
    #endregion

    #region Methods
    private string GetModalNameFromType(ModalType modalType)
    {
        if (modalType == ModalType.UpgradeModal)
        {
            return UPGRADE_MODAL_NAME;
        }
        else if (modalType == ModalType.AttackerInitModal)
        {
            return ATTACKER_INIT_MODAL_NAME;
        }
        else if (modalType == ModalType.DefenderInitModal)
        {
            return DEFENDER_INIT_MODAL_NAME;
        }
        else
        {
            return "";
        }
    }
    private void OpenModal(ModalType modalType)
    {
        _popupContainer.style.display = DisplayStyle.Flex;
        foreach (VisualElement ve in _popupContainer.Children())
        {
            if (ve.name == GetModalNameFromType(modalType))
            {
                ve.style.display = DisplayStyle.Flex;
            }
            else
            {
                ve.style.display = DisplayStyle.None;
            }
        }

        // TODO: implement timer close
    }
    #endregion
}
