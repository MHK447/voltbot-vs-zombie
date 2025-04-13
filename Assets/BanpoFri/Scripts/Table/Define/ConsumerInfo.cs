using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class ConsumerInfoData
    {
        [SerializeField]
		private int _idx;
		public int idx
		{
			get { return _idx;}
			set { _idx = value;}
		}
		[SerializeField]
		private string _prefab;
		public string prefab
		{
			get { return _prefab;}
			set { _prefab = value;}
		}
		[SerializeField]
		private int _speed;
		public int speed
		{
			get { return _speed;}
			set { _speed = value;}
		}
		[SerializeField]
		private string _skin;
		public string skin
		{
			get { return _skin;}
			set { _skin = value;}
		}

    }

    [System.Serializable]
    public class ConsumerInfo : Table<ConsumerInfoData, int>
    {
    }
}

