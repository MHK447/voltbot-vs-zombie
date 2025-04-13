using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class OutGameUnitUpgradeData
    {
        [SerializeField]
		private int _unit_idx;
		public int unit_idx
		{
			get { return _unit_idx;}
			set { _unit_idx = value;}
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
		[SerializeField]
		private int _skill_idx;
		public int skill_idx
		{
			get { return _skill_idx;}
			set { _skill_idx = value;}
		}
		[SerializeField]
		private int _skill_value;
		public int skill_value
		{
			get { return _skill_value;}
			set { _skill_value = value;}
		}
		[SerializeField]
		private int _skill_damage;
		public int skill_damage
		{
			get { return _skill_damage;}
			set { _skill_damage = value;}
		}
		[SerializeField]
		private int _debuff_type;
		public int debuff_type
		{
			get { return _debuff_type;}
			set { _debuff_type = value;}
		}

    }

    [System.Serializable]
    public class OutGameUnitUpgrade : Table<OutGameUnitUpgradeData, KeyValuePair<int,int>>
    {
    }
}

