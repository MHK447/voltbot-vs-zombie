using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class StageInfoData
    {
        [SerializeField]
		private int _stageidx;
		public int stageidx
		{
			get { return _stageidx;}
			set { _stageidx = value;}
		}
		[SerializeField]
		private int _seedmoney_value;
		public int seedmoney_value
		{
			get { return _seedmoney_value;}
			set { _seedmoney_value = value;}
		}
		[SerializeField]
		private string _stage_name;
		public string stage_name
		{
			get { return _stage_name;}
			set { _stage_name = value;}
		}
		[SerializeField]
		private int _start_consumer_count;
		public int start_consumer_count
		{
			get { return _start_consumer_count;}
			set { _start_consumer_count = value;}
		}
		[SerializeField]
		private int _consumerfirst_idx;
		public int consumerfirst_idx
		{
			get { return _consumerfirst_idx;}
			set { _consumerfirst_idx = value;}
		}
		[SerializeField]
		private int _revenue_buff_profit;
		public int revenue_buff_profit
		{
			get { return _revenue_buff_profit;}
			set { _revenue_buff_profit = value;}
		}
		[SerializeField]
		private string _nextstage_name;
		public string nextstage_name
		{
			get { return _nextstage_name;}
			set { _nextstage_name = value;}
		}
		[SerializeField]
		private string _nextstage_image;
		public string nextstage_image
		{
			get { return _nextstage_image;}
			set { _nextstage_image = value;}
		}
		[SerializeField]
		private byte[] _next_stage_money;
		public System.Numerics.BigInteger next_stage_money
		{
			get { return new System.Numerics.BigInteger(_next_stage_money);}
			set { _next_stage_money = value.ToByteArray();}
		}

    }

    [System.Serializable]
    public class StageInfo : Table<StageInfoData, int>
    {
    }
}

