// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace BanpoFri.Data
{

using global::System;
using global::System.Collections.Generic;
using global::FlatBuffers;

public struct UpgradeData : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_2_0_0(); }
  public static UpgradeData GetRootAsUpgradeData(ByteBuffer _bb) { return GetRootAsUpgradeData(_bb, new UpgradeData()); }
  public static UpgradeData GetRootAsUpgradeData(ByteBuffer _bb, UpgradeData obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public UpgradeData __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int Upgradeidx { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public bool MutateUpgradeidx(int upgradeidx) { int o = __p.__offset(4); if (o != 0) { __p.bb.PutInt(o + __p.bb_pos, upgradeidx); return true; } else { return false; } }
  public int Upgradetype { get { int o = __p.__offset(6); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public bool MutateUpgradetype(int upgradetype) { int o = __p.__offset(6); if (o != 0) { __p.bb.PutInt(o + __p.bb_pos, upgradetype); return true; } else { return false; } }
  public int Stageidx { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public bool MutateStageidx(int stageidx) { int o = __p.__offset(8); if (o != 0) { __p.bb.PutInt(o + __p.bb_pos, stageidx); return true; } else { return false; } }
  public bool Isbuycheck { get { int o = __p.__offset(10); return o != 0 ? 0!=__p.bb.Get(o + __p.bb_pos) : (bool)false; } }
  public bool MutateIsbuycheck(bool isbuycheck) { int o = __p.__offset(10); if (o != 0) { __p.bb.Put(o + __p.bb_pos, (byte)(isbuycheck ? 1 : 0)); return true; } else { return false; } }

  public static Offset<BanpoFri.Data.UpgradeData> CreateUpgradeData(FlatBufferBuilder builder,
      int upgradeidx = 0,
      int upgradetype = 0,
      int stageidx = 0,
      bool isbuycheck = false) {
    builder.StartTable(4);
    UpgradeData.AddStageidx(builder, stageidx);
    UpgradeData.AddUpgradetype(builder, upgradetype);
    UpgradeData.AddUpgradeidx(builder, upgradeidx);
    UpgradeData.AddIsbuycheck(builder, isbuycheck);
    return UpgradeData.EndUpgradeData(builder);
  }

  public static void StartUpgradeData(FlatBufferBuilder builder) { builder.StartTable(4); }
  public static void AddUpgradeidx(FlatBufferBuilder builder, int upgradeidx) { builder.AddInt(0, upgradeidx, 0); }
  public static void AddUpgradetype(FlatBufferBuilder builder, int upgradetype) { builder.AddInt(1, upgradetype, 0); }
  public static void AddStageidx(FlatBufferBuilder builder, int stageidx) { builder.AddInt(2, stageidx, 0); }
  public static void AddIsbuycheck(FlatBufferBuilder builder, bool isbuycheck) { builder.AddBool(3, isbuycheck, false); }
  public static Offset<BanpoFri.Data.UpgradeData> EndUpgradeData(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<BanpoFri.Data.UpgradeData>(o);
  }
};


}
