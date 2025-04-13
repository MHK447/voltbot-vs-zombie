using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class EnemyInfoData
    {
        [SerializeField]
		private int _unit_idx;
		public int unit_idx
		{
			get { return _unit_idx;}
			set { _unit_idx = value;}
		}
		[SerializeField]
		private int _movespeed;
		public int movespeed
		{
			get { return _movespeed;}
			set { _movespeed = value;}
		}
		[SerializeField]
		private string _prefab;
		public string prefab
		{
			get { return _prefab;}
			set { _prefab = value;}
		}
		[SerializeField]
		private string _image;
		public string image
		{
			get { return _image;}
			set { _image = value;}
		}
		[SerializeField]
		private string _name;
		public string name
		{
			get { return _name;}
			set { _name = value;}
		}
		[SerializeField]
		private int _basehp;
		public int basehp
		{
			get { return _basehp;}
			set { _basehp = value;}
		}

    }

    [System.Serializable]
    public class EnemyInfo : Table<EnemyInfoData, int>
    {
    }
}

