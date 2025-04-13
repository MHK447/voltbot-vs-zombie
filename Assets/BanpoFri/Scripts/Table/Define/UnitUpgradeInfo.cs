using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class UnitUpgradeInfoData
    {
        [SerializeField]
		private int _upgrade_idx;
		public int upgrade_idx
		{
			get { return _upgrade_idx;}
			set { _upgrade_idx = value;}
		}
		[SerializeField]
		private int _upgrade_cost;
		public int upgrade_cost
		{
			get { return _upgrade_cost;}
			set { _upgrade_cost = value;}
		}
		[SerializeField]
		private int _cost_type;
		public int cost_type
		{
			get { return _cost_type;}
			set { _cost_type = value;}
		}
		[SerializeField]
		private int _cost_idx;
		public int cost_idx
		{
			get { return _cost_idx;}
			set { _cost_idx = value;}
		}
		[SerializeField]
		private int _cost_value;
		public int cost_value
		{
			get { return _cost_value;}
			set { _cost_value = value;}
		}
		[SerializeField]
		private int _upgrade_incrase;
		public int upgrade_incrase
		{
			get { return _upgrade_incrase;}
			set { _upgrade_incrase = value;}
		}

    }

    [System.Serializable]
    public class UnitUpgradeInfo : Table<UnitUpgradeInfoData, int>
    {
    }
}

