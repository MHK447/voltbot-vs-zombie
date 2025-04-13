using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BanpoFri;

public class GameRoot : Singleton<GameRoot>
{
	[SerializeField]
	private Transform MainUITrans;
	[SerializeField]
	private Transform HUDUITrans;
	[SerializeField]
	public Canvas MainCanvas;
	[SerializeField]
	private Canvas WorldCanvas;
	[SerializeField]
	private GameObject CheatWindow;

	[SerializeField]
	private GameObject DebugConsoleObj;


	[HideInInspector]
	public LoadingBasic Loading;
	[SerializeField]
	private AdManager AdManager;
	[SerializeField]
	private InAppPurchaseManager inAppPurchaseManager;
	[SerializeField]
	private Joystick JoyStick;

	public Joystick GetJoyStick { get { return JoyStick; } }
	public InAppPurchaseManager GetInAppPurchaseManager { get { return inAppPurchaseManager; } }

	public RectTransform GetMainCanvasTR { get { return MainCanvas.transform as RectTransform; } }
	public UISystem UISystem { get; private set; } = new UISystem();
	public UserDataSystem UserData { get; private set; } = new UserDataSystem();
	public InGameSystem InGameSystem { get; private set; } = new InGameSystem();
	public SnapshotCamera SnapshotCam { get; private set; } = null;
	public PluginSystem PluginSystem { get; private set; } = new PluginSystem();
	public PlayTimeSystem PlayTimeSystem { get; private set; } = new PlayTimeSystem();
	public EffectSystem EffectSystem { get; private set; } = new EffectSystem();
	public TutorialSystem TutorialSystem { get; private set; } = new TutorialSystem();
	public FacilitySystem FacilitySystem { get; private set; } = new FacilitySystem();
	public UpgradeSystem UpgradeSystem { get; private set; } = new UpgradeSystem();
	public NaviSystem NaviSystem { get; private set; } = new NaviSystem();

	public ShopSystem ShopSystem { get; private set; } = new ShopSystem();


	public GameNotificationSystem GameNotification { get; private set; } = new GameNotificationSystem();

	public VehicleSystem VehicleSystem { get; private set; } = new VehicleSystem();
	public BoostSystem BoostSystem { get; private set; } = new BoostSystem();
	public ContentsOpenSystem ContentsOpenSystem { get; private set; } = new ContentsOpenSystem();



	private Queue<System.Action> PauseActions = new Queue<System.Action>();

	public AdManager GetAdManager { get { return AdManager; } }

	public GameObject UILock;
	private static bool InitTry = false;
	public static bool LoadComplete { get; private set; } = false;
	private float deltaTime = 0f;
	public InGameType CurInGameType { get; private set; } = InGameType.Main;
	private Queue<System.Action> TouchStartActions = new Queue<System.Action>();
	public Queue<System.Action> TitleCloseActions = new Queue<System.Action>();

	private int loadcount = 0;
	public static bool IsInit()
	{


		if (instance != null && !InitTry)
			Load();

		return instance != null;


	}

	public static void Load()
	{
		InitTry = true;
		Addressables.InstantiateAsync("GameRoot").Completed += (handle) =>
		{
			instance = handle.Result.GetComponent<GameRoot>();
			instance.name = "GameRoot";
		};
	}

	public void AddTouchAction(System.Action cb)
	{
		TouchStartActions.Enqueue(cb);
	}

	public void AddTitleCloseAction(System.Action cb)
	{
		TitleCloseActions.Enqueue(cb);
	}


	IEnumerator waitEndFrame(System.Action callback)
	{
		yield return new WaitForEndOfFrame();

		callback?.Invoke();
	}

	public void WaitEndFrameCallback(System.Action callback)
	{
		StartCoroutine(waitEndFrame(callback));
	}

	void InitUILoading()
	{
		// 로딩 팝업 어드레서블 로드
		var path = UISystem.GetUIPath<LoadingBasic>();
		if (!string.IsNullOrEmpty(path))
		{
			Addressables.InstantiateAsync(path, MainUITrans, false).Completed += (handle) =>
			{
				Loading = handle.Result.GetComponent<LoadingBasic>();
				loadcount++;
			};
		}
	}

