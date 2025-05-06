using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextureImportor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        // 가져오는 TextureImporter 객체
        TextureImporter importer = (TextureImporter)assetImporter;

        if (importer != null)
        {
            // 텍스처를 Sprite로 설정
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;

            // Windows / Mac / Linux 플랫폼의 오버라이드 해제
            RemovePlatformOverride(importer, "Standalone"); // Standalone이 Win/Mac/Linux 공통 
            RemovePlatformOverride(importer, "Android");
            RemovePlatformOverride(importer, "iPhone");

            // 필요시 Android, iPhone은 오버라이드 유지 가능
            // SetPlatformTextureSettings(importer, "Android", TextureImporterFormat.ASTC_8x8);
            // SetPlatformTextureSettings(importer, "iPhone", TextureImporterFormat.ASTC_8x8);
        }
    }

    void RemovePlatformOverride(TextureImporter importer, string platformName)
    {
        TextureImporterPlatformSettings platformSettings = importer.GetPlatformTextureSettings(platformName);
        if (platformSettings.overridden)
        {
            platformSettings.overridden = false;
            importer.SetPlatformTextureSettings(platformSettings);
        }
    }

    void SetPlatformTextureSettings(TextureImporter importer, string platformName, TextureImporterFormat format)
    {
        TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings
        {
            overridden = true,
            name = platformName,
            maxTextureSize = 2048,
            format = format,
            compressionQuality = 50
        };

        importer.SetPlatformTextureSettings(platformSettings);
    }
}
