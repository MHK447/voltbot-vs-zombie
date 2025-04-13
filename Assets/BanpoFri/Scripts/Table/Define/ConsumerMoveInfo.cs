using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class ConsumerMoveInfoData
    {
        [SerializeField]
		private int _stageidx;
		public int stageidx
		{
			get { return _stageidx;}
			set { _stageidx = value;}
		}
		[SerializeField]
		private int _order;
		public int order
		{
			get { return _order;}
			set { _order = value;}
		}
		[SerializeField]
		private List<int> _facilityidx;
		public List<int> facilityidx
		{
			get { return _facilityidx;}
			set { _facilityidx = value;}
		}
		[SerializeField]
		private List<int> _count;
		public List<int> count
		{
			get { return _count;}
			set { _count = value;}
		}

    }

    [System.Serializable]
    public class ConsumerMoveInfo : Table<ConsumerMoveInfoData, KeyValuePair<int,int>>
    {
    }
}

