using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Excalibur;
using UnityEngine.UI;

namespace Excalibur
{
    public sealed class GameActivate : MonoSingleton<GameActivate>
    {
        protected override void Awake()
        {
            base.Awake();

            TheFirstFrame();
        }

        protected override void Start()
        {
            base.Start();

            TheSecondFrame();
        }

        protected override void Update()
        {
            base.Update();

            Execute();
        }

        private void TheFirstFrame()
        {
            InitManagers();
        }

        private void TheSecondFrame()
        {
            //ConfigurationHander.ReadAllData();
        }

        private void InitManagers()
        {
            if (Camera.main == null)
            {
                //Utility.LoadAsset<GameObject>(AssetType.MainCamera, "MainCamera");
                //GameObject light = Utility.LoadAsset<GameObject>(AssetType.MainCamera, "DirectionalLight");
                //light.transform.SetParent(Camera.main.transform);
                //light.transform.localPosition = Vector3.zero;
            }

            if (!GameManager.Initialized())
                this.gameObject.AddComponent<GameManager>();

            if (!InputManager.Initialized())
                this.gameObject.AddComponent<InputManager>();

            //if (!MonoEventManager.Initialized())
            //    this.gameObject.AddComponent<MonoEventManager>();

            if (!FormManager.Initialized())
            {
                GameObject uiNode = GameObject.Find("UINode");
                if (uiNode == null)
                {
                    uiNode = Utility.CreateGameObject("UINode", false);
                }
                uiNode.AddComponent<FormManager>();
            }
        }

        private void Execute()
        {
            if (InputManager.Initialized())
            {
                InputManager.Instance.Execute();
            }

            if (MonoEventManager.Initialized())
            {
                MonoEventManager.Instance.Execute();
            }
        }
    }
}
