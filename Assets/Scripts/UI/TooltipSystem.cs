using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem Instance { get; private set; }

    [Header("References")]
    public Tooltip Tooltip;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public static void Show(string content, string header = "")
    {
        Instance.Tooltip.SetText(content, header);
        Instance.Tooltip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        Instance.Tooltip.gameObject.SetActive(false);
    }

    public static void ShowPart(DinosaurPart part)
    {
        string header = part.Reference.Name;
        string content = "";
        foreach (var stat in part.Stats)
        {
            content += $"{stat.Type.ToString()}: {stat.Value}\n";
        }
        Instance.Tooltip.SetText(content, header);
        Instance.Tooltip.gameObject.SetActive(true);
    }
}
