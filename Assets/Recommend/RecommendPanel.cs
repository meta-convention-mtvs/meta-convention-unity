using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace UI2.Recommend
{
    public class RecommendPanel : MonoBehaviour
    {
        public List<RecommendItem> items = new();
        public List<RecommendPanelRow> rows = new();
        public RecommendPanelRow rowPrefab;

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
            for (var i = 0; i < 12; i++)
            {
                AddRecommendItem(new RecommendItem()
                {
                    sprite = null,
                    name = "Metaverse Academi",
                    desc = "The company named Metaverse Academi."
                });
            }

        }

        public void AddRecommendItem(RecommendItem item)
        {
            var lastRow = GetLastRow();
            if (lastRow == null || lastRow.ItemAmount >= lastRow.Capacity)
            {
                lastRow = CreateRow();
            }
            items.Add(item);
            lastRow.View(item);
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
        public Sprite sprite;
        public string name;
        public string desc;
    }
}