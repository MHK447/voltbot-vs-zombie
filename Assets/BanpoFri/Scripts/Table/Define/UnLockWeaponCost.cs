using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class UnLockWeaponCostData
    {
        [SerializeField]
		private int _upgrade_count;
		public int upgrade_count
		{
			get { return _upgrade_count;}
			set { _upgrade_count = value;}
		}
		[SerializeField]
		private int _cost;
		public int cost
		{
			get { return _cost;}
			set { _cost = value;}
		}

    }

    [System.Serializable]
    public class UnLockWeaponCost : Table<UnLockWeaponCostData, int>
    {
    }
}

