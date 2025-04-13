using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using FlatBuffers;
using BanpoFri.Data;
using BanpoFri;

public partial class UserDataSystem
{
	private readonly string dataFileName = "Master.dat";

	private BanpoFri.Data.UserData flatBufferUserData;
	private float saveWaitStandardTime = 0.3f;
	private float deltaTime = 0f;
	private bool saving = false;
    public bool Vib = true;
    public bool SubscribeOrder { get; set; } = true;
    public bool AutoFelling { get; set; } = false;

	public void Update()
	{
		if (saving)
		{
			if (deltaTime > saveWaitStandardTime)
			{
				saving = false;
				SaveFile();
				return;
			}
			deltaTime += Time.deltaTime;
		}

	}

	public string GetSaveFilePath()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return $"{Application.persistentDataPath}/{dataFileName}";
		}
		else
		{
			return $"{Application.dataPath}/{dataFileName}";
		}
	}



	public string GetBackUpSaveFilePath()
	{
		var path = GetSaveFilePath();
		return $"{path}.backup";
	}


	public void Load()
	{


		var filePath = GetBackUpSaveFilePath();

		if (File.Exists(filePath))
		{
			mainData = new UserDataMain();
			eventData = new UserDataEvent();
			CurMode = mainData;
			var data = File.ReadAllBytes(filePath);
			ByteBuffer bb = new ByteBuffer(data);
			flatBufferUserData = BanpoFri.Data.UserData.GetRootAsUserData(bb);
			ConnectReadOnlyDatas();
			File.Delete(filePath);
			return;
		}







		filePath = GetSaveFilePath();

		if (File.Exists(filePath))
		{
			var data = File.ReadAllBytes(filePath);
			ByteBuffer bb = new ByteBuffer(data);
			flatBufferUserData = BanpoFri.Data.UserData.GetRootAsUserData(bb);
			ConnectReadOnlyDatas();
		}
		else
		{
			ChangeDataMode(DataState.Main);
		}
	}

	public void Save(bool Immediately = false)
	{
		if (Immediately)
		{
			SaveFile();
			return;
		}
		if (saving)
		{
			deltaTime = 0f;
			return;
		}

		saving = true;
		deltaTime = 0f;
	}

	private void SaveFile()
	{
		TpLog.Log("save file");
		var builder = new FlatBufferBuilder(1);
		int dataIdx = 0;
		var money = builder.CreateString(mainData.Money.Value.ToString());
		var tutorial = builder.CreateString(string.Join(";", Tutorial));


		var option = BanpoFri.Data.OptionData.CreateOptionData(builder, builder.CreateString(Language.ToString()), Bgm, Effect, SlowGraphic, Vib, SubscribeOrder, AutoFelling);

		Offset<BanpoFri.Data.RecordCount>[] recordCount = null;
		if (RecordCount.Count > 0)
		{
			recordCount = new Offset<BanpoFri.Data.RecordCount>[RecordCount.Count];
			dataIdx = 0;
			foreach (var rc in RecordCount)
			{
				recordCount[dataIdx++] = BanpoFri.Data.RecordCount.CreateRecordCount(builder, builder.CreateString(rc.Key), rc.Value);
			}
		}
		VectorOffset recordCountVec = default(VectorOffset);
		if (recordCount != null)
			recordCountVec = BanpoFri.Data.UserData.CreateRecordcountVector(builder, recordCount);




		//facilitydata
		Offset<BanpoFri.Data.facilityidata>[] facilitydatas = null;

		facilitydatas = new Offset<BanpoFri.Data.facilityidata>[mainData.StageData.StageFacilityDataList.Count];

		dataIdx = 0;

		foreach (var facility in mainData.StageData.StageFacilityDataList)
		{
			var facilitymoney = builder.CreateString(facility.MoneyCount.ToString());

			facilitydatas[dataIdx++] = BanpoFri.Data.facilityidata.Createfacilityidata(builder, facility.FacilityIdx, facilitymoney, facility.IsOpen, facility.CapacityCountProperty.Value);
		}

		var facilitydatavec = BanpoFri.Data.StageData.CreateFacilitydatasVector(builder, facilitydatas);

		var stagedata = BanpoFri.Data.StageData.CreateStageData(builder,
			mainData.StageData.NextFacilityOpenOrderProperty.Value, mainData.StageData.StageIdx
			, facilitydatavec);


		//facilyupgrade
		Offset<BanpoFri.Data.FacilityUpgradeData>[] facilyupgradedatas = null;

		facilyupgradedatas = new Offset<BanpoFri.Data.FacilityUpgradeData>
		[mainData.FishUpgradeDatas.Count];

		dataIdx = 0;

		foreach (var upgrade in mainData.FishUpgradeDatas)
		{
			facilyupgradedatas[dataIdx++] = BanpoFri.Data.FacilityUpgradeData.
			CreateFacilityUpgradeData(builder, upgrade.Level, upgrade.FishIdx);
		}

		var faciltyupgradedata = BanpoFri.Data.UserData.CreateFacilityupgradedatasVector(builder, facilyupgradedatas);


		//upgradedata
		Offset<BanpoFri.Data.UpgradeData>[] upgradedatas = null;

		upgradedatas = new Offset<BanpoFri.Data.UpgradeData>[mainData.UpgradeGroupData.StageUpgradeCollectionList.Count];

		dataIdx = 0;

		foreach (var upgrade in mainData.UpgradeGroupData.StageUpgradeCollectionList)
		{
			upgradedatas[dataIdx++] = BanpoFri.Data.UpgradeData.CreateUpgradeData(builder, upgrade.UpgradeIdx, upgrade.UpgradeType, upgrade.StageIdx, upgrade.IsBuyCheckProperty.Value);
		}

		var upgradedata = BanpoFri.Data.UserData.CreateUpgradedatasVector(builder, upgradedatas);


		//insert start
		BanpoFri.Data.UserData.StartUserData(builder);
		BanpoFri.Data.UserData.AddStagedata(builder, stagedata);
		BanpoFri.Data.UserData.AddLastlogintime(builder, mainData.LastLoginTime.Ticks);
		BanpoFri.Data.UserData.AddMoney(builder, money);
		BanpoFri.Data.UserData.AddCurplaydatetime(builder, mainData.CurPlayDateTime.Ticks);
		BanpoFri.Data.UserData.AddCash(builder, Cash.Value);
		BanpoFri.Data.UserData.AddUpgradedatas(builder, upgradedata);
		BanpoFri.Data.UserData.AddRecordcount(builder, recordCountVec);
		BanpoFri.Data.UserData.AddBoosttime(builder, mainData.BoostTime.Value);
		BanpoFri.Data.UserData.AddFacilityupgradedatas(builder, faciltyupgradedata);
		BanpoFri.Data.UserData.AddTutorial(builder, tutorial);
		BanpoFri.Data.UserData.AddOptiondata(builder , option);

		//end 
		var orc = BanpoFri.Data.UserData.EndUserData(builder);


		builder.Finish(orc.Value);

		var dataBuf = builder.DataBuffer;
		flatBufferUserData = BanpoFri.Data.UserData.GetRootAsUserData(dataBuf);

		var filePath = GetSaveFilePath();
		using (var ms = new MemoryStream(flatBufferUserData.ByteBuffer.ToFullArray(), dataBuf.Position, builder.Offset))
		{
			File.WriteAllBytes(filePath, ms.ToArray());
		}
	}


	public void ChangeDataMode(DataState state)
	{
		if (state == DataState)
			return;

		TpLog.Log($"ChangeDataMode:{state.ToString()}");
		switch (state)
		{
			case DataState.Main:
				CurMode = mainData;
				break;
			case DataState.Event:
				CurMode = eventData;
				break;
		}

		DataState = state;
	}
}
