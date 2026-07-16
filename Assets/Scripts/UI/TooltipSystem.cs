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

    public static void Show(string content, string header = "", float duration = 0.3f)
    {
        Instance.Tooltip.SetText(content, header);
        Instance.Tooltip.FadeIn(duration);
    }

    public static void Hide(float duration = 0f)
    {
        if (!Instance.Tooltip.gameObject.activeSelf) return;
        Instance.Tooltip.FadeOut(duration);
    }

    public static void ShowPart(DinosaurPart part, float duration = 0.3f)
    {
        string header = part.Reference.Name;
        string content = "";
        foreach (var stat in part.Stats)
        {
            content += $"{stat.Type.ToString()}: {stat.Value}\n";
        }
        if (part.Wildcard != null) content += $"{part.Wildcard.Name}: {part.Wildcard.Description}";
        Instance.Tooltip.SetText(content, header);
        Instance.Tooltip.FadeIn(duration);
    }
}
