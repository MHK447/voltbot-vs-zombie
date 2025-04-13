using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.Linq;
using BanpoFri;

[System.Serializable]
public class Config : BanpoFri.SingletonScriptableObject<Config>, BanpoFri.ILoader
{

    public enum LandCondination
    {
        Great,
        Basic,
        Sad,
    }

    public enum Language
    {
        en = 0,
        ko = 1,
        ja = 2,
    }

    public enum InGameUpgradeIdx
    {
        ATTACK,
        ATTACKSPEED,
        ATTACKRANGE,
        ATTACKREGEN,
        HP,
        HPREGEN,
        CRITICALPERCENT,
        CRITICALMULTIPLE,
    }

    public enum LABUpgradeIdx
    {
        ATTACK,
        ATTACKREGEN,
        ATTACKRANGE,
        HP,
        HPREGEN,
        CRITICALPERCENT,
        CRITICALDAMAGE,
    }

    public enum WeaponUpgradeIdx
    {
        ATTACK = 0,
        ATTACKSPEED = 1,
        ATTACKRANGE = 2,
        ATTACKREGEN = 3,
        CRITICALPERCENT = 6,
        CRITICALDAMAGE = 7,
        DONE,
    }

    public enum WeaponType
    {
        Base = 1,
        TrackEnemy,
    }

    public enum CurrencyID
    {
        Money = 1,
        Cash = 2,
        EnergyMoney = 3,
        GachaCoin = 4,
    }


    public enum RewardType
    {
        Currency = 1,
    }

    public enum FacilityTypeIdx
    {
        None,

        RedSnapperDisplay = 1,      // 붉은열기 진열대 1
        MackerelDisplay = 2,        // 고등어 진열대 2
        TropicalFishDisplay = 3,    // 열대어 진열대 3
        CrabDisplay = 4,            // 꽃게 진열대 4
        GrilledRedSnapperDisplay = 5, // 붉은열기 구이 진열대 5
        SnapperRamenDisplay = 6,    // 열기 라면 진열대 6
        TropicalFishCurryDisplay = 7, // 열대어 카레 진열대 7
        GrilledCrabDisplay = 8,     // 크랩 구이 진열대 8
        CrabCurryDisplay = 9,       // 크랩 카레 진열대 9
        AppleDisplay = 10,       // 사과 진열대
        OrangeDisplay = 11,       // 오렌지 진열대
        CuttingAppleDisplay = 12,       // 오렌지 진열대
        CakeDisplay = 13,       // 오렌지 진열대
        SquidDisplay = 14, //오징어
        CodDisplay = 15, //대구 
        GrilledSquidDisplay = 16, //오징어 구이
        SteamedCodDisplay = 17, //대구 찜
        SquidRiceBowlDisplay = 18, // 오징어 덮밥
        SeafoodStewDisplay = 19, // 해믈스튜
        SeafoodRisottoDisplay = 20, //해물리조또 

        RedStarfishDisplay = 21, // 빨간 불가사리 진열대
        ShrimpDisplay = 22, // 새우 진열대
        OctopusDisplay = 23, // 문어 진열대
        MushroomDisplay = 24, // 버섯 진열대
        FriedStarfishDisplay = 25, // 불가사리 튀김 진열대
        AssortedFriesDisplay = 26, // 모듬 튀김 진열대
        SeafoodCutletDisplay = 27, // 해물까스 진열대
        FriedRiceDisplay = 28, // 튀김 덮밥 진열대

        CheckoutCounter = 1000,       // 계산대 10

        RedSnapperFishing = 101,     // 붉은열기 낚시시설 11
        MackerelFishing = 102,       // 고등어 낚시시설 12
        TropicalFishFishing = 103,   // 열대어 낚시시설 13
        CrabFishing = 104,           // 꽃게 낚시시설 14
        AppleFishing = 105,           // 사과 낚시시설 15
        OrangeFishing = 106,           // 오렌지 낚시시설 16
        SquidFishing = 107, //오징어 낚시시설 17 
        CodFishing = 108, //대구 낚시시설 18
        RedStarfishFishing = 109, // 빨간 불가사리 낚시시설
        ShrimpFishing = 110, // 새우 낚시시설
        OctopusFishing = 111, // 문어 낚시시설
        RedSnapperCooking = 1001,     // 붉은열기 요리 시설 15
        SnapperRamenCooking = 1002,   // 열기 라면 요리 시설 16
        TropicalFishCurryCooking = 1003, // 열대어 카레 요리 시설 17
        GrilledCrabCooking = 1004,    // 크랩 구이 요리 시설 18
        CrabCurryCooking = 1005,       // 크랩 카레 요리 시설 19
        CuttingAppleCooking = 1006,       // 썰은사과
        FruitCakeCooking = 1007,      // 케이크
        GrilledSquidCooking = 1008, //오징어 구이 요리시설
        SteamedCodCooking = 1009, //대구 찜 요리시설
        SquidRiceBowlCooking = 1010, // 오징어 덮밥 요리시설
        SeafoodStewCooking = 1011, // 해믈스튜 요리시설
        SeafoodRisottoCooking = 1012, //해물리조또 요리시설  
        MushroomToStarfishExchangeCooking = 1013, // 버섯 => 빨간불가사리 교환 
        FriedStarfishCooking = 1014, // 불가사리 튀김
        AssortedFriesCooking = 1015, // 모둠 튀김
        SeafoodCutletCooking = 1016, // 해물까스
        FriedRiceCooking = 1017, // 튀김덮밥

    }

