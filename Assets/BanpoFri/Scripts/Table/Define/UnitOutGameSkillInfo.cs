using UnityEngine;
using System.Collections.Generic;

namespace BanpoFri
{
    [System.Serializable]
    public class UnitOutGameSkillInfoData
    {
        [SerializeField]
		private int _skill_idx;
		public int skill_idx
		{
			get { return _skill_idx;}
			set { _skill_idx = value;}
		}
		[SerializeField]
		private string _name;
		public string name
		{
			get { return _name;}
			set { _name = value;}
		}
		[SerializeField]
		private string _ingame_select_name;
		public string ingame_select_name
		{
			get { return _ingame_select_name;}
			set { _ingame_select_name = value;}
		}
		[SerializeField]
		private string _ingame_select_desc;
		public string ingame_select_desc
		{
			get { return _ingame_select_desc;}
			set { _ingame_select_desc = value;}
		}

    }

    [System.Serializable]
    public class UnitOutGameSkillInfo : Table<UnitOutGameSkillInfoData, int>
    {
    }
}

