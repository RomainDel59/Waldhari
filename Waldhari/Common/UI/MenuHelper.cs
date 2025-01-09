using System;
using System.Collections.Generic;
using GTA;
using LemonUI;
using LemonUI.Menus;
using LemonUI.Scaleform;
using Waldhari.Behavior.Mission;
using Waldhari.Common.Files;
using Control = GTA.Control;

namespace Waldhari.Common.UI
{
    public static class MenuHelper
    {
        public static void SetButtons(NativeMenu menu)
        {
            menu.Buttons.Clear();
            menu.Buttons.Add(new InstructionalButton(Localization.GetTextByKey("menu_select"), Control.PhoneSelect));
            menu.Buttons.Add(new InstructionalButton(Localization.GetTextByKey("menu_back"), Control.PhoneCancel));
        }
        
        public static void CreateMissionItem<T>(string title, string description, ObjectPool pool, NativeMenu menu) where T : AbstractMissionScript
        {
            var item = new NativeItem(
                Localization.GetTextByKey(title),
                Localization.GetTextByKey(description)
            );
            item.Activated += (sender, e) =>
            {
                var script = Script.InstantiateScript<T>();
                script.MenuPool = pool;
                script.Start(null);
            };
            menu.Add(item);
        }

        public static void CreateCheckItem(string title, string description, string message, NativeMenu menu, Func<int> getValue)
        {
            var item = new NativeItem(
                Localization.GetTextByKey(title),
                Localization.GetTextByKey(description)
            );
            item.Activated += (sender, e) =>
            {
                NotificationHelper.Show(message, new List<string>{getValue().ToString()});
            };
            menu.Add(item);
        }

        public static void CreateSeparator(NativeMenu menu)
        {
            menu.Add(new NativeItem("----------"));
        }
        
    }
}