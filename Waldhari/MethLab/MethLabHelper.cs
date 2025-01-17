using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.UI;
using iFruitAddon2;
using LemonUI;
using LemonUI.Menus;
using Waldhari.Common.Entities;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;
using Waldhari.MethLab.Missions;

namespace Waldhari.MethLab
{
    public static class MethLabHelper
    {
        public static class Positions
        {
            public static readonly WPosition Property = new WPosition
            {
                Position = new Vector3(1394.511f, 3598.316f, 34.98711f),
                Rotation = new Vector3(0.000529588f, -0.0006858803f, -167.1918f),
                Heading = 192.8082f
            };

            public static readonly WPosition Storage = new WPosition
            {
                Position = new Vector3(1394.949f, 3613.84f, 34.98093f),
                Rotation = new Vector3(0.0005374776f, -0.0007078056f, 15.72955f),
                Heading = 15.72955f
            };

            public static readonly WPosition Parking = new WPosition
            {
                Position = new Vector3(1380.113f, 3599f, 34.88007f),
                Rotation = new Vector3(0f, 0f, -162.9709f),
                Heading = 197.0291f
            };

            public static WPosition GetWorkstation()
            {
                var index = RandomHelper.Next(0, Workstations.Count);
                return Workstations[index];
            }

            private static readonly List<WPosition> Workstations = new List<WPosition>
            {
                new WPosition
                {
                    Position = new Vector3(1388.98f, 3604.892f, 38.94193f),
                    Rotation = new Vector3(0.0005379676f, -0.0006872303f, -69.14804f),
                    Heading = 290.852f
                },
                new WPosition
                {
                    Position = new Vector3(1390.096f, 3608.829f, 38.94193f),
                    Rotation = new Vector3(0.0005379398f, -0.0006871976f, 23.74725f),
                    Heading = 23.74725f
                },
                new WPosition
                {
                    Position = new Vector3(1394.383f, 3601.791f, 38.94189f),
                    Rotation = new Vector3(0.0005868266f, -0.0006681096f, -161.9645f),
                    Heading = 198.0355f
                }
            };
        }

        #region Worker

        public static MethLabManufactureScript ManufactureScript;

        public static void StartManufacture(bool force = false)
        {
            if (!MethLabSave.Instance.Worker && !force) return;

            if(ManufactureScript == null)
            {
                ManufactureScript = Script.InstantiateScript<MethLabManufactureScript>();
            }
            
            ManufactureScript.Start();
        }

        public static void ShowFromWorker(string messageKey, List<string> messageValues = null)
        {
            NotificationHelper.ShowFromDefault(messageKey, "chemist", messageValues);
        }

        #endregion


        #region menu

        private static ObjectPool _pool;

        public static void ProcessMenu()
        {
            _pool?.Process();
        }

        private static NativeMenu _menu;

        public static NativeMenu GetMenu()
        {
            if (_pool != null && _menu != null) return _menu;

            // Create main menu
            _pool = new ObjectPool();
            _menu = new NativeMenu(Localization.GetTextByKey("methlab"));
            MenuHelper.SetButtons(_menu);
            _pool.Add(_menu);

            // Create supply mission item
            MenuHelper.CreateMissionItem<MethLabSupplyScript>(
                "supply_menu_title",
                "supply_menu_description",
                _pool,
                _menu
            );

            // Create deal mission item
            MenuHelper.CreateMissionItem<MethLabDealScript>(
                "deal_menu_title",
                "deal_menu_description",
                _pool,
                _menu
            );

            // Create bulk mission item
            MenuHelper.CreateMissionItem<MethLabBulkScript>(
                "bulk_menu_title",
                "bulk_menu_description",
                _pool,
                _menu
            );

            // Separator
            MenuHelper.CreateSeparator(_menu);

            // Create check supply item
            MenuHelper.CreateCheckItem(
                "methlab_menu_check_supply_title",
                "methlab_menu_check_supply_description",
                "methlab_supply_amount",
                _menu,
                () => MethLabSave.Instance.Supply
            );

            // Create check product item
            MenuHelper.CreateCheckItem(
                "methlab_menu_check_product_title",
                "methlab_menu_check_product_description",
                "methlab_product_amount",
                _menu,
                () => MethLabSave.Instance.Product
            );

            Logger.Debug($"Menu='{_menu.Name}' created");

            return _menu;
        }

        #endregion

        #region phone

        private static iFruitContact _contact;

        public static iFruitContact GetContact()
        {
            if (_contact != null) return _contact;
            _contact = new iFruitContact(Localization.GetTextByKey("methlab"))
            {
                DialTimeout = 1000, // Delay before answering
                Active = true, // true = the contact is available and will answer the phone
                Icon = ContactIcon.Ron // Contact's icon
            };

            // Linking the Answered event with our function
            _contact.Answered += contact =>
            {
                if (MethLabSave.Instance.Worker)
                {
                    PhoneHelper.GetCharacterPhone().Close(1000);
                    Script.Wait(1000);
                    GetMenu().Visible = true;
                }
                else
                {
                    PhoneHelper.GetCharacterPhone().Close(1000);
                    Script.Wait(1000);
                    var script = Script.InstantiateScript<MethLabPickUpPedScript>();
                    script.Start();
                }
            };

            Logger.Debug($"Contact='{_contact.Name}' created");

            return _contact;
        }

        public static void ShowFromContact(string messageKey, List<string> messageValues = null)
        {
            NotificationHelper.ShowWithIcon(NotificationIcon.Ron, "ron", messageKey, messageValues);
        }

        #endregion
    }
}