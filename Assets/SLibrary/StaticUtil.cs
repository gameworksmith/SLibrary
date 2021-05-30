using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace SLibrary
{
    public class StaticUtil
    {
        public static T GetOrAddComponent<T>(GameObject go) where T : Component
        {
            if (ReferenceEquals(go, null))
            {
                return null;
            }

            var ret = go.GetComponent<T>();
            if (ret == null)
            {
                ret = go.AddComponent<T>();
            }

            return ret;
        }
        
        public static EventTrigger AttachEventTriggers(GameObject go, UnityAction<BaseEventData> onClick = null, UnityAction<BaseEventData> onBeginDrag = null,
            UnityAction<BaseEventData> onDrag = null, UnityAction<BaseEventData> onEndDrag = null,UnityAction<BaseEventData> onDrop = null)
        {
            // Profiler.BeginSample("AttachEventTrigger");
            var trigger = GetOrAddComponent<EventTrigger>(go);
            trigger.triggers.Clear();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { onClick?.Invoke((PointerEventData) data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.BeginDrag;
            entry.callback.AddListener((data) => { onBeginDrag?.Invoke((PointerEventData) data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener((data) => { onDrag?.Invoke((PointerEventData) data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.EndDrag;
            entry.callback.AddListener((data) => { onEndDrag?.Invoke((PointerEventData) data); });
            trigger.triggers.Add(entry);

            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drop;
            entry.callback.AddListener((data) => { onDrop?.Invoke((PointerEventData) data); });
            trigger.triggers.Add(entry);
            // Profiler.EndSample();
            return trigger;
        }
    }
}