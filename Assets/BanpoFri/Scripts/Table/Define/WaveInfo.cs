using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class WaveInfoData
    {
        [SerializeField]
		private int _stageidx;
		public int stageidx
		{
			get { return _stageidx;}
			set { _stageidx = value;}
		}
		[SerializeField]
		private int _waveidx;
		public int waveidx
		{
			get { return _waveidx;}
			set { _waveidx = value;}
		}
		[SerializeField]
		private List<int> _enemy_idx;
		public List<int> enemy_idx
		{
			get { return _enemy_idx;}
			set { _enemy_idx = value;}
		}
		[SerializeField]
		private List<int> _count;
		public List<int> count
		{
			get { return _count;}
			set { _count = value;}
		}
		[SerializeField]
		private List<int> _time;
		public List<int> time
		{
			get { return _time;}
			set { _time = value;}
		}

    }

    [System.Serializable]
    public class WaveInfo : Table<WaveInfoData, KeyValuePair<int,int>>
    {
    }
}

