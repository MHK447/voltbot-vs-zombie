using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class SelectWeaponGachaSkilInfoData
    {
        [SerializeField]
		private int _skill_idx;
		public int skill_idx
		{
			get { return _skill_idx;}
			set { _skill_idx = value;}
		}
		[SerializeField]
		private int _select_type;
		public int select_type
		{
			get { return _select_type;}
			set { _select_type = value;}
		}
		[SerializeField]
		private int _value_1;
		public int value_1
		{
			get { return _value_1;}
			set { _value_1 = value;}
		}
		[SerializeField]
		private int _value_2;
		public int value_2
		{
			get { return _value_2;}
			set { _value_2 = value;}
		}
		[SerializeField]
		private int _level_buff_value;
		public int level_buff_value
		{
			get { return _level_buff_value;}
			set { _level_buff_value = value;}
		}
		[SerializeField]
		private string _icon;
		public string icon
		{
			get { return _icon;}
			set { _icon = value;}
		}
		[SerializeField]
		private string _desc_1;
		public string desc_1
		{
			get { return _desc_1;}
			set { _desc_1 = value;}
		}
		[SerializeField]
		private string _desc_2;
		public string desc_2
		{
			get { return _desc_2;}
			set { _desc_2 = value;}
		}
		[SerializeField]
		private int _instantuse;
		public int instantuse
		{
			get { return _instantuse;}
			set { _instantuse = value;}
		}
		[SerializeField]
		private int _desc_type;
		public int desc_type
		{
			get { return _desc_type;}
			set { _desc_type = value;}
		}

    }

    [System.Serializable]
    public class SelectWeaponGachaSkilInfo : Table<SelectWeaponGachaSkilInfoData, int>
    {
    }
}

