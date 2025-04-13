using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class OutGameGachaGradeInfoData
    {
        [SerializeField]
		private int _grade;
		public int grade
		{
			get { return _grade;}
			set { _grade = value;}
		}
		[SerializeField]
		private int _ratio;
		public int ratio
		{
			get { return _ratio;}
			set { _ratio = value;}
		}

    }

    [System.Serializable]
    public class OutGameGachaGradeInfo : Table<OutGameGachaGradeInfoData, int>
    {
    }
}

