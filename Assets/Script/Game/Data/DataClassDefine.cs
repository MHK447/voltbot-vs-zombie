using System;
using System.Numerics;
using UniRx;
using System.Collections.Generic;
using UnityEngine;
using BanpoFri;
using System.Linq;
using BanpoFri.Data;

public interface IReadOnlyData : ICloneable
{
	void Create();
}
public interface IClientData { }


public class PlayerData
{

}

public class StageData
{

	public int StageIdx { get; set; } = 1;
}





