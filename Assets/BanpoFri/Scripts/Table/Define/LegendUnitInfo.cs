using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class LegendUnitInfoData
    {
        [SerializeField]
		private int _unit_idx;
		public int unit_idx
		{
			get { return _unit_idx;}
			set { _unit_idx = value;}
		}
		[SerializeField]
		private List<int> _combine_unit;
		public List<int> combine_unit
		{
			get { return _combine_unit;}
			set { _combine_unit = value;}
		}

    }

    [System.Serializable]
    public class LegendUnitInfo : Table<LegendUnitInfoData, int>
    {
    }
}

