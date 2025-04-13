using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class StageWaveInfoData
    {
        [SerializeField]
		private int _wave_idx;
		public int wave_idx
		{
			get { return _wave_idx;}
			set { _wave_idx = value;}
		}
		[SerializeField]
		private int _wave_count;
		public int wave_count
		{
			get { return _wave_count;}
			set { _wave_count = value;}
		}
		[SerializeField]
		private int _unit_delaytime;
		public int unit_delaytime
		{
			get { return _unit_delaytime;}
			set { _unit_delaytime = value;}
		}
		[SerializeField]
		private int _spawn_cooltime;
		public int spawn_cooltime
		{
			get { return _spawn_cooltime;}
			set { _spawn_cooltime = value;}
		}
		[SerializeField]
		private int _unit_idx;
		public int unit_idx
		{
			get { return _unit_idx;}
			set { _unit_idx = value;}
		}
		[SerializeField]
		private int _boss_idx;
		public int boss_idx
		{
			get { return _boss_idx;}
			set { _boss_idx = value;}
		}
		[SerializeField]
		private int _hp_buff_value;
		public int hp_buff_value
		{
			get { return _hp_buff_value;}
			set { _hp_buff_value = value;}
		}

    }

    [System.Serializable]
    public class StageWaveInfo : Table<StageWaveInfoData, int>
    {
    }
}

