using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class GachaUnitCardInfoData
    {
        [SerializeField]
		private int _grade_info;
		public int grade_info
		{
			get { return _grade_info;}
			set { _grade_info = value;}
		}
		[SerializeField]
		private int _cost_value;
		public int cost_value
		{
			get { return _cost_value;}
			set { _cost_value = value;}
		}
		[SerializeField]
		private int _probability_value;
		public int probability_value
		{
			get { return _probability_value;}
			set { _probability_value = value;}
		}

    }

    [System.Serializable]
    public class GachaUnitCardInfo : Table<GachaUnitCardInfoData, int>
    {
    }
}

