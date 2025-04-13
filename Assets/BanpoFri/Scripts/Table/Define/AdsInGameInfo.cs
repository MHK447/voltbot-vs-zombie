using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class AdsInGameInfoData
    {
        [SerializeField]
		private int _currency_idx;
		public int currency_idx
		{
			get { return _currency_idx;}
			set { _currency_idx = value;}
		}
		[SerializeField]
		private int _value_1;
		public int value_1
		{
			get { return _value_1;}
			set { _value_1 = value;}
		}
		[SerializeField]
		private int _multiple_value;
		public int multiple_value
		{
			get { return _multiple_value;}
			set { _multiple_value = value;}
		}

    }

    [System.Serializable]
    public class AdsInGameInfo : Table<AdsInGameInfoData, int>
    {
    }
}

