using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class RobotInfoData
    {
        [SerializeField]
		private int _product_idx;
		public int product_idx
		{
			get { return _product_idx;}
			set { _product_idx = value;}
		}
		[SerializeField]
		private int _base_move_speed;
		public int base_move_speed
		{
			get { return _base_move_speed;}
			set { _base_move_speed = value;}
		}
		[SerializeField]
		private int _base_attack_speed;
		public int base_attack_speed
		{
			get { return _base_attack_speed;}
			set { _base_attack_speed = value;}
		}
		[SerializeField]
		private int _base_attack_damage;
		public int base_attack_damage
		{
			get { return _base_attack_damage;}
			set { _base_attack_damage = value;}
		}
		[SerializeField]
		private int _base_hp;
		public int base_hp
		{
			get { return _base_hp;}
			set { _base_hp = value;}
		}
		[SerializeField]
		private string _prefab;
		public string prefab
		{
			get { return _prefab;}
			set { _prefab = value;}
		}

    }

    [System.Serializable]
    public class RobotInfo : Table<RobotInfoData, int>
    {
    }
}

