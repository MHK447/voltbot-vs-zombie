using System;
using System.Collections.Generic;
using UniRx;
using Google.FlatBuffers;

public partial class UserDataSystem
{
    public StageData Stagedata { get; private set; } = new StageData();
    private void SaveData_StageData(FlatBufferBuilder builder)
    {
        // 선언된 변수들은 모두 저장되어야함

        // Stagedata 단일 저장
        // Stagedata 최종 생성 및 추가
        var stagedata_Offset = BanpoFri.Data.StageData.CreateStageData(
            builder,
            Stagedata.Stageidx.Value
        );


        Action cbAddDatas = () => {
            BanpoFri.Data.UserData.AddStagedata(builder, stagedata_Offset);
        };

        cb_SaveAddDatas += cbAddDatas;

    }
    private void LoadData_StageData()
    {
        // 로드 함수 내용

        // Stagedata 로드
        var fb_Stagedata = flatBufferUserData.Stagedata;
        if (fb_Stagedata.HasValue)
        {
            Stagedata.Stageidx.Value = fb_Stagedata.Value.Stageidx;
        }
    }

}

public class StageData
{
    public IReactiveProperty<int> Stageidx { get; private set; } = new ReactiveProperty<int>(1);

}
