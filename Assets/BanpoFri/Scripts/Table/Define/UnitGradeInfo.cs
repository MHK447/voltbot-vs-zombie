using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class UnitGradeInfoData
    {
        [SerializeField]
		private int _level;
		public int level
		{
			get { return _level;}
			set { _level = value;}
		}
		[SerializeField]
		private List<int> _gradepercent;
		public List<int> gradepercent
		{
			get { return _gradepercent;}
			set { _gradepercent = value;}
		}

    }

    [System.Serializable]
    public class UnitGradeInfo : Table<UnitGradeInfoData, int>
    {
    }
}

