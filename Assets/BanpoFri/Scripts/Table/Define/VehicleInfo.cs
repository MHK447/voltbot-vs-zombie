using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class VehicleInfoData
    {
        [SerializeField]
		private int _vehicle_idx;
		public int vehicle_idx
		{
			get { return _vehicle_idx;}
			set { _vehicle_idx = value;}
		}
		[SerializeField]
		private int _buff_value;
		public int buff_value
		{
			get { return _buff_value;}
			set { _buff_value = value;}
		}

    }

    [System.Serializable]
    public class VehicleInfo : Table<VehicleInfoData, int>
    {
    }
}

