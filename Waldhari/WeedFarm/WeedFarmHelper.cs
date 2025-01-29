using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.UI;
using iFruitAddon2;
using LemonUI;
using LemonUI.Menus;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;
using Waldhari.WeedFarm.Missions;

namespace Waldhari.WeedFarm
{
    public static class WeedFarmHelper
    {
        public static class Positions
        {
            public static readonly WPosition Property = new WPosition
            {
                Position = new Vector3(2220.382f, 5614.367f, 54.72742f),
                Rotation = new Vector3(-0.0003605913f, 4.733887E-08f, -81.828f),
                Heading = 278.172f
            };
            
            public static readonly WPosition Storage = new WPosition
            {
                Position = new Vector3(2194.004f, 5593.924f, 53.75492f),
                Rotation = new Vector3(-0.000361025f, -7.503591E-05f, 164.5876f),
                Heading = 164.5876f
            };
            
            public static WPosition Parking = new WPosition
            {
                Position = new Vector3(2214.336f, 5600.055f, 54.02474f),
                Rotation = new Vector3(-0.0003605108f, -6.488535E-09f, 1.376211f),
                Heading = 1.376211f
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
                    Position = new Vector3(2222.764f, 5578.261f, 53.70979f),
                    Rotation = new Vector3(-0.0003607887f, -7.461072E-05f, -179.0753f),
                    Heading = 180.9247f
                },
                new WPosition
                {
                    Position = new Vector3(2227.685f, 5577.925f, 53.69601f),
                    Rotation = new Vector3(-0.0003608046f, -7.461171E-05f, 173.252f),
                    Heading = 173.252f
                },
                new WPosition
                {
                    Position = new Vector3(2222.681f, 5576.244f, 53.65268f),
                    Rotation = new Vector3(-0.0003608477f, -7.445754E-05f, 167.5867f),
                    Heading = 167.5867f
                },
                new WPosition
                {
                    Position = new Vector3(2216.26f, 5576.372f, 53.58198f),
                    Rotation = new Vector3(-0.0003608917f, -7.454795E-05f, -2.24531f),
                    Heading = 357.7547f
                }
            };

            public static readonly List<WPositionHelper.GuardPositions> GuardPositionsList =
                new List<WPositionHelper.GuardPositions>
                {
                    new WPositionHelper.GuardPositions
                    {
                        Position = new List<WPosition>
                        {
                            new WPosition
                            {
                                Position = new Vector3(2218.663f, 5600.003f, 54.50877f),
                                Rotation = new Vector3(-2.945041E-07f, -4.635889E-05f, 157.818f),
                                Heading = 157.818f
                            },
                            new WPosition
                            {
                                Position = new Vector3(2215.679f, 5610.32f, 54.50102f),
                                Rotation = new Vector3(-2.870833E-07f, -4.635937E-05f, 17.52297f),
                                Heading = 17.52297f 
                            }
                        }
                    },
                    new WPositionHelper.GuardPositions
                    {
                        Position = new List<WPosition>
                        {
                            new WPosition
                            {
                                Position = new Vector3(2197.66f, 5582.771f, 53.70533f),
                                Rotation = new Vector3(-3.105117E-07f, -4.633842E-05f, -150.0599f),
                                Heading = 209.9401f
                            },
                            new WPosition
                            {
                                Position = new Vector3(2203.246f, 5605.534f, 53.76904f),
                                Rotation = new Vector3(-2.932144E-07f, -4.634442E-05f, -66.31038f),
                                Heading = 293.6896f
                            }
                        }
                    }
                };

        }
        
        #region Worker

        public static WeedFarmManufactureScript ManufactureScript;

        public static void StartManufacture(bool force = false)
        {
            if (!WeedFarmSave.Instance.Worker && !force) return;

            if(ManufactureScript == null)
            {
                ManufactureScript = Script.InstantiateScript<WeedFarmManufactureScript>();
            }
            
            ManufactureScript.Start();
        }

        public static void ShowFromWorker(string messageKey, List<string> messageValues = null)
        {
            NotificationHelper.ShowFromDefault(messageKey, "farmer", messageValues);
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
            var name = Localization.GetTextByKey("weedfarm");
            _menu = new NativeMenu(name);
            _menu.Name = name;
            MenuHelper.SetButtons(_menu);
            _pool.Add(_menu);

            // Create supply mission item
            MenuHelper.CreateMissionItem<WeedFarmSupplyScript>(
                "supply_menu_title",
                "supply_menu_description",
                _pool,
                _menu
            );

            // Create deal mission item
            MenuHelper.CreateMissionItem<WeedFarmDealScript>(
                "deal_menu_title",
                "deal_menu_description",
                _pool,
                _menu
            );

            // Create bulk mission item
            MenuHelper.CreateMissionItem<WeedFarmBulkScript>(
                "bulk_menu_title",
                "bulk_menu_description",
                _pool,
                _menu
            );

            // Separator
            MenuHelper.CreateSeparator(_menu);

            // Create check supply item
            MenuHelper.CreateCheckItem(
                "weedfarm_menu_check_supply_title",
                "weedfarm_menu_check_supply_description",
                "weedfarm_supply_amount",
                _menu,
                () => WeedFarmSave.Instance.Supply
            );

            // Create check product item
            MenuHelper.CreateCheckItem(
                "weedfarm_menu_check_product_title",
                "weedfarm_menu_check_product_description",
                "weedfarm_product_amount",
                _menu,
                () => WeedFarmSave.Instance.Product
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
            _contact = new iFruitContact(Localization.GetTextByKey("weedfarm"))
            {
                DialTimeout = 1000, // Delay before answering
                Active = true, // true = the contact is available and will answer the phone
                Icon = ContactIcon.Barry // Contact's icon
            };

            // Linking the Answered event with our function
            _contact.Answered += contact =>
            {
                if (WeedFarmSave.Instance.Worker)
                {
                    PhoneHelper.GetCharacterPhone().Close(1000);
                    Script.Wait(1000);
                    GetMenu().Visible = true;
                }
                else
                {
                    PhoneHelper.GetCharacterPhone().Close(1000);
                    Script.Wait(1000);
                    var script = Script.InstantiateScript<WeedFarmPickUpPedScript>();
                    script.Start();
                }
            };

            Logger.Debug($"Contact='{_contact.Name}' created");

            return _contact;
        }

        public static void ShowFromContact(string messageKey, List<string> messageValues = null)
        {
            NotificationHelper.ShowWithIcon(NotificationIcon.Barry, "barry", messageKey, messageValues);
        }

        #endregion
    }
}