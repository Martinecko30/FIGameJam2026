using Dialogue;
using FPSDemo.NPC;
using UnityEngine;

public class MadManCall : MonoBehaviour
{
    [SerializeField] private Dialog dialog;
    [SerializeField] private GameObject guardPoint;
    private void Start()
    {
        dialog.finishesDialog.AddListener(delegate
        {
            guardPoint.GetComponent<ScriptedGuardAlert>().TriggerInvestigation();
        });
    }
}
