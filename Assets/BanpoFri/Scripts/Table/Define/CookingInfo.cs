using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class CookingInfoData
    {
        [SerializeField]
		private int _cookingidx;
		public int cookingidx
		{
			get { return _cookingidx;}
			set { _cookingidx = value;}
		}
		[SerializeField]
		private List<int> _material_idxs;
		public List<int> material_idxs
		{
			get { return _material_idxs;}
			set { _material_idxs = value;}
		}
		[SerializeField]
		private int _material_max_count;
		public int material_max_count
		{
			get { return _material_max_count;}
			set { _material_max_count = value;}
		}
		[SerializeField]
		private int _food_idx;
		public int food_idx
		{
			get { return _food_idx;}
			set { _food_idx = value;}
		}
		[SerializeField]
		private int _break_count;
		public int break_count
		{
			get { return _break_count;}
			set { _break_count = value;}
		}
		[SerializeField]
		private int _cooking_cooltime;
		public int cooking_cooltime
		{
			get { return _cooking_cooltime;}
			set { _cooking_cooltime = value;}
		}

    }

    [System.Serializable]
    public class CookingInfo : Table<CookingInfoData, int>
    {
    }
}

