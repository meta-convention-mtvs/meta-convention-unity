using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CHJ
{
    public static class EnumUtility
    {
        public static T? GetEnumValue<T>(string value) where T : struct, Enum
        {
            if (Enum.TryParse(typeof(T), value, true, out var result))
            {
                return (T)result;
            }
            return null; // 잘못된 입력인 경우 null 반환
        }
    }
}
