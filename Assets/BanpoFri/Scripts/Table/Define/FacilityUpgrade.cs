using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class FacilityUpgradeData
    {
        [SerializeField]
		private int _stageidx;
		public int stageidx
		{
			get { return _stageidx;}
			set { _stageidx = value;}
		}
		[SerializeField]
		private int _facilityidx;
		public int facilityidx
		{
			get { return _facilityidx;}
			set { _facilityidx = value;}
		}
		[SerializeField]
		private int _upgrade_reward_idx;
		public int upgrade_reward_idx
		{
			get { return _upgrade_reward_idx;}
			set { _upgrade_reward_idx = value;}
		}
		[SerializeField]
		private int _upgrade_reward_count;
		public int upgrade_reward_count
		{
			get { return _upgrade_reward_count;}
			set { _upgrade_reward_count = value;}
		}
		[SerializeField]
		private int _value_count;
		public int value_count
		{
			get { return _value_count;}
			set { _value_count = value;}
		}
		[SerializeField]
		private int _income_multiple_value_group;
		public int income_multiple_value_group
		{
			get { return _income_multiple_value_group;}
			set { _income_multiple_value_group = value;}
		}
		[SerializeField]
		private int _income_multiple_value;
		public int income_multiple_value
		{
			get { return _income_multiple_value;}
			set { _income_multiple_value = value;}
		}
		[SerializeField]
		private int _max_ugprade_count;
		public int max_ugprade_count
		{
			get { return _max_ugprade_count;}
			set { _max_ugprade_count = value;}
		}
		[SerializeField]
		private int _base_income_cost;
		public int base_income_cost
		{
			get { return _base_income_cost;}
			set { _base_income_cost = value;}
		}
		[SerializeField]
		private int _income_cost_multiple;
		public int income_cost_multiple
		{
			get { return _income_cost_multiple;}
			set { _income_cost_multiple = value;}
		}
		[SerializeField]
		private int _income_cost_level_multiple;
		public int income_cost_level_multiple
		{
			get { return _income_cost_level_multiple;}
			set { _income_cost_level_multiple = value;}
		}
		[SerializeField]
		private int _openfacilitycheck;
		public int openfacilitycheck
		{
			get { return _openfacilitycheck;}
			set { _openfacilitycheck = value;}
		}

    }

    [System.Serializable]
    public class FacilityUpgrade : Table<FacilityUpgradeData, KeyValuePair<int,int>>
    {
    }
}

