using UnityEngine;

public static class SpriteUtility
{
    /// <summary>
    /// Texture2D를 Sprite로 변환합니다.
    /// </summary>
    /// <param name="texture">변환할 Texture2D</param>
    /// <returns>변환된 Sprite</returns>
    public static Sprite ConvertTextureToSprite(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("Texture2D가 null입니다.");
            return null;
        }

        // 전체 Texture2D 영역을 사용하여 Sprite 생성
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f), // 피벗을 중앙으로 설정
            100.0f // Pixels Per Unit 설정 (필요에 따라 조정)
        );
    }
}