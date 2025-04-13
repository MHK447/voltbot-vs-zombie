using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class StageCardDrawInfoData
    {
        [SerializeField]
		private int _stage_idx;
		public int stage_idx
		{
			get { return _stage_idx;}
			set { _stage_idx = value;}
		}
		[SerializeField]
		private List<int> _unitdead_count;
		public List<int> unitdead_count
		{
			get { return _unitdead_count;}
			set { _unitdead_count = value;}
		}

    }

    [System.Serializable]
    public class StageCardDrawInfo : Table<StageCardDrawInfoData, int>
    {
    }
}

