using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public static class EventManager
{
   public static UnityEvent GrabOptionEvent = new UnityEvent();
   public static UnityEvent DropOptionEvent = new UnityEvent();
   public static UnityEvent<Topic> TopicSelectedEvent = new UnityEvent<Topic>();
   //public static UnityEvent RefreshGridEvent = new UnityEvent();
}