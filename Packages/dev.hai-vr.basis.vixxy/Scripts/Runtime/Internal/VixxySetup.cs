using HVR.Basis.Comms;
using UnityEngine;

namespace HVR.Basis.Vixxy.Runtime
{
    public class VixxySetup
    {
        public static P12VixxyOrchestrator EnsureInitialized(Component comp)
        {
            var avatar = HVRCommsUtil.GetAvatar(comp);
            if (avatar == null)
            {
                return EnsureSceneHasNonAvatarOrchestrator();
            }

            var existingOrchestrator = avatar.GetComponentInChildren<P12VixxyOrchestrator>(true);
            if (existingOrchestrator != null) return existingOrchestrator;

            var orchestrator = CreateOrchestrator(avatar.transform, "HVRAvatarVixxyOrchestrator", avatar.transform);
            return orchestrator;
        }

        private static P12VixxyOrchestrator EnsureSceneHasNonAvatarOrchestrator()
        {
            var existingOrchestrators = Object.FindObjectsByType<P12VixxyOrchestrator>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var existingOrchestrator in existingOrchestrators)
            {
                var isNotInsideAvatar = HVRCommsUtil.GetAvatar(existingOrchestrator) == null;
                if (isNotInsideAvatar)
                {
                    return existingOrchestrator;
                }
            }

            var sceneOrchestrator = CreateOrchestrator(null, "HVRSceneVixxyOrchestrator", null);
            return sceneOrchestrator;
        }

        private static P12VixxyOrchestrator CreateOrchestrator(Transform contextNullable, string name, Transform parentNullable)
        {
            var go = new GameObject(name);
            if (parentNullable != null)
            {
                go.transform.SetParent(parentNullable);
            }
            go.SetActive(false);
            var gadgetRepository = go.AddComponent<P12GadgetRepository>();
            var sceneOrchestrator = go.AddComponent<P12VixxyOrchestrator>();
            sceneOrchestrator.acquisitionService = AcquisitionService.SceneInstance;
            sceneOrchestrator.gadgetRepository = gadgetRepository;
            sceneOrchestrator.context = contextNullable;
            go.SetActive(true);
            return sceneOrchestrator;
        }
    }
}