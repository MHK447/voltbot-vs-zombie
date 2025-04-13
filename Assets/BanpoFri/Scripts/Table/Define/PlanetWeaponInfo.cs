using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class PlanetWeaponInfoData
    {
        [SerializeField]
		private int _weapon_idx;
		public int weapon_idx
		{
			get { return _weapon_idx;}
			set { _weapon_idx = value;}
		}
		[SerializeField]
		private int _weapon_type;
		public int weapon_type
		{
			get { return _weapon_type;}
			set { _weapon_type = value;}
		}
		[SerializeField]
		private int _ability_base;
		public int ability_base
		{
			get { return _ability_base;}
			set { _ability_base = value;}
		}
		[SerializeField]
		private int _ability_duration;
		public int ability_duration
		{
			get { return _ability_duration;}
			set { _ability_duration = value;}
		}
		[SerializeField]
		private int _ability_value;
		public int ability_value
		{
			get { return _ability_value;}
			set { _ability_value = value;}
		}
		[SerializeField]
		private int _base_attack;
		public int base_attack
		{
			get { return _base_attack;}
			set { _base_attack = value;}
		}
		[SerializeField]
		private int _base_attack_regen;
		public int base_attack_regen
		{
			get { return _base_attack_regen;}
			set { _base_attack_regen = value;}
		}
		[SerializeField]
		private int _base_attack_speed;
		public int base_attack_speed
		{
			get { return _base_attack_speed;}
			set { _base_attack_speed = value;}
		}
		[SerializeField]
		private int _baase_critical_percent;
		public int baase_critical_percent
		{
			get { return _baase_critical_percent;}
			set { _baase_critical_percent = value;}
		}
		[SerializeField]
		private int _base_critical_damage;
		public int base_critical_damage
		{
			get { return _base_critical_damage;}
			set { _base_critical_damage = value;}
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
		private string _name_desc;
		public string name_desc
		{
			get { return _name_desc;}
			set { _name_desc = value;}
		}
		[SerializeField]
		private int _ColSizeX;
		public int ColSizeX
		{
			get { return _ColSizeX;}
			set { _ColSizeX = value;}
		}
		[SerializeField]
		private int _ColSizeY;
		public int ColSizeY
		{
			get { return _ColSizeY;}
			set { _ColSizeY = value;}
		}
		[SerializeField]
		private int _attack_speed;
		public int attack_speed
		{
			get { return _attack_speed;}
			set { _attack_speed = value;}
		}

    }

    [System.Serializable]
    public class PlanetWeaponInfo : Table<PlanetWeaponInfoData, int>
    {
    }
}

