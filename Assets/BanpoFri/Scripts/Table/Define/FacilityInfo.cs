using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class FacilityInfoData
    {
        [SerializeField]
		private int _facilityidx;
		public int facilityidx
		{
			get { return _facilityidx;}
			set { _facilityidx = value;}
		}
		[SerializeField]
		private string _image;
		public string image
		{
			get { return _image;}
			set { _image = value;}
		}
		[SerializeField]
		private int _facility_type;
		public int facility_type
		{
			get { return _facility_type;}
			set { _facility_type = value;}
		}
		[SerializeField]
		private int _start_capacity;
		public int start_capacity
		{
			get { return _start_capacity;}
			set { _start_capacity = value;}
		}
		[SerializeField]
		private int _fish_idx;
		public int fish_idx
		{
			get { return _fish_idx;}
			set { _fish_idx = value;}
		}
		[SerializeField]
		private int _value_1;
		public int value_1
		{
			get { return _value_1;}
			set { _value_1 = value;}
		}
		[SerializeField]
		private int _rack_group;
		public int rack_group
		{
			get { return _rack_group;}
			set { _rack_group = value;}
		}
		[SerializeField]
		private int _cooking_group;
		public int cooking_group
		{
			get { return _cooking_group;}
			set { _cooking_group = value;}
		}

    }

    [System.Serializable]
    public class FacilityInfo : Table<FacilityInfoData, int>
    {
    }
}

