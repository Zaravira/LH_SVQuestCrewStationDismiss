using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;
using Text = UnityEngine.UI.Text;

namespace LH_SVQuestCrewStationDismiss
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]

    public class LH_SVQuestCrewStationDismiss : BaseUnityPlugin
    {
        public const string pluginGuid = "LH_SVQuestCrewStationDismiss";
        public const string pluginName = "LH_SVQuestCrewStationDismiss";
        public const string pluginVersion = "0.0.1";
        private static Button DismissButton;
        private static DockingUI DockingUIInstance;
        private static DockingUI DockingUIInstance2;
        private static GameObject academyPanel;

        public void Awake()
        {
            Harmony.CreateAndPatchAll(typeof(LH_SVQuestCrewStationDismiss));
        }

        [HarmonyPatch(typeof(DockingUI), nameof(DockingUI.ShowQuests))]
        [HarmonyPrefix]
        private static void GetDockingUIPanelRef(DockingUI __instance)
        {
            DockingUIInstance = __instance;
        }

        [HarmonyPatch(typeof(StationQuestSlot), "Setup")]
        [HarmonyPostfix]

        private static void AddDismissButton(GameObject ___imageRank1, int questIndex)
        {
            if (___imageRank1.transform.parent.Find("Shoo") == null)
            {
                GameObject ButtonClose = ___imageRank1.transform.parent.parent.parent.parent.parent.parent.GetChild(0).GetChild(5).gameObject;
                DismissButton = GameObject.Instantiate(ButtonClose.transform.GetComponent<Button>());
                DismissButton.transform.SetParent(___imageRank1.transform.parent);
                DismissButton.transform.localPosition = new Vector3(-18, -18, 0);
                DismissButton.transform.localScale = new Vector3(1, 1, 0);
                DismissButton.transform.name = "Shoo";
                DismissButton.gameObject.SetActive(true);
                DismissButton.onClick = new Button.ButtonClickedEvent();
                DismissButton.onClick.AddListener(() =>
                {
                    Debug.Log("button!");
                    AccessTools.Method(typeof(DockingUI), "PurchaseStationQuest").Invoke(DockingUIInstance, new object[] { questIndex });
                });
            }
        }

        [HarmonyPatch(typeof(DockingUI), "ShowCrewForHire")]
        [HarmonyPrefix]
        private static void GetDockingUIPanelRef2(DockingUI __instance, Transform ___academyPanel)
        {
            Debug.Log("fekkin saved da instance yeh?");
            DockingUIInstance2 = __instance;
            academyPanel = ___academyPanel.gameObject;
        }

        [HarmonyPatch(typeof(CrewHireSlot), "Setup")]
        [HarmonyPostfix]


        private static void AddShooCrewButton(CrewHireSlot __instance)
        {
            if (__instance.transform.Find("Shoo") == null)
            {
                GameObject ButtonClose = academyPanel.transform.parent.parent.parent.GetChild(0).GetChild(5).gameObject;
                DismissButton = GameObject.Instantiate(ButtonClose.transform.GetComponent<Button>());
                DismissButton.transform.SetParent(__instance.gameObject.transform);
                DismissButton.transform.localPosition = new Vector3(-253, 0, 0);
                DismissButton.transform.localScale = new Vector3(1, 1, 0);
                DismissButton.transform.name = "Shoo";
                DismissButton.gameObject.SetActive(true);
                DismissButton.onClick = new Button.ButtonClickedEvent();
                DismissButton.onClick.AddListener(() =>
                {
                    DockingUIInstance2.station.crewForHireIDs.Remove(__instance.crewMember.id);
                    AccessTools.Method(typeof(DockingUI), "ShowCrewForHire").Invoke(DockingUIInstance2, null);
                });
            }
        }

        // DockingUIInstance2.station.crewForHireIDs.Remove(academyPanel.transform.GetChild(academyPanel.transform.childCount - 1).GetComponent<CrewHireSlot>().crewMember.id);

    }
}



