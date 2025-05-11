using System;
using System.Collections.Generic;
using UniRx;
using Google.FlatBuffers;

public partial class UserDataSystem
{
    public PlayerData Playerdata { get; private set; } = new PlayerData();
    private void SaveData_PlayerData(FlatBufferBuilder builder)
    {
        // 선언된 변수들은 모두 저장되어야함

        // Playerdata 단일 저장
        // Playerdata 최종 생성 및 추가
        var playerdata_Offset = BanpoFri.Data.PlayerData.CreatePlayerData(
            builder,
            Playerdata.Hpcount.Value
        );


        Action cbAddDatas = () => {
            BanpoFri.Data.UserData.AddPlayerdata(builder, playerdata_Offset);
        };

        cb_SaveAddDatas += cbAddDatas;

    }
    private void LoadData_PlayerData()
    {
        // 로드 함수 내용

        // Playerdata 로드
        var fb_Playerdata = flatBufferUserData.Playerdata;
        if (fb_Playerdata.HasValue)
        {
            Playerdata.Hpcount.Value = fb_Playerdata.Value.Hpcount;
        }
    }

}

public class PlayerData
{
    public IReactiveProperty<int> Hpcount { get; set; } = new ReactiveProperty<int>(0);

}
