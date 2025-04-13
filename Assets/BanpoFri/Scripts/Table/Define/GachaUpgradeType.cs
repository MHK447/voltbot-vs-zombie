using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class GachaUpgradeTypeData
    {
        [SerializeField]
		private int _upgrade_type;
		public int upgrade_type
		{
			get { return _upgrade_type;}
			set { _upgrade_type = value;}
		}
		[SerializeField]
		private string _desc;
		public string desc
		{
			get { return _desc;}
			set { _desc = value;}
		}

    }

    [System.Serializable]
    public class GachaUpgradeType : Table<GachaUpgradeTypeData, int>
    {
    }
}

