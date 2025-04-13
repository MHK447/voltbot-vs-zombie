using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class UnitSellInfoData
    {
        [SerializeField]
		private int _grade;
		public int grade
		{
			get { return _grade;}
			set { _grade = value;}
		}
		[SerializeField]
		private int _reward_type;
		public int reward_type
		{
			get { return _reward_type;}
			set { _reward_type = value;}
		}
		[SerializeField]
		private int _reward_idx;
		public int reward_idx
		{
			get { return _reward_idx;}
			set { _reward_idx = value;}
		}
		[SerializeField]
		private int _reward_value;
		public int reward_value
		{
			get { return _reward_value;}
			set { _reward_value = value;}
		}

    }

    [System.Serializable]
    public class UnitSellInfo : Table<UnitSellInfoData, int>
    {
    }
}