    public enum FoodType
    {
        None = -1,
        RedSnapper = 1,            // 붉은열기 1
        Mackerel = 2,              // 고등어 2
        GrilledRedSnapper = 3,     // 붉은열기 구이 3
        MackerelSnapperRamen = 4,  // 고등어 열기 라면 4
        TropicalFish = 5,          // 열대어 5
        Crab = 6,                  // 크랩 6
        TropicalFishCurry = 7,     // 열대어 카레 7
        GrilledCrab = 8,           // 크랩 구이 8
        GrilledCrabCurry = 9,       // 크랩 카레 구이 9
        Apple = 10,       // 크랩 카레 구이 9
        Orange = 11,       // 크랩 카레 구이 9
        CuttingApple = 12,       // 크랩 카레 구이 9
        FruiteCake = 13,       // 크랩 카레 구이 9
        Squid = 14, //오징어
        Cod = 15, //대구 
        GrilledSquid = 16, //오징어 구이
        SteamedCod = 17, //대구 찜
        SquidRiceBowl = 18, // 오징어 덮밥
        SeafoodStew = 19, // 해믈스튜
        SeafoodRisotto = 20, //해물리조또 
        RedStarFish = 21,       // 불가사리
        Mushroom = 24,       // 버섯
        Shrimp = 23,         // 새우
        FriedStarfish = 22,  // 불가사리 튀김
        AssortedFried = 25,  // 모듬 튀김
        Octopus = 26,        // 문어
        SeafoodCutlet = 27,  // 해물 까스
        FriedRiceBowl = 28   // 튀김덮밥

    }

    public enum ManagerGrade
    {
        Noraml = 1,
        Rare = 2,
        Unique = 3,
        UnCommon = 0,
    }

    public enum RecordCountKeys
    {
        Init,
        StartStage,
        Navi_Start,
    }



    [System.Serializable]
    public class ColorDefine
    {
        public string key_string;
        public Color color;
    }

    [HideInInspector]
    [SerializeField]
    private List<ColorDefine> _textColorDefines = new List<ColorDefine>();
    [HideInInspector]
    [SerializeField]
    private List<ColorDefine> _eventTextColorDefines = new List<ColorDefine>();
    private Dictionary<string, Color> _textColorDefinesDic = new Dictionary<string, Color>();
    public List<ColorDefine> TextColorDefines
    {
        get
        {
            return _textColorDefines;
        }
    }
    public List<ColorDefine> EventTextColorDefines
    {
        get
        {
            return _eventTextColorDefines;
        }
    }

    [HideInInspector]
    [SerializeField]
    private List<ColorDefine> _imageColorDefines = new List<ColorDefine>();
    [HideInInspector]
    [SerializeField]
    private List<ColorDefine> _eventImgaeColorDefines = new List<ColorDefine>();
    private Dictionary<string, Color> _imageColorDefinesDic = new Dictionary<string, Color>();
    public List<ColorDefine> ImageColorDefines
    {
        get
        {
            return _imageColorDefines;
        }
    }
    public List<ColorDefine> EventImageColorDefines
    {
        get
        {
            return _eventImgaeColorDefines;
        }
    }

    public Material SkeletonGraphicMat;
    public Material DisableSpriteMat;
    public Material EnableSpriteMat;
    public Material ImgAddtiveMat;

    [SerializeField]
    private SpriteAtlas InGameAtlas;

    [SerializeField]
    private SpriteAtlas CommonAtlas;




    public Color GetTextColor(string key)
    {
        if (_textColorDefinesDic.ContainsKey(key))
            return _textColorDefinesDic[key];

        return Color.white;
    }


    public Color GetImageColor(string key)
    {
        if (_imageColorDefinesDic.ContainsKey(key))
            return _imageColorDefinesDic[key];

        return Color.white;
    }

    public Color GetManagerGradeBGColor(ManagerGrade grade)
    {
        switch (grade)
        {
            case ManagerGrade.Noraml:
                return GetImageColor("Card_Normal_Bg");
            case ManagerGrade.Rare:
                return GetImageColor("Card_Rare_Bg");
            case ManagerGrade.Unique:
                return GetImageColor("Card_Unique_Bg");
        }

        return Color.white;
    }

    public Color GetManagerGradeFrameColor(ManagerGrade grade)
    {
        switch (grade)
        {
            case ManagerGrade.Noraml:
                return GetImageColor("Card_Normal_Frame");
            case ManagerGrade.Rare:
                return GetImageColor("Card_Rare_Frame");
            case ManagerGrade.Unique:
                return GetImageColor("Card_Unique_Frame");
        }

        return Color.white;
    }
    public Sprite GetIngameImg(string key)
    {
        return InGameAtlas.GetSprite(key);
    }

    public Sprite GetCommonImg(string key)
    {
        return CommonAtlas.GetSprite(key);
    }

    public Color GetUnitGradeColor(int grade)
    {
        switch (grade)
        {
            case 1:
                return GetImageColor("Unit_Grade_1");
            case 2:
                return GetImageColor("Unit_Grade_2");
            case 3:
                return GetImageColor("Unit_Grade_3");
        }

        return Color.white;
    }


    public void Load()
    {
        _textColorDefinesDic.Clear();
        foreach (var cd in _textColorDefines)
        {
            _textColorDefinesDic.Add(cd.key_string, cd.color);
        }
        foreach (var cd in _eventTextColorDefines)
        {
            _textColorDefinesDic.Add(cd.key_string, cd.color);
        }
        _imageColorDefinesDic.Clear();
        foreach (var cd in _imageColorDefines)
        {
            _imageColorDefinesDic.Add(cd.key_string, cd.color);
        }
        foreach (var cd in _eventImgaeColorDefines)
        {
            _imageColorDefinesDic.Add(cd.key_string, cd.color);
        }
    }
}
