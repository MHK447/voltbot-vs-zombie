using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class StageInfoData
    {
        [SerializeField]
		private int _stage_idx;
		public int stage_idx
		{
			get { return _stage_idx;}
			set { _stage_idx = value;}
		}
		[SerializeField]
		private string _prefab;
		public string prefab
		{
			get { return _prefab;}
			set { _prefab = value;}
		}
		[SerializeField]
		private int _enemy_increase_hp;
		public int enemy_increase_hp
		{
			get { return _enemy_increase_hp;}
			set { _enemy_increase_hp = value;}
		}
		[SerializeField]
		private int _enemy_increase_attack;
		public int enemy_increase_attack
		{
			get { return _enemy_increase_attack;}
			set { _enemy_increase_attack = value;}
		}

    }

    [System.Serializable]
    public class StageInfo : Table<StageInfoData, int>
    {
    }
}

