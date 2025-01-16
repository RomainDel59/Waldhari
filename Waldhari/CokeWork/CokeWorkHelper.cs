using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.UI;
using iFruitAddon2;
using LemonUI;
using LemonUI.Menus;
using Waldhari.CokeWork.Missions;
using Waldhari.Common.Entities;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.CokeWork
{
    public static class CokeWorkHelper
    {
        public static class Positions
        {
            public static readonly WPosition Property = new WPosition
            {
                Position = new Vector3(-51.17182f, 1911.078f, 195.3615f),
                Rotation = new Vector3(0f, 0f, 80.97736f),
                Heading = 80.97736f
            };
            
            public static readonly WPosition Storage = new WPosition
            {
                Position = new Vector3(-52.77469f, 1887.465f, 195.3669f),
                Rotation = new Vector3(0f, 0f, -178.372f),
                Heading = 181.628f
            };
            
            public static WPosition Parking = new WPosition
            {
                Position = new Vector3(-44.75359f, 1878.744f, 196.2369f),
                Rotation = new Vector3(0f, 0f, 118.0105f),
                Heading = 118.0105f
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
                    Position = new Vector3(-54.52531f, 1896.378f, 195.3613f),
                    Rotation = new Vector3(0f, 0f, 83.15569f),
                    Heading = 83.15569f
                },
                new WPosition
                {
                    Position = new Vector3(-55.13873f, 1906.508f, 195.3613f),
                    Rotation = new Vector3(0f, 0f, 21.9702f),
                    Heading = 21.9702f
                },
                new WPosition
                {
                    Position = new Vector3(-52.706f, 1916.138f, 195.3614f),
                    Rotation = new Vector3(0f, 0f, 15.54415f),
                    Heading = 15.54415f
                }
            };
        }
        
        #region Worker

        public static CokeWorkManufactureScript ManufactureScript;

        public static void StartManufacture(bool force = false)
        {
            if (!CokeWorkSave.Instance.Worker && !force) return;

            if(ManufactureScript == null)
            {
                ManufactureScript = Script.InstantiateScript<CokeWorkManufactureScript>();
            }
            
            ManufactureScript.Start();
        }

        public static void ShowFromWorker(string messageKey, List<string> messageValues = null)
        {
            NotificationHelper.ShowFromDefault(messageKey, "manager", messageValues);
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
            _menu = new NativeMenu(Localization.GetTextByKey("cokework"));
            MenuHelper.SetButtons(_menu);
            _pool.Add(_menu);

            // Create supply mission item
            MenuHelper.CreateMissionItem<CokeWorkSupplyScript>(
                "supply_menu_title",
                "supply_menu_description",
                _pool,
                _menu
            );

            // Create deal mission item
            MenuHelper.CreateMissionItem<CokeWorkDealScript>(
                "deal_menu_title",
                "deal_menu_description",
                _pool,
                _menu
            );

            // Create bulk mission item
            MenuHelper.CreateMissionItem<CokeWorkBulkScript>(
                "bulk_menu_title",
                "bulk_menu_description",
                _pool,
                _menu
            );

            // Separator
            MenuHelper.CreateSeparator(_menu);

            // Create check supply item
            MenuHelper.CreateCheckItem(
                "cokework_menu_check_supply_title",
                "cokework_menu_check_supply_description",
                "cokework_supply_amount",
                _menu,
                () => CokeWorkSave.Instance.Supply
            );

            // Create check product item
            MenuHelper.CreateCheckItem(
                "cokework_menu_check_product_title",
                "cokework_menu_check_product_description",
                "cokework_product_amount",
                _menu,
                () => CokeWorkSave.Instance.Product
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
            _contact = new iFruitContact(Localization.GetTextByKey("cokework"))
            {
                DialTimeout = 1000, // Delay before answering
                Active = true, // true = the contact is available and will answer the phone
                Icon = ContactIcon.MP_MexLt // Contact's icon
            };

            // Linking the Answered event with our function
            _contact.Answered += contact =>
            {
                if (CokeWorkSave.Instance.Worker)
                {
                    PhoneHelper.GetIFruit().Close(1000);
                    Script.Wait(1000);
                    GetMenu().Visible = true;
                }
                else
                {
                    PhoneHelper.GetIFruit().Close(1000);
                    Script.Wait(1000);
                    var script = Script.InstantiateScript<CokeWorkPickUpPedScript>();
                    script.Start();
                }
            };

            Logger.Debug($"Contact='{_contact.Name}' created");

            return _contact;
        }

        public static void ShowFromContact(string messageKey, List<string> messageValues = null)
        {
            NotificationHelper.ShowWithIcon(NotificationIcon.MpMexLt, "miguel", messageKey, messageValues);
        }

        #endregion
    }
}