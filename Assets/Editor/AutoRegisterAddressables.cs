using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.IO;
using System.Text.RegularExpressions;

public class AutoRegisterAddressables : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            // Debug.LogWarning("AddressableAssetSettings를 찾을 수 없습니다. Addressable Asset System이 초기화되었는지 확인하세요.");
            return;
        }

        foreach (string assetPath in importedAssets)
        {
            if (assetPath.EndsWith(".prefab"))
            {
                RegisterOrUpdatePrefab(assetPath, settings);
            }
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            string newAssetPath = movedAssets[i];
            string oldAssetPath = movedFromAssetPaths[i];

            if (newAssetPath.EndsWith(".prefab"))
            {
                RegisterOrUpdatePrefab(newAssetPath, settings);
            }
        }
    }

    static void RegisterOrUpdatePrefab(string assetPath, AddressableAssetSettings settings)
    {
        string fileName = Path.GetFileNameWithoutExtension(assetPath);
        
        // Popup, Page, InGame, Top 이름을 포함한 프리팹만 처리
        if (!IsValidPrefabType(fileName))
        {
            return;
        }
        
        AddressableAssetGroup defaultGroup = settings.DefaultGroup;
        if (defaultGroup == null)
        {
            // 기본 그룹이 없으면 새로 생성
            defaultGroup = settings.CreateGroup("Default Group", false, false, true, null);
            settings.DefaultGroup = defaultGroup;
            if (defaultGroup == null)
            {
                Debug.LogError("기본 어드레서블 에셋 그룹을 찾거나 생성할 수 없습니다.");
                return;
            }
            Debug.LogWarning("기본 어드레서블 그룹이 없어 새로 생성했습니다: Default Group");
        }

        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        if (string.IsNullOrEmpty(guid))
        {
            // Debug.LogWarning($"GUID를 찾을 수 없습니다: {assetPath}. 에셋이 아직 데이터베이스에 완전히 반영되지 않았을 수 있습니다.");
            return;
        }

        // 주소 형식 생성 (UI/Popup/PopupName 또는 UI/Page/PageName 형식)
        string desiredAddress = GetFormattedAddress(fileName);

        AddressableAssetEntry entry = settings.FindAssetEntry(guid);
        if (entry == null)
        {
            entry = settings.CreateOrMoveEntry(guid, defaultGroup, false);
            if (entry != null)
            {
                entry.SetAddress(desiredAddress);
                Debug.Log($"[Addressable 등록] 프리팹 '{fileName}'을(를) 주소 '{desiredAddress}'(으)로 등록했습니다.");
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryAdded, entry, true);
            }
            else
            {
                Debug.LogError($"'{assetPath}'에 대한 Addressable 항목을 생성하지 못했습니다.");
            }
        }
        else
        {
            if (entry.address != desiredAddress)
            {
                string oldAddress = entry.address;
                entry.SetAddress(desiredAddress);
                Debug.Log($"[Addressable 주소 업데이트] 프리팹 '{fileName}'의 주소를 '{oldAddress}'에서 '{desiredAddress}'(으)로 변경했습니다.");
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);
            }
        }
    }
    
    // 프리팹 이름이 Popup, Page, InGame, Top을 포함하는지 확인
    static bool IsValidPrefabType(string fileName)
    {
        return fileName.Contains("Popup") || 
               fileName.Contains("Page") || 
               fileName.Contains("InGame") || 
               fileName.Contains("Top");
    }
    
    // 주소 형식 생성 (UI/Popup/PopupName 또는 UI/Page/PageName 형식)
    static string GetFormattedAddress(string fileName)
    {
        if (fileName.Contains("Popup"))
        {
            return $"UI/Popup/{fileName}";
        }
        else if (fileName.Contains("Page"))
        {
            return $"UI/Page/{fileName}";
        }
        else if (fileName.Contains("InGame"))
        {
            return $"UI/InGame/{fileName}";
        }
        else if (fileName.Contains("Top"))
        {
            return $"UI/Top/{fileName}";
        }
        
        // 기본값 (일반적으로 도달하지 않음)
        return $"UI/{fileName}";
    }
}