using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace UI2.Recommend
{
    public class RecommendPanel : MonoBehaviour
    {
        public List<RecommendItem> items = new();
        public List<RecommendPanelRow> rows = new();
        public RecommendPanelRow rowPrefab;
        [SerializeField] private string loadingSceneName;

        public void Awake()
        {
            var items = GetComponentsInChildren<RecommendPanelRow>();
            foreach (var item in items)
            {
                Destroy(item.gameObject);
            }
        }

        public void Start()
        {
            // test data
            //for (var i = 0; i < 12; i++)
            //{
            //    AddRecommendItem(new RecommendItem()
            //    {
            //        sprite_name = "",
            //        company_uuid = "",
            //        name = "Metaverse Academy",
            //        desc = "The company named Metaverse Academy."
            //    });
            //}
        }

        public void AddRecommendItem(RecommendItem item)
        {
            var lastRow = GetLastRow();
            if (lastRow == null || lastRow.ItemAmount >= lastRow.Capacity)
            {
                lastRow = CreateRow();
            }
            items.Add(item);
            lastRow.View(item, loadingSceneName);
        }

        private RecommendPanelRow CreateRow()
        {
            var row = Instantiate(rowPrefab, transform);
            row.transform.SetSiblingIndex(rows.Count);
            rows.Add(row);
            return row;
        }

        private RecommendPanelRow GetLastRow()
        {
            if (rows.Count == 0)
            {
                return null;
            }
            return rows[^1];
        }
    }

    public class RecommendItem
    {
        public string sprite_name;
        public string company_uuid;
        public string name;
        public string desc;
        public string category;
    }
}