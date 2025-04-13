using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class OutGameUnitLevelinfoData
    {
        [SerializeField]
		private int _level;
		public int level
		{
			get { return _level;}
			set { _level = value;}
		}
		[SerializeField]
		private int _cardcount;
		public int cardcount
		{
			get { return _cardcount;}
			set { _cardcount = value;}
		}
		[SerializeField]
		private int _costvalue;
		public int costvalue
		{
			get { return _costvalue;}
			set { _costvalue = value;}
		}

    }

    [System.Serializable]
    public class OutGameUnitLevelinfo : Table<OutGameUnitLevelinfoData, int>
    {
    }
}

