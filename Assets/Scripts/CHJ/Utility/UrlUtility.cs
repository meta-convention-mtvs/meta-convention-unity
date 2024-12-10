using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHJ
{
    public class UrlUtility
    {
        public static string ReplaceSpacesWithEncodedValue(string url)
        {
            // URL에서 공백만 %20으로 변환
            return url.Replace(" ", "%20");
        }

        public static string ReplaceSpacesWithUnderline(string url)
        {
            return url.Replace(" ", "_");
        }
    }

}
