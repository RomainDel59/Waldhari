using System.Collections.Generic;
using GTA;
using GTA.Math;
using iFruitAddon2;
using LemonUI;
using LemonUI.Menus;
using Waldhari.Common.Entities;
using Waldhari.Common.Files;
using Waldhari.Common.UI;
using Waldhari.MethLab.Missions;

namespace Waldhari.MethLab
{
    public static class MethLabHelper
    {
        
        public static Vector3 PropertyPosition = new Vector3(1394.793f, 3598.564f, 34.98966f);
        public static WPosition LaboratoryPosition = new WPosition
        {
            Position = new Vector3(1389.134f, 3605.236f, 38.94193f),
            //todo: add rotation and heading
        };
        
        // no need at the moment
        public static PedHash Chemist = (PedHash)3988008767;
        public static void ShowFromChemist(string messageKey, List<string> messageValues = null)
        {
            NotificationHelper.ShowFromDefault(messageKey, "chemist_sender", messageValues);
        }
        
        public static Vector3 LabParking = new Vector3(1381.139f, 3597.028f, 34.87445f);

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
            MenuHelper.CreateMissionItem<SupplyMissionScript>(
                "methlab_menu_supply_title",
                "methlab_menu_supply_description",
                _pool,
                _menu
            );

            // Create manufacture mission item
            MenuHelper.CreateMissionItem<ManufactureMissionScript>(
                "methlab_menu_manufacture_title",
                "methlab_menu_manufacture_description",
                _pool,
                _menu
            );
            
            // Create deal mission item
            MenuHelper.CreateMissionItem<DealMissionScript>(
                "methlab_menu_deal_title",
                "methlab_menu_deal_description",
                _pool,
                _menu
            );
            
            // Create bulk mission item
            MenuHelper.CreateMissionItem<BulkMissionScript>(
                "methlab_menu_bulk_title",
                "methlab_menu_bulk_description",
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
                DialTimeout = 1000,            // Delay before answering
                Active = true,                 // true = the contact is available and will answer the phone
                Icon = ContactIcon.Ron       // Contact's icon
            };
            
            // Linking the Answered event with our function
            _contact.Answered += contact =>
            {
                PhoneHelper.GetIFruit().Close(1000);
                //todo: make a pause of 1 sec before show menu
                GetMenu().Visible = true;
            };
            
            return _contact;
        }
        
        #endregion
        
    }
}