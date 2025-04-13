using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class FishInfoData
    {
        [SerializeField]
		private int _idx;
		public int idx
		{
			get { return _idx;}
			set { _idx = value;}
		}
		[SerializeField]
		private string _icon;
		public string icon
		{
			get { return _icon;}
			set { _icon = value;}
		}
		[SerializeField]
		private int _base_revenue;
		public int base_revenue
		{
			get { return _base_revenue;}
			set { _base_revenue = value;}
		}
		[SerializeField]
		private int _living_type;
		public int living_type
		{
			get { return _living_type;}
			set { _living_type = value;}
		}
		[SerializeField]
		private int _fish_rack_idx;
		public int fish_rack_idx
		{
			get { return _fish_rack_idx;}
			set { _fish_rack_idx = value;}
		}
		[SerializeField]
		private int _fish_facility_idx;
		public int fish_facility_idx
		{
			get { return _fish_facility_idx;}
			set { _fish_facility_idx = value;}
		}

    }

    [System.Serializable]
    public class FishInfo : Table<FishInfoData, int>
    {
    }
}

