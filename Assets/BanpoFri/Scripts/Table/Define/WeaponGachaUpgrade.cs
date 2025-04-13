using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class WeaponGachaUpgradeData
    {
        [SerializeField]
		private int _weapon_idx;
		public int weapon_idx
		{
			get { return _weapon_idx;}
			set { _weapon_idx = value;}
		}
		[SerializeField]
		private int _level;
		public int level
		{
			get { return _level;}
			set { _level = value;}
		}
		[SerializeField]
		private int _upgrade_type;
		public int upgrade_type
		{
			get { return _upgrade_type;}
			set { _upgrade_type = value;}
		}
		[SerializeField]
		private int _upgrade_value;
		public int upgrade_value
		{
			get { return _upgrade_value;}
			set { _upgrade_value = value;}
		}

    }

    [System.Serializable]
    public class WeaponGachaUpgrade : Table<WeaponGachaUpgradeData, KeyValuePair<int,int>>
    {
    }
}

