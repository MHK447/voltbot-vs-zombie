using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class UpgradeInfoData
    {
        [SerializeField]
		private int _stageidx;
		public int stageidx
		{
			get { return _stageidx;}
			set { _stageidx = value;}
		}
		[SerializeField]
		private int _upgrade_idx;
		public int upgrade_idx
		{
			get { return _upgrade_idx;}
			set { _upgrade_idx = value;}
		}
		[SerializeField]
		private int _upgrade_type;
		public int upgrade_type
		{
			get { return _upgrade_type;}
			set { _upgrade_type = value;}
		}
		[SerializeField]
		private int _cost;
		public int cost
		{
			get { return _cost;}
			set { _cost = value;}
		}
		[SerializeField]
		private int _value;
		public int value
		{
			get { return _value;}
			set { _value = value;}
		}
		[SerializeField]
		private int _value2;
		public int value2
		{
			get { return _value2;}
			set { _value2 = value;}
		}
		[SerializeField]
		private string _icon;
		public string icon
		{
			get { return _icon;}
			set { _icon = value;}
		}
		[SerializeField]
		private string _desc;
		public string desc
		{
			get { return _desc;}
			set { _desc = value;}
		}
		[SerializeField]
		private string _name;
		public string name
		{
			get { return _name;}
			set { _name = value;}
		}

    }

    [System.Serializable]
    public class UpgradeInfo : Table<UpgradeInfoData, KeyValuePair<int,int>>
    {
    }
}

