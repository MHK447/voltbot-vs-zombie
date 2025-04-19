using System;
using System.Numerics;
using UniRx;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using BanpoFri.Data;

public interface IReadOnlyData : ICloneable
{
	void Create();
}
public interface IClientData { }


public class FacilityData
{
	public bool IsOpen = false;

	public int FacilityIdx = 0;

	public System.Numerics.BigInteger MoneyCount = 0;

	public IReactiveProperty<int> CapacityCountProperty = new ReactiveProperty<int>(0);

	public FacilityData(int facilityidx, System.Numerics.BigInteger moneycount, bool isopen, int capacitycount)
	{
		IsOpen = isopen;
		FacilityIdx = facilityidx;
		MoneyCount = moneycount;
		CapacityCountProperty.Value = capacitycount;

	}

}

public class UpgradeGroupData
{
	public IReactiveCollection<UpgradeData> StageUpgradeCollectionList = new ReactiveCollection<UpgradeData>();


	public UpgradeData FindUpgradeData(int upgradeidx)
	{
		var curstageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

		var finddata = StageUpgradeCollectionList.ToList().Find(x => x.UpgradeIdx == upgradeidx);

		if (finddata != null)
		{
			return finddata;
		}
		else
		{
			var td = Tables.Instance.GetTable<UpgradeInfo>().GetData(new KeyValuePair<int, int>(curstageidx, upgradeidx));

			if (td != null)
			{
				var newupgradedata = new UpgradeData(upgradeidx, td.upgrade_type, td.stageidx, false);

				StageUpgradeCollectionList.Add(newupgradedata);

				return newupgradedata;
			}
		}

		return null;
	}
}


public class UpgradeData
{
	public int UpgradeIdx = 0;

	public int UpgradeType = 0;

	public int StageIdx = 0;

	public IReactiveProperty<bool> IsBuyCheckProperty = new ReactiveProperty<bool>();

	public UpgradeData(int upgradeidx, int upgradetype, int stageidx, bool isbuy)
	{
		UpgradeIdx = upgradeidx;
		UpgradeType = upgradetype;
		StageIdx = stageidx;
		IsBuyCheckProperty.Value = isbuy;
	}

	public void UpgradeGet()
	{
		IsBuyCheckProperty.Value = true;

	}
}


public class StageFishUpgradeData
{
	public int FishIdx = 0;
	public int Level = 0;

	public StageFishUpgradeData(int fishidx, int level)
	{
		FishIdx = fishidx;
		Level = level;
	}
}

public class NoticeData
{
	public int NotiIdx = 0;
	public Transform Target;

	public NoticeData(int notiidx , Transform target)
	{
		NotiIdx = notiidx;
		Target = target;
	}

}

public class PlayerData
{
	public IReactiveProperty<int> VehiclePropertyIdx = new ReactiveProperty<int>();
	


}

public class StageData
{
	public List<FacilityData> StageFacilityDataList = new List<FacilityData>();

	public int StageIdx { get; set; } = 1;
	public IReactiveProperty<int> NextFacilityOpenOrderProperty = new ReactiveProperty<int>();

	public FacilityData FindFacilityData(int facilityidx)
	{
		var finddata = StageFacilityDataList.Find(x => x.FacilityIdx == facilityidx);

		if (finddata != null)
		{
			return finddata;
		}
		else
		{
			var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

			var newfacilitydata = new FacilityData(facilityidx, 0, false, 0);

			StageFacilityDataList.Add(newfacilitydata);

			return newfacilitydata;
		}
	}


	public FacilityData FindFishFacilityData(int fishhidx)
	{
		var facilitytd = Tables.Instance.GetTable<FishInfo>().GetData(fishhidx);

		if (facilitytd != null)
		{
			var finddata = StageFacilityDataList.Find(x => x.FacilityIdx == facilitytd.fish_facility_idx);
			if (finddata != null)
			{
				return finddata;
			}
			else
			{
				var stageidx = GameRoot.Instance.UserData.CurMode.StageData.StageIdx;

				var newfacilitydata = new FacilityData(fishhidx, 0, false, 0);

				StageFacilityDataList.Add(newfacilitydata);

				return newfacilitydata;
			}
		}

		return null;
	}



	public void SetStageIdx(int idx)
	{
		StageFacilityDataList.Clear();
		NextFacilityOpenOrderProperty.Value = 0;
		StageIdx = idx;
	}

	public void StageEndClear()
	{

	}


	public void SetWave(int waveidx)
	{
	}
}





