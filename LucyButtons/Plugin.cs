using BepInEx;
using Cinemachine;
using GorillaNetworking;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utilla;

namespace LucyButtons
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public static HalloweenGhostChaser Lucy;

        GameObject Buttona;
        List<GameObject> bButtons = new List<GameObject>();
        List<GameObject> rButtons = new List<GameObject>();
        GameObject thig;

        bool inroom;
        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
        }
        void OnGameInitialized(object sender, EventArgs e)
        {
            Lucy = GameObject.Find("Environment Objects/PersistentObjects_Prefab/Halloween2023_PersistentObjects/Halloween Ghost/FloatingChaseSkeleton").GetComponent<HalloweenGhostChaser>();
            thig = GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/gorillachest");
            Buttona = new GameObject("LucyButtons");
            foreach (Transform t in Lucy.spawnTransforms)
            {
                CreatebButtons(thig);
                CreaterButtons(thig);
            }
            bButtons[0].transform.position = new Vector3(-46.507f, 5.464f, -42.671f);
            rButtons[0].transform.position = new Vector3(-46.507f, 5.564f, -42.671f);

            bButtons[1].transform.position = new Vector3(-68.846f, -26.501f, -28.963f);
            rButtons[1].transform.position = new Vector3(-68.846f, -26.601f, -28.963f);

            bButtons[2].transform.position = new Vector3(-117.08f, -3.448f, -155.41f);
            rButtons[2].transform.position = new Vector3(-117.08f, -3.548f, -155.41f);

            bButtons[3].transform.position = new Vector3(25.0243f, 0.8849f, -37.0995f);
            rButtons[3].transform.position = new Vector3(25.0243f, 0.9849f, -37.0995f);

            bButtons[4].transform.position = new Vector3(-57.52f, 6.356f, -155.853f);
            rButtons[4].transform.position = new Vector3(-57.52f, 6.456f, -155.853f);
        }

        void Update()
        {
            if (!inroom)
            {
                Buttona.SetActive(false);
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                if (Lucy.currentState == HalloweenGhostChaser.ChaseState.Dormant)
                {
                    Buttona.SetActive(true);
                }
                else
                {
                    Buttona.SetActive(false);
                }
            }
        }
        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            inroom = true;
        }
        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            inroom = false;
            Buttona.SetActive(false);
        }

        void CreatebButtons(GameObject templateButton)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshFilter meshFilter = cube.GetComponent<MeshFilter>();
            GameObject CreatePageButton()
            {
                GameObject button = GameObject.Instantiate(templateButton);
                button.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
                button.GetComponent<Renderer>().material = new Material(GorillaComputer.instance.unpressedMaterial);
                button.GetComponent<Renderer>().material.color = Lucy.defaultColor;
                Destroy(cube);
                button.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                Destroy(cube);
                button.AddComponent<BoxCollider>();
                button.GetComponent<BoxCollider>().isTrigger = true;
                button.layer = LayerMask.NameToLayer("GorillaInteractable");
                button.AddComponent<LucyButton>();
                return button;
            }
            GameObject b = CreatePageButton();
            b.name = "Button";
            Destroy(cube);
            b.transform.SetParent(Buttona.transform, false);
            bButtons.Add(b);
        }
        void CreaterButtons(GameObject templateButton)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            MeshFilter meshFilter = cube.GetComponent<MeshFilter>();
            GameObject CreatePageButton()
            {
                GameObject button = GameObject.Instantiate(templateButton);
                button.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
                button.GetComponent<Renderer>().material = new Material(GorillaComputer.instance.unpressedMaterial);
                button.GetComponent<Renderer>().material.color = Lucy.summonedColor;
                Destroy(cube);
                button.transform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
                Destroy(cube);
                button.AddComponent<BoxCollider>();
                button.GetComponent<BoxCollider>().isTrigger = true;
                button.layer = LayerMask.NameToLayer("GorillaInteractable");
                button.AddComponent<LucyButton>().Summoned = true;
                return button;
            }
            GameObject b = CreatePageButton();
            b.name = "Button";
            Destroy(cube);
            b.transform.SetParent(Buttona.transform, false);
            rButtons.Add(b);
        }
    }
    class LucyButton : MonoBehaviour
    {
        public bool Summoned;
        private void OnTriggerEnter(Collider collider)
        {
            GorillaTriggerColliderHandIndicator component = collider.GetComponent<GorillaTriggerColliderHandIndicator>();
            if (!(component == null))
            {
                ButtonActivation();
                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, component.isLeftHand, 0.05f);
                GorillaTagger.Instance.StartVibration(component.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration);
                if (PhotonNetwork.InRoom && GorillaTagger.Instance.myVRRig != null)
                {
                    GorillaTagger.Instance.myVRRig.RPC("PlayHandTap", RpcTarget.Others, 67, component.isLeftHand, 0.05f);
                }
            }
        }
        void ButtonActivation()
        {
            Plugin.Lucy.isSummoned = Summoned;
            Plugin.Lucy.nextTimeToChasePlayer = 0.5f;
        }
    }
}
