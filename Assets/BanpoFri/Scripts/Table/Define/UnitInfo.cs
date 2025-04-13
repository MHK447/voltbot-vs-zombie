using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class UnitInfoData
    {
        [SerializeField]
		private int _unit_idx;
		public int unit_idx
		{
			get { return _unit_idx;}
			set { _unit_idx = value;}
		}
		[SerializeField]
		private int _unit_type;
		public int unit_type
		{
			get { return _unit_type;}
			set { _unit_type = value;}
		}
		[SerializeField]
		private int _hp;
		public int hp
		{
			get { return _hp;}
			set { _hp = value;}
		}
		[SerializeField]
		private int _movespeed;
		public int movespeed
		{
			get { return _movespeed;}
			set { _movespeed = value;}
		}
		[SerializeField]
		private int _attack;
		public int attack
		{
			get { return _attack;}
			set { _attack = value;}
		}
		[SerializeField]
		private int _attack_speed;
		public int attack_speed
		{
			get { return _attack_speed;}
			set { _attack_speed = value;}
		}
		[SerializeField]
		private string _critical_percent;
		public string critical_percent
		{
			get { return _critical_percent;}
			set { _critical_percent = value;}
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
		private string _star_image;
		public string star_image
		{
			get { return _star_image;}
			set { _star_image = value;}
		}

    }

    [System.Serializable]
    public class UnitInfo : Table<UnitInfoData, int>
    {
    }
}

