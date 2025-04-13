using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class StageFacilityInfoData
    {
        [SerializeField]
		private int _stageidx;
		public int stageidx
		{
			get { return _stageidx;}
			set { _stageidx = value;}
		}
		[SerializeField]
		private int _openorder;
		public int openorder
		{
			get { return _openorder;}
			set { _openorder = value;}
		}
		[SerializeField]
		private int _facilityidx;
		public int facilityidx
		{
			get { return _facilityidx;}
			set { _facilityidx = value;}
		}
		[SerializeField]
		private int _open_cost;
		public int open_cost
		{
			get { return _open_cost;}
			set { _open_cost = value;}
		}

    }

    [System.Serializable]
    public class StageFacilityInfo : Table<StageFacilityInfoData, KeyValuePair<int,int>>
    {
    }
}