	private void Update()
	{
		if (!LoadComplete)
			return;

		UserData.Update();
		PlayTimeSystem.Update();

		if (deltaTime >= 1f) // one seconds updates;
		{

			deltaTime -= 1f;

			VehicleSystem.OneSecondUpdate();
			BoostSystem.UpdateOneSecond();
			ShopSystem.UpdateOneTimeSecond();


		}
		deltaTime += Time.deltaTime;

		if (Input.GetMouseButtonUp(0))
		{
			if (InGameSystem.CurInGame != null)
			{
				Vector2 localPos;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(MainCanvas.transform as RectTransform, Input.mousePosition,
					MainCanvas.worldCamera, out localPos);


				if (!IsPointerOverUIObject(Input.mousePosition))
				{
				}


				if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null)
				{


					if (TouchStartActions.Count > 0)
					{
						var cnt = TouchStartActions.Count;
						for (int i = 0; i < cnt; i++)
						{
							TouchStartActions.Dequeue().Invoke();
						}
					}
				}
			}
		}

		

	}

	public bool IsPointerOverUIObject(Vector2 touchPos)
	{
		UnityEngine.EventSystems.PointerEventData eventDataCurrentPosition
			= new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);

		eventDataCurrentPosition.position = touchPos;

		List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();


		UnityEngine.EventSystems.EventSystem.current
		.RaycastAll(eventDataCurrentPosition, results);

		return results.Count > 0;
	}

	public void DestroyObj(GameObject obj)
	{
		Destroy(obj);
	}

	IEnumerator Start()
	{
		if (instance == null)
		{
			Load();
			Destroy(this.gameObject);
			yield break;
		}

		#if BANPOFRI_LOG
			DebugConsoleObj.SetActive(true);
		#else
			DebugConsoleObj.SetActive(false);
		#endif
		
		//TouchStartActions.Clear();
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		PluginSystem.Init();
		//InAppPurchaseManager = GetComponent<InAppPurchaseManager>();
		//if (InAppPurchaseManager != null)
		//	InAppPurchaseManager.Init();
		//SnapshotCam = SnapshotCamera.MakeSnapshotCamera("SnapShot");
		//SnapshotCam.transform.SetParent(this.transform);
		//SnapshotCam.transform.position = new Vector3(0f, 0f, -1f);
		yield return TimeSystem.GetGoogleTime(null);
		UISystem.SetMainUI(MainUITrans);
		UISystem.SetHudUI(HUDUITrans);
		UISystem.SetWorldCanvas(WorldCanvas);
		UISystem.LockScreen = UILock;
		yield return LoadGameData();
		//Application.deepLinkActivated += OnDeepLinkActivated;
		if (!string.IsNullOrEmpty(Application.absoluteURL))
		{
			// Cold start and Application.absoluteURL not null so process Deep Link.
			//OnDeepLinkActivated(Application.absoluteURL);
		}
	}

	private IEnumerator LoadGameData()
	{

		yield return Config.Create();
		yield return Tables.Create();
		yield return SoundPlayer.Create();

		// 로딩 팝업 어드레서블 로드
		loadcount = 0;
		InitUILoading();

		yield return new WaitUntil(() => loadcount == 1);
		UserData.Load();
		InGameSystem.ChangeMode(CurInGameType);

		LoadComplete = true;

		InitSystem();

		InGameSystem.Create();
		GameNotification.Create();
		VehicleSystem.Create();
		BoostSystem.Create();
		NaviSystem.Create();
		ShopSystem.Create();


		GameRoot.instance.WaitTimeAndCallback(0.5f, () =>
		{
			JoyStick.Init();
			BgmOn();
		});
	}

	public void BgmOn()
	{
		if (GameRoot.instance.CurInGameType == InGameType.Event)
		{
			SoundPlayer.Instance.PlayBGM("bgm_pirate", true);
			SoundPlayer.Instance.BgmSwitch(UserData.Bgm);
		}
		else
		{
			SoundPlayer.Instance.PlayBGM("bgm", true);
			SoundPlayer.Instance.BgmSwitch(UserData.Bgm);
		}
	}

	public void ChangeIngameType(InGameType type, bool changeData = false)
	{

		CurInGameType = type;
		//InGameSystem.ChangeMode(CurInGameType);
		if (changeData)
		{
			DataState dataState = DataState.None;
			switch (CurInGameType)
			{
				case InGameType.Main:
					{
						dataState = DataState.Main;
					}
					break;
				case InGameType.Event:
					{
						dataState = DataState.Event;
					}
					break;
			}
			if (dataState != DataState.None)
			{
				UserData.ChangeDataMode(dataState);
				GameRoot.Instance.UserData.CurMode.LastLoginTime = TimeSystem.GetCurTime();
			}
		}
	}

	void InitSystem()
	{
		QualitySettings.vSyncCount = 0;  // VSync 비활성화
		Application.targetFrameRate = 60;  // (제한 없음)

		var count = GameRoot.instance.UserData.GetRecordCount(Config.RecordCountKeys.Init);

		if (GameRoot.Instance.UserData.CurMode.StageData.StageIdx == 1 && count == 0)
		{
			GameRoot.instance.UserData.AddRecordCount(Config.RecordCountKeys.Init, 1);

			GameRoot.instance.UserData.SetReward((int)Config.RewardType.Currency, (int)Config.CurrencyID.Money, 100);

			GameRoot.instance.FacilitySystem.CreateStageFacility(GameRoot.Instance.UserData.CurMode.StageData.StageIdx);

			GameRoot.instance.UpgradeSystem.StageSetUpgradeData(1);

			SetNativeLanguage();
		}
		else
		{
			
		}

	}

	private void SetNativeLanguage()
	{
	}

	public void SetCheatWindow(bool value)
	{
		if (CheatWindow != null)
			CheatWindow.SetActive(value);
            
		// DebugLogManager가 있으면 팝업도 함께 활성화/비활성화
		if (IngameDebugConsole.DebugLogManager.Instance != null)
		{
			IngameDebugConsole.DebugLogManager.Instance.PopupEnabled = value;
		}
	}

	IEnumerator waitTimeAndCallback(float time, System.Action callback)
	{
		yield return new WaitForSeconds(time);
		callback?.Invoke();
	}

	IEnumerator waitFrameAndCallback(int frame, System.Action callback)
	{
		for (int i = 0; i < frame; i++)
			yield return new WaitForEndOfFrame();
		callback?.Invoke();


	}

	public void WaitTimeAndCallback(float time, System.Action callback)
	{
		StartCoroutine(waitTimeAndCallback(time, callback));
	}

	public void WaitFrameAndCallback(int frame, System.Action callback)
	{
		StartCoroutine(waitFrameAndCallback(frame, callback));
	}

	public Vector3 GetRewardEndPos(int rewardType, int rewardIdx, UIBase ui)
	{
		return ui.GetCurrencyImgTr(rewardType, rewardIdx).position;
	}



	private void OnApplicationPause(bool pause)
	{
		if (!LoadComplete)
			return;

		if (pause)
		{

			PluginSystem.OnApplicationPause(pause);

			GameRoot.Instance.UserData.CurMode.LastLoginTime = TimeSystem.GetCurTime();
		}
		else
		{
			if (InGameSystem.GetInGame<InGameTycoon>() == null) return;
			if (InGameSystem.GetInGame<InGameTycoon>().curInGameStage == null) return;
			if (InGameSystem.GetInGame<InGameTycoon>().curInGameStage.IsLoadComplete == false) return;


			if (GameRoot.Instance.TutorialSystem.IsActive())
				return;

			var time = GameRoot.Instance.UserData.CurMode.LastLoginTime;

			if (time.Equals(default(System.DateTime)))
				return;




			System.Action NextAction = () =>
			{
				if (PauseActions.Count < 1)
					return;

				var action = PauseActions.Dequeue();
				action.Invoke();
			};

			var diff = TimeSystem.GetCurTime().Subtract(time);

			var minRewardTime = Tables.Instance.GetTable<Define>().GetData("max_offline_time").value;
			var maxRewardTime = Tables.Instance.GetTable<Define>().GetData("offline_min_time").value;

			var stage = GameRoot.instance.InGameSystem.GetInGame<InGameTycoon>().curInGameStage;

			if (diff.TotalSeconds > minRewardTime)
			{
				PauseActions.Enqueue(() =>
				{
					var offlinereward = GameRoot.instance.UISystem.GetUI<PopupOfflineReward>();

					if (offlinereward != null)
					{
						if (!offlinereward.gameObject.activeSelf)
						{
							if ((int)diff.TotalSeconds >= maxRewardTime)
								GameRoot.instance.UISystem.OpenUI<PopupOfflineReward>(popup => popup.Set(maxRewardTime), NextAction);
							else
								GameRoot.instance.UISystem.OpenUI<PopupOfflineReward>(popup => popup.Set((int)diff.TotalSeconds), NextAction);
						}
						else
						{
							offlinereward.OnUIHideAfter = NextAction;

							//기존 열린 팝업의 시간이 맥스가 아니면 오프라인 시간을 더해서 보상을 준다.
							if (offlinereward.TimeSecond < maxRewardTime)
							{
								int rewardTime = 0;
								var newRewardTime = offlinereward.TimeSecond + (int)diff.TotalSeconds;
								if (newRewardTime >= maxRewardTime)
								{
									rewardTime = maxRewardTime;
								}
								else
								{
									rewardTime = newRewardTime;
								}

								offlinereward.Set(rewardTime);
							}
						}
					}
					else
					{

						if ((int)diff.TotalSeconds >= maxRewardTime)
							GameRoot.instance.UISystem.OpenUI<PopupOfflineReward>(popup => popup.Set(maxRewardTime), NextAction); //offline max value 
						else
							GameRoot.instance.UISystem.OpenUI<PopupOfflineReward>(popup => popup.Set((int)diff.TotalSeconds), NextAction); //offline not max value

					}
				});

			}
			NextAction.Invoke();

		}



	}


#if UNITY_EDITOR
	private void OnApplicationQuit()
	{
		PluginSystem.OnApplicationPause(true);
		UnityEditor.AssetDatabase.SaveAssets();
		UnityEditor.AssetDatabase.Refresh();
	}
#endif

}
