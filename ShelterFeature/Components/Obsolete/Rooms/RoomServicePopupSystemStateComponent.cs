using HECSFramework.Core;
using HECSFramework.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Components
{
    [Obsolete]
    [Serializable][Documentation(Doc.UI, Doc.Shelter, "RoomServicePopupSystemStateComponent")]
    public sealed class RoomServicePopupSystemStateComponent : BaseComponent
    {
        public HECSList<RowContext> RowContexts = new HECSList<RowContext>(16);
        public Action<RowContext> OnAddResourceButtonClick;

        public void RegisterRow(Entity room, UIAccessMonoComponent rowUIAccess, ResourceForServiceModel rowConfig)
        {
            var button = rowUIAccess.GetButton(UIAccessIdentifierMap.Button);
            var instanceID = button.gameObject.GetInstanceID();

            button.onClick.AddListener(ClickAddResourceButton);

            var context = new RowContext
            {
                Button = button,
                ButtonInstanceID = instanceID,
                Room = room,
                RowUIAccess = rowUIAccess,
                RowConfig = rowConfig
            };

            RowContexts.Add(context);
        }

        public void ClearContexts()
        {
            foreach(var context in RowContexts)
            {
                context.Button.onClick.RemoveListener(ClickAddResourceButton);
                MonoBehaviour.Destroy(context.RowUIAccess.gameObject); //todo - pool
                context.ClearContext();
            }

            RowContexts.ClearFast();
        }

        private void ClickAddResourceButton()
        {
            var clickedObjectInstanceID = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetInstanceID();

            for(int i = 0; i < RowContexts.Count; i++)
            {
                if (RowContexts[i].ButtonInstanceID != clickedObjectInstanceID)
                    continue;

                OnAddResourceButtonClick?.Invoke(RowContexts[i]);
                break;
            }
        }
    }

    public class RowContext
    {
        public int ButtonInstanceID;
        public Button Button;
        public UIAccessMonoComponent RowUIAccess;
        public Entity Room;
        public ResourceForServiceModel RowConfig;

        public void ClearContext()
        {
            Button = null;
            RowUIAccess = null;
            RowConfig = null;
            Room = null;
        }
    }
}