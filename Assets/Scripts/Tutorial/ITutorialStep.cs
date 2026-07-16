using System.Collections;
using UnityEngine;

public interface ITutorialStep
{
    IEnumerator Execute(TutorialManager manager);
    void Skip();
}
